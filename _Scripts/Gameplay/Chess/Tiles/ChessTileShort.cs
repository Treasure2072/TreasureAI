using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using UnityEngine;

namespace Game
{
    public class ChessTileShort : ChessTile
    {

        #region Properties

        private int Id { get; set; }
        private int Coin { get; set; }

        private bool ReceiveArriveMessage { get; set; } = false;
        
        private float FinishTs { get; set; }

        #endregion
        
        #region ChessTile

        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            var layer = UITutorialLayer.GetLayer("UITutorialLayer-Short");
            return new List<IQueueableEvent>
            {
                new CustomEvent(() =>
                {
                    if (TutorialHandler.IsTriggerable(TutorialHandler.FirstStandShortKey))
                    {
                        UIStaticsLayer.HideUIStaticsLayer();
                        UIActivityLayer.HideUIActivityLayer();
                        UIChessboardLayer.HideLayer();
                        layer.OnHidEvent.AddListener(() =>
                        {
                            TutorialHandler.Triggered(TutorialHandler.FirstStandShortKey);
                            UIStaticsLayer.ShowUIStaticsLayer();
                            UIActivityLayer.ShowUIActivityLayer();
                            UIChessboardLayer.ShowLayer();
                        });
                        layer.Show();
                    }
                }),
                new WaitForTrueEvent(() => !layer.IsShowing),
                new WaitForTrueEvent(() => ReceiveArriveMessage),
                new CustomEvent(() => { ReceiveArriveMessage = false; }),
                new CustomEvent(() =>
                {
                    if (Coin > 0)
                    {
                        SoundAPI.PlaySound(AudioInstance.Instance.Settings.goodSmall);
                    } else if (Coin < 0)
                    {
                        SoundAPI.PlaySound(AudioInstance.Instance.Settings.bad);
                    }
                }),
                new CustomEvent(
                    execute: () =>
                {
                    UIShortLayer.ShowUIShortLayer(Id);
                    var task = new List<IQueueableEvent>();
                    if (Coin > 0)
                    {
                        task.Add(EffectsAPI.CreateTip( () => Coin, () => 0, () => 0));
                        task.Add(EffectsAPI.CreateSoundEffect(() => EffectsAPI.EEffectType.Coin));
                        task.Add(EffectsAPI.CreateScreenFullEffect(() => Coin, () => 0, () => 0));
                        task.Add(new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectCoinNumber, () => Coin));
                    }
                    task.Add(new CustomEvent(() => { PlayerSandbox.Instance.CharacterHandler.Coin += Coin; }));

                    UIShortLayer.GetLayer().OnHideEvents.AddRange(task);
                    if (GameInstance.Instance.HostingHandler.Hosting)
                    {
                        FinishTs = Time.unscaledTime + UIShortLayer.GetLayer().Turn();
                    }
                }, tick: () => Time.unscaledTime >= FinishTs + 2f),
                new WaitForTrueEvent(() => !UIShortLayer.GetLayer().IsShowing),
                new WaitForSecondEvent(1f)
            };
        }

        #endregion
        
        #region Callbacks

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "short") return;

            Id = response.GetAttachmentAsInt("id");
            Coin = response.GetAttachmentAsInt("coin");
            ReceiveArriveMessage = true;
        }

        #endregion
    }
}
