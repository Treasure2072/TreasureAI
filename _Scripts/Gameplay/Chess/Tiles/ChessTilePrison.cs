using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class ChessTilePrison : ChessTile
    {

        #region Fields

        [Header("References")]
        [SerializeField] private Transform CageTransform;
        [SerializeField] private GameObject smoothEffectObject;
        
        [Header("Settings - Cage - Down")]
        public float DownDuring = 0.5f;
        public float DownDelay = 0.0f;
        public Ease DownEaseType = Ease.OutQuart;
        [SerializeField] private Vector3 CageDownPosition;
        
        [Header("Settings - Cage - Up")]
        public float UpDuring = 0.5f;
        public float UpDelay = 0.0f;
        public Ease UpEaseType = Ease.OutQuart;
        [SerializeField] private Vector3 CageUpPosition;

        #endregion

        #region Prtoperties

        private List<int> Dices { get; set; }
        private int PunishCoin { get; set; }
        private bool bReceiveArriveMessage { get; set; }

        #endregion

        #region ChessTile
        
        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            World.GetPlayer<GameCharacter>()?.GetCharacterAnimatorInterface().Sad();

            /*
             * 1.等待角色站立到prison上
             * 2.铁笼子落下
             * 3.等待服务端传回数据
             * 4.弹出窗口UI
             * 5.等待UI结束
             * 6.铁笼子升起
             * 7.播放相关特效
             */
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(IsPlayerStandThisTile),
                new CustomEvent(CageDown),
                new WaitForSecondEvent(0.5f),
                new GameObjectVisibilityEvent(smoothEffectObject),
                new WaitForSecondEvent(0.5f),
                new WaitForTrueEvent(() => bReceiveArriveMessage),
                new CustomEvent(() => { bReceiveArriveMessage = false; }),
                new CustomEvent(ShowLayer),
                new MoveCameraEvent(this, World.GetRegisteredObject("PrisonDice").transform, 0.25f),
                new WaitForTrueEvent(IsPrisonLayerHide),
                new MoveCameraEvent(this, null),
                new CustomEvent(CageUp),
                new CustomEvent(() =>
                {
                    PlayerSandbox.Instance.CharacterHandler.Coin -= Math.Abs(PunishCoin);
                }),
                new ConditionalEvent(IsSucceedEscape, 
                    () => new List<IQueueableEvent>
                    {
                        EffectsAPI.CreateSoundEffect(() => EffectsAPI.EEffectType.Coin),
                    },
                    () => new List<IQueueableEvent>
                    {
                        new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectMinusCoinNumber, () => -Math.Abs(PunishCoin)),
                    }
                ),
                new ConditionalEvent(() => !IsSucceedEscape(), () => new List<IQueueableEvent>
                {
                    new CustomEvent(() => { SoundAPI.PlaySound(AudioInstance.Instance.Settings.bad); })
                })
            };
        }

        #endregion

        #region Function

        private bool IsSucceedEscape()
        {
            return Dices.Sum() >= 24;
        }

        private bool IsPlayerStandThisTile()
        {
            return TilesAPI.GetManhattanDistance(World.GetPlayer().position, transform.position) <= 0.01f;
        }

        private void ShowLayer()
        {
            UIPrisonLayer.ShowUIPrisonLayer(Dices, PunishCoin);
            if (GameInstance.Instance.HostingHandler.Hosting)
            {
                StartCoroutine(UIPrisonLayer.GetLayer().AutoRollDice());
            }
        }
        private bool IsPrisonLayerHide()
        {
            var layer = UIManager.Instance.GetLayer("UIPrisonLayer");
            return !layer.IsShowing;
        }
        
        private void CageDown()
        {
            if (!CageTransform) return;
            CageTransform.gameObject.SetActive(true);
            CageTransform.DOComplete();
            CageTransform.localPosition = CageUpPosition;
            var tween = CageTransform.DOLocalMove(CageDownPosition, DownDuring).SetEase(DownEaseType);
            if(DownDelay > 0.0f) tween.SetDelay(DownDelay);
        }

        private void CageUp()
        {
            if (!CageTransform) return;
            CageTransform.DOComplete();
            CageTransform.localPosition = CageDownPosition;
            var tween = CageTransform.DOLocalMove(CageUpPosition, UpDuring).SetEase(UpEaseType);
            tween.onComplete = () =>
            {
                CageTransform.gameObject.SetActive(false);
            };
            if(UpDelay > 0.0f) tween.SetDelay(UpDelay);
        }

        #endregion

        #region Callback

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "prison") return;
            bReceiveArriveMessage = true;
            response.body.TryGetValue("dices", out var dices);
            Dices = JsonConvert.DeserializeObject<List<int>>(JsonConvert.SerializeObject(dices));
            PunishCoin = response.GetAttachmentAsInt("coin");
        }

        #endregion
    }
}
