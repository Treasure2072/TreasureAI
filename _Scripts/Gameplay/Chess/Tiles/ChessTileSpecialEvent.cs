using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class ChessTileSpecialEvent : ChessTile
    {
        #region Properties

        private bool HasPrice { get; set; }
        private float Token { get; set; }
        
        private bool ReceiveArriveMessage { get; set; } = false;

        #endregion

        #region ChessTile

        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(() => ReceiveArriveMessage),
                new CustomEvent(() => { ReceiveArriveMessage = false; }),
                new ConditionalEvent(() => HasPrice, () =>
                {
                    return new List<IQueueableEvent>
                    {
                        EffectsAPI.CreateTip(() => 0, () => 0, () => Token),
                        EffectsAPI.CreateSoundEffect(() => EffectsAPI.EEffectType.Token),
                        EffectsAPI.CreateScreenFullEffect(() => 0, () => 0, () => Token),
                        new CustomEvent(() =>
                        {
                            PlayerSandbox.Instance.CharacterHandler.Token += Mathf.Approximately(Token, -1) ? 0 : Token;
                        })
                    };
                }, () =>
                {
                    return new List<IQueueableEvent>
                    {
                        new CustomEvent(() =>
                        {
                            UITipLayer.DisplayTip(
                                this.GetLocalizedText("notice"), 
                                this.GetLocalizedText("special-event-has-no-price"), 
                                UITipLayer.ETipType.Bad);
                        }),
                        new ConditionalEvent(() => GameInstance.Instance.HostingHandler.Hosting, () =>
                        {
                            return new List<IQueueableEvent>
                            {
                                new WaitForSecondEvent(1f),
                                new CustomEvent(() =>
                                {
                                    if (GameInstance.Instance.HostingHandler.Hosting)
                                    {
                                        UITipLayer.GetLayer()?.Hide();
                                    }
                                })
                            };
                        })
                    };
                }),
            };
        }

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            base.OnReceiveMessage(response, service, method);
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "event") return;

            HasPrice = response.GetAttachmentAsBool("has-price");
            Token = response.GetAttachmentAsFloat("token");
            
            ReceiveArriveMessage = true;
        }

        #endregion
    }
}