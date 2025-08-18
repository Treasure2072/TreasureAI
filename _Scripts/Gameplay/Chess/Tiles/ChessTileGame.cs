using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class ChessTileGame : ChessTile
    {
        #region Fields
        
        [Header("References")]
        [SerializeField] private GameObject RibbonsMajorEffectObject;

        #endregion
        
        #region Properties

        private bool ReceiveArriveMessage { get; set; } = false;

        #endregion

        #region ChessTile

        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("match-three", 1);
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(() => ReceiveArriveMessage),
                new CustomEvent(() => { ReceiveArriveMessage = false; }),
                new CustomEvent(() =>
                {
                    UIStaticsLayer.HideUIStaticsLayer();
                    UIActivityLayer.HideUIActivityLayer();
                    UIChessboardLayer.HideLayer();
                }),
                new GameObjectVisibilityEvent(RibbonsMajorEffectObject),
                new CustomEvent(() =>
                {
                    SoundAPI.PlaySound(AudioInstance.Instance.Settings.goodBig);
                }) ,
                new WaitForSecondEvent(1),
                new CustomEvent(() =>
                {
                    UIManager.Instance.GetLayer("UIBlackScreen").Show();
                    SceneManager.Instance.AddSceneToLoadQueueByName("MatchScene", 1);
                    SceneManager.Instance.StartLoad();
                })
            };
        }


        public override void PlayArriveAnimation(EArriveAnimationType animationType, int playerTileIndex)
        {
            base.PlayArriveAnimation(animationType, playerTileIndex);
            World.GetPlayer<GameCharacter>()?.GetCharacterAnimatorInterface().Idle();
        }

        protected override void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            base.OnReceiveMessage(response, service, method);
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex != TileIndex) return;
            if (!response.IsSuccess()) return;
            if (service != GameSessionAPI.ChessBoardAPI.ServiceName || method != GSChessBoardAPI.MethodArrive) return;
            if(response.GetAttachmentAsString("tile") != "game") return;

            GameInstance.Instance.IsChessBoardScene = false;
            ReceiveArriveMessage = true;
        }

        #endregion
    }

}