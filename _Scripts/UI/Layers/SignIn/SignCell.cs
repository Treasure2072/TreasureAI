using System.Collections.Generic;
using _Scripts.Utils;
using Data;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using NBitcoin;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace Game
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class SignCell : MonoBehaviour
    {
        #region Property

        [Header("Settings")]
        [SerializeField] private Color uncollected = Color.white;
        [SerializeField] private Color collected = Color.gray;
        
        [Header("Reference")]
        [SerializeField] private Image bg;
        [SerializeField] private Transform lightTransform;
        [SerializeField] private Button buttonCollect;
        
        private int DayIndex { get; set; }

        private bool Initialized { get; set; } = false;
        
        private Tweener RotateTweener { get; set; }
        
        private bool IsClicked { get; set; } = false;

        #endregion
        
        #region Unity

        protected virtual void Awake()
        {
            buttonCollect.onClick.AddListener(OnCollectClick);
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
        }
        
        #endregion

        #region Function

        /// <summary>
        /// 当前 cell是否可以被领取
        /// conditions : loginDays中长度等于DayIndex
        /// </summary>
        /// <returns></returns>
        private bool CanCollect()
        {
            var loginDays = PlayerSandbox.Instance.ObjectiveHandler.CheckIn.loginDays;
            return loginDays.Count % 7 == DayIndex && (loginDays.Count == 0 || loginDays[^1] == TimeAPI.GetVietnamDayNumber() - 1);
        }

        private bool IsCollected()
        {
            return PlayerSandbox.Instance.ObjectiveHandler.CheckIn.loginDays.Count > DayIndex;
        }

        private void Refresh()
        {
            SetButton(CanCollect());
            SetLight(CanCollect());
            SetCollected(IsCollected());

        }

        private void SetButton(bool active)
        {
            buttonCollect.interactable = active;
        }

        private void SetLight(bool active)
        {
            lightTransform.gameObject.SetActive(active);
            if (active)
            {
                if (RotateTweener == null || !RotateTweener.IsActive() || !RotateTweener.IsPlaying())
                {
                    RotateTweener = lightTransform.DOLocalRotate(
                            new Vector3(0, 0, -360),
                            2f,
                            RotateMode.FastBeyond360)
                        .SetLoops(-1, LoopType.Restart)
                        .SetEase(Ease.Linear)
                        .SetUpdate(true); // 可选：不受 Time.timeScale 影响
                }
            }
            else
            {
                if (RotateTweener != null)
                {
                    RotateTweener.Kill();
                    RotateTweener = null;
                }

                // 重置角度（可选）
                lightTransform.rotation = Quaternion.identity;
            }
        }

        private void SetCollected(bool collect)
        {
            bg.color = collect ? collected : uncollected;
        }

        #endregion

        #region API

        public void Init(int dayIndex)
        {
            DayIndex = dayIndex;
            Refresh();
            Initialized = true;
        }

        #endregion

        #region Callback

        private void OnCollectClick()
        {
            if(!Initialized) return;
            GameSessionAPI.ObjectiveAPI.CompleteCheckIn();
        }

        #endregion
        
        #region Callback - Socket Receiver

        private void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (GameSessionAPI.ObjectiveAPI.ServiceName == service && GSObjectiveAPI.MethodCheckIn == method)
            {
                if (response.IsSuccess())
                {
                    var checkDataJson = response.GetAttachmentAsString("records");
                    var records = JsonConvert.DeserializeObject<FObjectiveCheckIn>(checkDataJson);

                    if (records.loginDays.Count != DayIndex + 1) return;
                    PlayerSandbox.Instance.ObjectiveHandler.CheckIn = records;
                    
                    var coin = response.GetAttachmentAsInt("coin");
                    var dice = response.GetAttachmentAsInt("dice");
                    var token = response.GetAttachmentAsFloat("token");
                    
                    Refresh();

                    var task = new List<IQueueableEvent>
                    {
                        EffectsAPI.CreateTip(() => coin, () => dice, () => token),
                        EffectsAPI.CreateSoundEffect(() => EffectsAPI.EEffectType.Coin),
                        EffectsAPI.CreateScreenFullEffect(() => coin, () => dice, () => token),
                        new CustomEvent(() =>
                        {
                            PlayerSandbox.Instance.CharacterHandler.Coin += coin;
                            PlayerSandbox.Instance.CharacterHandler.Dice += dice;
                            PlayerSandbox.Instance.CharacterHandler.Token += Mathf.Approximately(token, -1) ? 0 : token;
                        })
                    };
                    EventQueue.Instance.Enqueue(task);
                }
                else
                {
                    var error = response.GetError();
                    var localized = this.GetLocalizedText(response.GetError());
                    UITipLayer.DisplayTip(
                        this.GetLocalizedText("error"),
                        localized.IsNullOrEmpty() ? error : this.GetLocalizedText(error),
                        UITipLayer.ETipType.Bad);
                }
            }
        }

        #endregion
    }
}