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
    public class ChessTileRestaurant : ChessTile
    {
        #region Fields
        
        [FormerlySerializedAs("RibbonsEffectObject")]
        [Header("Effects")] 
        [SerializeField] private GameObject DiceEffectObject;
        
        #endregion

        #region Properties

        private int Dice { get; set; }
        private bool bReceiveArriveMessage { get; set; } = false;

        #endregion

        #region ChessTile
        
        public override List<IQueueableEvent> OnArrive()
        {
            GameSessionAPI.ChessBoardAPI.Arrive();
            World.GetPlayer<GameCharacter>()?.GetCharacterAnimatorInterface().Happy();
            return new List<IQueueableEvent>
            {
                new WaitForTrueEvent(() => bReceiveArriveMessage),
                new CustomEvent(() => { bReceiveArriveMessage = false; }),
                new GameObjectVisibilityEvent(DiceEffectObject),
                new CustomEvent(() =>
                {
                   SoundAPI.PlaySound(AudioInstance.Instance.Settings.goodSmall); 
                }),
                new ModifyNumWSEffectEvent<int>(transform.position, EffectInstance.Instance.Settings.uiEffectDiceNumber, () => Dice),
                new CustomEvent(() =>
                {
                    PlayerSandbox.Instance.CharacterHandler.Dice += Dice;
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
            if(response.GetAttachmentAsString("tile") != "restaurant") return;
            Dice = response.GetAttachmentAsInt("dice");
            bReceiveArriveMessage = true;
        }

        #endregion
    }

}