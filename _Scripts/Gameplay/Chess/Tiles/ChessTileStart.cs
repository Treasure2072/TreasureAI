using System;
using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class ChessTileStart : ChessTile
    {
        #region Fields

        [Header("Effects")] 
        [SerializeField] private GameObject Cash;
        [SerializeField] private GameObject CashMajor;

        private int PassReward { get; set; }
        private int ArriveReward { get; set; }

        private bool bReceivePassMessage { get; set; } = false;
        private bool bReceiveArriveMessage { get; set; } = false;

        #endregion

        #region ChessTile

        public override void Initialize(int tileIndex)
        {
            base.Initialize(tileIndex);
            Cash.SetActive(false);
            Cash.SetActive(false);
        }

        public override List<IQueueableEvent> OnPass()
        {
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(() => bReceivePassMessage),
                new CustomEvent(() => { bReceivePassMessage = false; }),
                new GameObjectVisibilityEvent(Cash),
                new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectCoinNumber, () => PassReward),
                new CustomEvent(() => PlayerSandbox.Instance.CharacterHandler.Coin += PassReward ),
            };
        }

        public override List<IQueueableEvent> OnArrive()
        {
            bReceiveArriveMessage = false;
            GameSessionAPI.ChessBoardAPI.Arrive();
            World.GetPlayer<GameCharacter>()?.GetCharacterAnimatorInterface().Happy();
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(() => bReceiveArriveMessage),
                new CustomEvent(() => { bReceiveArriveMessage = false; }),
                new GameObjectVisibilityEvent(CashMajor),
                new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectCoinNumber, () => ArriveReward),
                new CustomEvent(() => PlayerSandbox.Instance.CharacterHandler.Coin += ArriveReward ),
            };
        }

        #endregion

        #region Callbacks

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName) return;
            if(response.GetAttachmentAsString("tile") != "start") return;
            
            OnPassReceiveMessage(response, service, method);
            OnArriveReceiveMessage(response, service, method);
        }

        #endregion

        private void OnPassReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (method != "pass") return;
            bReceivePassMessage = true;
            PassReward = response.GetAttachmentAsInt("coin");
        }
        
        private void OnArriveReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (method != GSChessBoardAPI.MethodArrive) return;
            bReceiveArriveMessage = true;
            ArriveReward = response.GetAttachmentAsInt("coin");
        }
    }

}