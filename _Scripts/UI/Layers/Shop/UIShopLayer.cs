using System.Collections.Generic;
using _Scripts.Data.Shop;
using _Scripts.UI.Common;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UIShopLayer : UILayer
    {
        #region Properties
        
        [Header("Settings")]
        [SerializeField] private Color unselectedColor;
        [SerializeField] private Color selectedColor;

        #endregion

        #region Unity

        private void Awake()
        {
            PlayerSandbox.Instance.CharacterHandler.PlayerDiceChanged += OnPlayerDiceChangeCallback;
        }

        private void OnDestroy()
        {
            PlayerSandbox.Instance.CharacterHandler.PlayerDiceChanged -= OnPlayerDiceChangeCallback;
        }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnClose"].As<UIBasicButton>().OnClickEvent.AddListener(OnCloseClick);
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessageCallback;
            
            // shopItemContainer.PurchaseAction = OnPurchaseAction;
        }

        protected override void OnShow()
        {
            base.OnShow();
            // shopItemContainer.RecycleAllGrids(null, true);
            // shopItemContainer.SpawnAllGrids();
        }

        #endregion

        #region API

        public static UIShopLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIShopLayer>("UIShopLayer");
            Assert.IsNotNull(layer);
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

        public void OnPurchaseAction(string shopId)
        {
            UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                this.GetLocalizedText("function-not-available"));
            return;
            
            var data = PlayerSandbox.Instance.ChessBoardHandler.Shops.GetShopInfoById(shopId);
            UIPaymentLayer.ShowLayer((long)data.coinPrice, currency:data.price, onConfirm: () =>
            {
                // 创建订单
                GameSessionAPI.PaymentAPI.CreatePayment(shopId,
                    PlayerSandbox.Instance.BlockChainHandler.Account,
                    PlayerSandbox.Instance.RegisterAndLoginHandler.Email,
                    "none", response =>
                    {
                        if (!response.IsSuccess())
                        {
                            UITipLayer.DisplayTip(
                                this.GetLocalizedText("error"),
                                this.GetLocalizedText(response.GetError()),
                                UITipLayer.ETipType.Bad);
                            return;
                        }
                        this.LogEditorOnly(JsonConvert.SerializeObject(response.body));
                        var success = response.GetAttachmentAsInt("success");
                        if (success != 1)
                        {
                            UITipLayer.DisplayTip(
                                this.GetLocalizedText("notice"), 
                                this.GetLocalizedText("purchase-failed")
                                // string.Format(this.GetLocalizedText("purchase-failed-fmt"),
                                    // URLInstance.Instance.URLSettings.dAppUrl)
                                );
                            return;
                        }
                        
                        var amount = response.GetAttachmentAsInt("amount");
                        var id = response.GetAttachmentAsString("payment-id");
                        var rewardToken = response.GetAttachmentAsFloat("token-reward");

                        var task = new List<IQueueableEvent>
                        {
                            new CustomEvent(() =>
                            {
                                UITipLayer.DisplayTip(
                                    this.GetLocalizedText("notice"),
                                    this.GetLocalizedText("purchase-succeed"));
                            }),
                            new WaitForTrueEvent(() => !UITipLayer.GetLayer().IsShowing)
                        };
                        
                        if (rewardToken > 0)
                        {
                            var recharge = PlayerSandbox.Instance.ObjectiveHandler.Recharge;
                            var info = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.GetPaymentLimitByRecharge(recharge);
                            task.Add(new CustomEvent(() =>
                            {
                                UITipLayer.DisplayTip(
                                    this.GetLocalizedText("notice"), 
                                    string.Format(this.GetLocalizedText("objective-payment-gain-token-fmt"), info.sum, rewardToken), 
                                    UITipLayer.ETipType.Good,
                                    () =>
                                    {
                                        PlayerSandbox.Instance.CharacterHandler.Token += rewardToken;
                                    });
                            }));
                        }
                        
                        
                        PlayerSandbox.Instance.ObjectiveHandler.Recharge += amount;
                        if (shopId.StartsWith("dice"))
                        {
                            var info = PlayerSandbox.Instance.ChessBoardHandler.Shops.GetShopInfoById(shopId);
                            PlayerSandbox.Instance.CharacterHandler.Dice += info.count;
                        }
                        
                        EventQueue.Instance.Enqueue(task);
                    });

                // 直接购买
                // GameSessionAPI.CharacterAPI.Purchase(shopId);
            });
        }

        private void OnReceiveMessageCallback(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess())
            {
                this.LogErrorEditorOnly(response.error);
                return;
            }
            
            if (service == GameSessionAPI.CharacterAPI.ServiceName && method == GSCharacterAPI.MethodPurchase)
            {
                var id = response.GetAttachmentAsString("id");
                var count = response.GetAttachmentAsInt("count");

                switch (id)
                {
                    case "dice": PlayerSandbox.Instance.CharacterHandler.Dice += count; break;
                    case "coin": PlayerSandbox.Instance.CharacterHandler.Coin += count; break;
                    default:
                        var tempItems = new Dictionary<string, int>(PlayerSandbox.Instance.CharacterHandler.Items);
                        if (!tempItems.TryAdd(id, count))
                        {
                            tempItems[id] += count;
                        }
                        PlayerSandbox.Instance.CharacterHandler.Items = tempItems;
                        break;
                }
                
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("notice"), 
                    this.GetLocalizedText("purchase-succeed"));
            }
        }

        private void OnPlayerDiceChangeCallback(int? preValue, int newValue)
        {
            if (newValue <= 0)
            {
                // UITipLayer.DisplayTip(
                //     this.GetLocalizedText("notice"), 
                //     this.GetLocalizedText("dice-shot-des"), 
                //     UITipLayer.ETipType.Bad,
                //     ShowLayer);
            }
        }

        #endregion
    }

}