using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIChanceLayer : UILayer
    {
        #region Fields
        [Header("Settings")] 
        [SerializeField] private ChanceSettings settings;
        [SerializeField] private ChanceCard goodCard;
        [SerializeField] private ChanceCard badCard;
        
        #endregion

        #region Properties

        private int EventId { get; set; }
        
        private int Coin  { get; set; }
        
        private int Dice { get; set; }
        
        private float Token { get; set; }
        
        private int Step { get; set; }

        #endregion

        #region UILayer

        protected override void OnShow()
        {
            base.OnShow();
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")?.SetElementsVisible(false);
        }

        protected override void OnHide()
        {
            base.OnHide();
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")?.SetElementsVisible(true);
        }

        #endregion

        #region Function

        private void HideCards()
        {
            goodCard.gameObject.SetActive(false);

            badCard.gameObject.SetActive(false);
        }

        private void SetLayer(int id, int coin, int dice, float token, int step)
        {
            HideCards();
            EventId = id;
            Coin = coin;
            Dice = dice;
            Token = token;
            Step = step;
            var chanceEvent = GetChanceEvent(id);
            var card = chanceEvent.chanceType == ChanceType.EChanceType.Good ? goodCard : badCard;


            var description = this.GetLocalizedText(chanceEvent.GetDescriptionKey());
            if (coin < 0)
            {
                description += $"\n{string.Format(this.GetLocalizedText("chance-event-deduct-coin-fmt"), coin)}";
            } else if (coin > 0)
            {
                description += $"\n{string.Format(this.GetLocalizedText("chance-event-gain-coin-fmt"), coin)}";
            }

            if (dice > 0)
            {
                description += $"\n{string.Format(this.GetLocalizedText("chance-event-gain-dice-fmt"), dice)}";
            }

            if (token > 0f)
            {
                description +=$"\n{string.Format(this.GetLocalizedText("chance-event-gain-token-fmt"), token.ToString("0.00"))}";
            }

            if (step > 0)
            {
                description += $"\n{string.Format(this.GetLocalizedText("chance-event-forward-fmt"), step)}";
            }
            
            card.SetDescription(description);
            card.gameObject.SetActive(true);
        }
        
        private ChanceEvent GetChanceEvent(int id)
        {
            return settings.GetChanceEvent(id);
        }

        #endregion

        #region API

        public static UIChanceLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIChanceLayer>("UIChanceLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowLayer(int id, int coin = 0, int dice = 0, float token = 0f, int step = 0)
        {
            if (id != 9) step = 0;
            GetLayer()?.SetLayer(id, coin, dice, token, step);
            GetLayer()?.Show();
        }

        public static void Hidelayer()
        {
            GetLayer()?.Hide();
        }

        public static bool Showing()
        {
            return GetLayer().IsShowing;
        }

        #endregion

        #region Function

        /// <summary>
        /// 角色出现效果播放
        /// </summary>
        private void OnCharacterDisplay()
        {
            var cameraGroup = CameraGroup.GetCameraGroup();
            var chanceEvent = GetChanceEvent(EventId);
            cameraGroup.PlayModel(
                chanceEvent.chanceType == ChanceType.EChanceType.Good 
                ? EffectInstance.Instance.Settings.actionChanceGood
                : EffectInstance.Instance.Settings.actionChanceBad);
        }

        #endregion
    }
}
