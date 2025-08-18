using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class ChessTileCompany : ChessTile
    {
        #region Fields

        [Header("References")] 
        [SerializeField] private GameObject coinEffectObject;
        [SerializeField] private GameObject diamondEffectObject;

        #endregion

        #region Prperties

        private bool ReceiveArriveMessage { get; set; }
        
        private int Coin  { get; set; }
        
        private float Token  { get; set; }

        #endregion

        #region ChessTile

        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            World.GetPlayer<GameCharacter>()?.GetCharacterAnimatorInterface().Happy();
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(() => ReceiveArriveMessage),
                new CustomEvent(() => { ReceiveArriveMessage = false; }),
                new GameObjectVisibilityEvent(coinEffectObject),
                new GameObjectVisibilityEvent(diamondEffectObject),
                new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectCoinNumber, () => Coin),
                new WaitForSecondEvent(1),
                new ModifyNumWSEffectEvent<float>(transform.position, EffectInstance.Instance.Settings.uiEffectTokenNumber, () => Token),
                new CustomEvent(() =>
                {
                    PlayerSandbox.Instance.CharacterHandler.Coin += Coin;
                    PlayerSandbox.Instance.CharacterHandler.Token += Mathf.Approximately(Token, -1) ? 0 : Token;
                }),
            };
        }

        #endregion

        #region Callback

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            base.OnReceiveMessage(response, service, method);
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "company") return;
            ReceiveArriveMessage = true;
            var coin = response.GetAttachmentAsInt("coin");
            var token = response.GetAttachmentAsFloat("token");
            Coin = coin == -1 ? 0 : coin;
            Token = Mathf.Approximately(token, -1) ? 0 : token;
        }
        #endregion

    }

}