using _Scripts.UI.Common;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class UIOrdersLayer : UILayer
    {
        #region Property

        public OrdersComponent Orders { get; set; }
        public OrdersDetailsComponent OrderDetails { get; set; }

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnClose"].As<UIBasicButton>().OnClickEvent.AddListener(OnCloseClick);
            this["BtnBackToOrder"].As<UIBasicButton>().OnClickEvent.AddListener(OnBackToOrder);
            Orders = this["Orders"].As<OrdersComponent>();
            OrderDetails = this["OrderDetails"].As<OrdersDetailsComponent>();
        }

        protected override void OnShow()
        {
            base.OnShow();
            OpenOrder();
        }

        #endregion

        #region API

        public static UIOrdersLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIOrdersLayer>(nameof(UIOrdersLayer));
            Debug.Assert(layer);
            return layer;
        }

        public void OpenOrder()
        {
            Orders.gameObject.SetActive(true);
            OrderDetails.gameObject.SetActive(false);
        }

        public void OpenOrderDetails()
        {
            Orders.gameObject.SetActive(false);
            OrderDetails.gameObject.SetActive(true);
        }

        #endregion

        #region Callback

        private void OnCloseClick(UIBasicButton sender)
        {
            Hide();
        }

        private void OnBackToOrder(UIBasicButton sender)
        {
            OpenOrder();
        }

        #endregion
    }
}