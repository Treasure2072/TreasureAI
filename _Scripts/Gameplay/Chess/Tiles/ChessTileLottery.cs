using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Network;

namespace Game
{
    public class ChessTileLottery : ChessTile
    {
        #region Proeprty

        private bool ReceiveArriveMessage { get; set; } = false;        

        #endregion

        #region Chesstile

        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(() => ReceiveArriveMessage),
                new CustomEvent(() => { ReceiveArriveMessage = false; }),
                new CustomEvent(() => { UILotteryDrawLayer.GetLayer().Show(); }),
                new WaitForTrueEvent(() => !UILotteryDrawLayer.GetLayer().IsShowing),
                new WaitForSecondEvent(1)
            };
        }

        #endregion

        #region Callback

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "lottery") return;
            ReceiveArriveMessage = true;
        }

        #endregion
    }
}