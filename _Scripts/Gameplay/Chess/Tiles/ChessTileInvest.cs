using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class ChessTileInvest : ChessTile
    {
        #region Fields

        [Header("References")]
        [SerializeField] private GameObject RibbonsEffectObject;

        #endregion

        #region Properties

        private bool bReceiveArriveMessage { get; set; } = false;
        private int Invest { get; set; }

        #endregion
        
        #region ChessTile
        
        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            World.GetPlayer<GameCharacter>()?.GetCharacterAnimatorInterface().Happy();
            var layer = UITutorialLayer.GetLayer("UITutorialLayer-Invest");
            return new List<IQueueableEvent>
            {
                new CustomEvent(() =>
                {
                    if (TutorialHandler.IsTriggerable(TutorialHandler.FirstStandInvestKey))
                    {
                        UIStaticsLayer.HideUIStaticsLayer();
                        UIActivityLayer.HideUIActivityLayer();
                        UIChessboardLayer.HideLayer();
                        layer.OnHidEvent.AddListener(() =>
                        {
                            TutorialHandler.Triggered(TutorialHandler.FirstStandInvestKey);
                            UIStaticsLayer.ShowUIStaticsLayer();
                            UIActivityLayer.ShowUIActivityLayer();
                            UIChessboardLayer.ShowLayer();
                        });
                        layer.Show();
                    }
                }),
                new WaitForTrueEvent(() => !layer.IsShowing),
                new WaitForTrueEvent(() => bReceiveArriveMessage),
                new CustomEvent(() => { bReceiveArriveMessage = false; }),
                new GameObjectVisibilityEvent(RibbonsEffectObject),
                new MoveCameraEvent(this, World.GetRegisteredObject("Bank").transform),
                new CoinFlyEvent(this, () => Invest, EffectInstance.Instance.Settings.uiBankCoinTip, World.GetRegisteredObject("Bank").transform),
                new CustomEvent(() => { PlayerSandbox.Instance.ChessBoardHandler.InvestCoin += Invest; }),
                new MoveCameraEvent(this, null)
            };
        }

        #endregion

        #region Callbacks

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "invest") return;
            bReceiveArriveMessage = true;
            Invest = response.GetAttachmentAsInt("invest");
        }

        #endregion
    }

    internal class CoinFlyEvent : ChessTileEvent
    {
        #region Properties

        private Transform Target { get; set; }
        private UIWorldElement CoinTip { get; set; }
        
        private Func<int> GetCoin { get; set; }
        
        // private bool bCompleted { get; set; } = false;
        
        private float FinishTs { get; set; }
        
        // private float DelayDisappear { get; set; }

        #endregion
        
        #region ChessTileEvent

        public CoinFlyEvent(ChessTile tile, Func<int> coinFunc, UIWorldElement coinTipPrefab, Transform target, float delayDisappear = 2f) : base(tile)
        {
            GetCoin = coinFunc;
            CoinTip = coinTipPrefab;
            Target = target;
            // DelayDisappear = delayDisappear;
            // bCompleted = false;
            FinishTs = Time.unscaledTime + delayDisappear;
        }

        public override void OnExecute()
        {
            base.OnExecute();
            var layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
            var coinTip = layer.SpawnWorldElement<UIWSBankTip>(CoinTip, Target.position);
            coinTip.SetCoin((int)PlayerSandbox.Instance.ChessBoardHandler.InvestCoin);
            coinTip.AddCoin(GetCoin());
            
            // CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            // {
            //     bCompleted = true;
            // }, DelayDisappear);
        }

        public override bool OnTick()
        {
            return Time.unscaledTime >= FinishTs;
        }

        #endregion
    }

}