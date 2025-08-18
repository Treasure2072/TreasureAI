using System.Collections.Generic;
using Data;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public struct FOrder
    {
        public EPaymentStatus type;
        public Sprite icon;
        public Vector2 sizeDelta;
        public Color color;
    }
    
    public class OrdersComponent : UIComponent
    {
        #region Property

        [Header("Settings")]
        [SerializeField] private FOrder[] orders = new FOrder[4];

        [Header("Reference")]
        [SerializeField] private OrdersContainer container;

        #endregion

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            foreach (Transform order in container.transform)
            {
                Destroy(order.gameObject);
            }
        }

        public override void OnShow()
        {
            base.OnShow();
            RefreshOrders();
        }

        #endregion

        #region Function

        public FOrder GetOrder(EPaymentStatus type)
        {
            foreach (var order in orders)
            {
                if(type == order.type) return order;
            }

            return default;
        }

        private void RefreshOrders()
        {
            container.RecycleAllGrids();
            GameSessionAPI.PaymentAPI.QueryPaymentByUserId(PlayerSandbox.Instance.RegisterAndLoginHandler.Id,
                response =>
                {
                    if (response.IsSuccess())
                    {
                        var paymentsJson = response.GetAttachmentAsString("payments");
                        var payments = JsonConvert.DeserializeObject<List<FPayment>>(paymentsJson);
                        container.SpawnAllGrids(payments);
                    }
                });
        }

        #endregion
    }
}