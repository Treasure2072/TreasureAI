using Data;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UIAnnLayer : UILayer
    {
        [SerializeField] private TextMeshProUGUI tmpContent;
        [SerializeField] private GameObject fingerClose;
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            
        }

        protected override void OnShow()
        {
            base.OnShow();
            ActiveCloseFinger(TutorialHandler.IsTriggerable(TutorialHandler.FirstEnterKey));
            SetContent();
        }

        #endregion
        
        #region API

        public static UIAnnLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIAnnLayer>("UIAnnLayer");
            Debug.Assert(layer);
            return layer;
        }

        public void ActiveCloseFinger(bool active = false)
        {
            fingerClose.SetActive(active);
        }
        
        #endregion

        #region Function

        private void SetContent()
        {
            var content = this.GetLocalizedText("ann-welcome") + '\n' + '\n'
                + string.Format(this.GetLocalizedText("ann-website-fmt"), URLInstance.Instance.URLSettings.websiteURL) + '\n' + '\n'
                + this.GetLocalizedText("ann-start-game");
            
            tmpContent.SetText(content);
        }

        #endregion
    }
}