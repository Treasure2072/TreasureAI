using _Scripts.UI.Common;
using Data;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIInventoryLayer : UILayer
    {
        #region Properties

        [Header("References")]
        [SerializeField] private UIInventoryContainer container;

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnClose"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnCloseClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            
            container.RecycleAllGrids(null, true);
            container.SpawnAllGrids();
        }

        #endregion

        #region UILayer

        public static UIInventoryLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIInventoryLayer>("UIInventoryLayer");
            Debug.Assert(layer != null);
            return layer;
        }

        public static void ShowLayer()
        {
            GetLayer()?.Show();
        }

        public static void HideLayer()
        {
            GetLayer()?.Hide();
        }

        #endregion

        #region Callback

        private void OnCloseClick(UIBasicButton sender)
        {
            Hide();
        }

        #endregion
    }
}