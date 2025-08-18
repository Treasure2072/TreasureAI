using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game
{
    public class UIPrisonNormalLayer : UILayer
    {
        #region Fields

        [Header("Settings")] 
        [SerializeField] private Color Incomplete;

        [SerializeField] private Color Full;

        [Header("References")]
        [SerializeField] private Image IMG_TMPBackground;
        [SerializeField] private UIAnimatedNumberText TMP_Point;

        #endregion

        #region Unity

#if UNITY_EDITOR
        private void Reset()
        {
            ColorUtility.TryParseHtmlString("#FF8D8D", out Incomplete);
            ColorUtility.TryParseHtmlString("#C4FF8F", out Full);
        }
#endif

        #endregion
        
        #region UILayer

        protected override void OnShow()
        {
            base.OnShow();
            TMP_Point.SetNumberDirectly(0);
        }

        #endregion

        #region API

        public static UIPrisonNormalLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIPrisonNormalLayer>("UIPrisonNormalLayer");
            Assert.IsNotNull(layer);
            return layer;
        }

        public void SetTMPRollPoints(int points)
        {
            TMP_Point.SetNumber(points);
            IMG_TMPBackground.color = points < 24 ? Incomplete : Full;
        }
        
        #endregion
    }

}