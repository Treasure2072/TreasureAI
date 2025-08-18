using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UIObjectiveItem : MonoBehaviour, IObjective, IMessageReceiver
    {
        #region Properties

        [Header("References")]
        [SerializeField] private TextMeshProUGUI tmpDescription;
        
        [Header("Reference - Claim")]
        [SerializeField] private GameObject objCoin;
        [SerializeField] private TextMeshProUGUI tmpCoin;
        [SerializeField] private GameObject objDice;
        [SerializeField] private TextMeshProUGUI tmpDice;
        [SerializeField] private GameObject objToken;
        [SerializeField] private TextMeshProUGUI tmpToken;
        
        [Header("References - Sign")]
        [SerializeField] private Button btnGet;
        [SerializeField] private GameObject objCollected;

        private UnityAction<IObjective> OnCollectAction { get; set; }
        
        private string Id { get; set; }
        
        #endregion

        #region Unity

        private void Awake()
        {
            btnGet.onClick.AddListener(OnClickGet);
            PlayerSandbox.Instance.ObjectiveHandler.ObjectiveDailyChanged += OnObjectiveDailyChanged;
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
        }

        private void OnDestroy()
        {
            PlayerSandbox.Instance.ObjectiveHandler.ObjectiveDailyChanged -= OnObjectiveDailyChanged;
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler -= OnReceiveMessage;
        }

        #endregion

        #region Function

        public Transform GetTransform()
        {
            return this.transform;
        }

        public void SetContent()
        {
            tmpDescription.text = $"{GetDescription(Id)} {GetProgress(Id)}";
        }

        private string GetDescription(string id)
        {
            var maxProgress = PlayerSandbox.Instance.ChessBoardHandler.Objectives.GetMaxProgressById(id);
            if (id.StartsWith("dice"))
            {
                return string.Format(this.GetLocalizedText("objective-dice-fmt"), NumberUtils.GetDisplayNumberString(maxProgress));
            }

            if (id.StartsWith("coin"))
            {
                return string.Format(this.GetLocalizedText("objective-coin-fmt"), NumberUtils.GetDisplayNumberString(maxProgress));
            }

            return "";
        }
        
        private string GetProgress(string id)
        {
            var maxProgress = PlayerSandbox.Instance.ChessBoardHandler.Objectives.GetMaxProgressById(id);
            var progress = PlayerSandbox.Instance.ObjectiveHandler.Daily.GetProgressById(id);
            return $"({NumberUtils.GetDisplayNumberString(progress)}/{NumberUtils.GetDisplayNumberString(maxProgress)})";
        }

        private bool CanClaim(string id)
        {
            var completed = PlayerSandbox.Instance.ObjectiveHandler.Daily.IsCompletedById(id);
            var collected = PlayerSandbox.Instance.ObjectiveHandler.Daily.IsCollectedById(id);
            return completed && !collected;
        }

        public void SetClaimNums()
        {
            var dailyRouter = MissionInstance.Instance.Settings.GetTaskDailyRouteById(Id);
            objCoin.gameObject.SetActive(dailyRouter.coin > 0);
            objDice.gameObject.SetActive(dailyRouter.dice > 0);
            objToken.gameObject.SetActive(dailyRouter.token > 0);
            
            tmpCoin.SetText(NumberUtils.GetDisplayNumberString(dailyRouter.coin));
            tmpDice.SetText(NumberUtils.GetDisplayNumberString(dailyRouter.dice));
            tmpToken.SetText(NumberUtils.GetDisplayNumberStringAsCurrency(dailyRouter.token, 1));
        }
        

        public void RefreshCollectStatus()
        {
            var maxProgress = PlayerSandbox.Instance.ChessBoardHandler.Objectives.GetMaxProgressById(Id);
            var progress = PlayerSandbox.Instance.ObjectiveHandler.Daily.GetProgressById(Id);
            if (CanClaim(Id))
            {
                // 可以被领取
                objCollected.SetActive(false);
                btnGet.gameObject.SetActive(true);
                btnGet.interactable = true;
            }
            else if(progress < maxProgress)
            {
                // 还未完成
                objCollected.SetActive(false);
                btnGet.gameObject.SetActive(false);
                btnGet.interactable = false;
            }
            else
            {
                // 已经被领取
                objCollected.SetActive(true);
                btnGet.gameObject.SetActive(false);
                btnGet.interactable = false;
            }
        }

        #endregion

        #region API

        public void Initialize(string id, UnityAction<IObjective> onCollectAction)
        {
            Id = id;
            OnCollectAction = onCollectAction;
            SetContent();
            RefreshCollectStatus();
            SetClaimNums();
        }

        #endregion

        #region Callback

        private void OnClickGet()
        {
            if (!CanClaim(Id)) return;
            this.LogEditorOnly($"领取奖励{Id}");
            GameSessionAPI.ObjectiveAPI.RewardDaily(Id);
        }

        private void OnObjectiveDailyChanged(FObjectiveDaily preValue, FObjectiveDaily newValue)
        {
            // 已经被领取
            if (!preValue.IsCollectedById(Id) && newValue.IsCollectedById(Id))
            {
                RefreshCollectStatus();
            }
        }

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess())
            {
                this.LogErrorEditorOnly(response.error);
                return;
            }

            if (service != GameSessionAPI.ObjectiveAPI.ServiceName || method != GSObjectiveAPI.MethodRewardDaily) return;
            if (!response.GetAttachmentAsString("objective").Equals(Id)) return;
            OnCollectAction?.Invoke(this);
            // 领取成功
            var dice = response.GetAttachmentAsInt("dice");
            var coin = response.GetAttachmentAsInt("coin");
            var token = response.GetAttachmentAsFloat("token");
            var tasks = new List<IQueueableEvent>
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
            
            EventQueue.Instance.Enqueue(tasks);
            PlayerSandbox.Instance.ObjectiveHandler.Daily.CompletedById(Id);
        }

        #endregion
    }
}