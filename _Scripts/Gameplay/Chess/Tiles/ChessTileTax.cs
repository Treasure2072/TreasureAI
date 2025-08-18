using System;
using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public class ChessTileTax : ChessTile
    {

        #region Properties

        private int Coin { get; set; }
        private bool bReceiveArriveMessage { get; set; } = false;

        #endregion
        
        #region ChessTile

        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            World.GetPlayer<GameCharacter>()?.GetCharacterAnimatorInterface().Sad();
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(() => bReceiveArriveMessage),
                new CustomEvent(() => { bReceiveArriveMessage = false; }),
                new CustomEvent(() =>
                {
                    var tasks = new List<IQueueableEvent>
                    {
                        new CustomEvent(() =>
                        {
                           SoundAPI.PlaySound(AudioInstance.Instance.Settings.bad); 
                        }),
                        new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectMinusCoinNumber, () => -Math.Abs(Coin)),
                        new CustomEvent(() => PlayerSandbox.Instance.CharacterHandler.Coin -= Math.Abs(Coin) )
                    };
                    UITipLayer.DisplayTip(
                        this.GetLocalizedText("notice"), 
                        string.Format(this.GetLocalizedText("pay-tax-fmt"), Coin), 
                        UITipLayer.ETipType.Bad);
                    UITipLayer.GetLayer()?.OnHideEvents.AddRange(tasks);
                    if (GameInstance.Instance.HostingHandler.Hosting)
                    {
                        UITipLayer.GetLayer()?.Hide();
                    }
                }),
            };
        }


        #endregion
        
        #region Callbacks

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "tax") return;
            Coin = response.GetAttachmentAsInt("coin");
            bReceiveArriveMessage = true;
        }

        #endregion
    }

}