using _Scripts.Data.Shop;
using _Scripts.UI.Common.Grids;
using Data;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class UIShopItem : GridBase
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Image imgIcon;
        [SerializeField] private TextMeshProUGUI tmpName;
        [SerializeField] private TextMeshProUGUI tmpNumber;
        [SerializeField] private TextMeshProUGUI tmpPrice;
        [SerializeField] private TextMeshProUGUI tmpOperated;
        [SerializeField] private Button operatedButton;

        #endregion

        #region Properties - Data

        private FShopInfo Data { get; set; }
        private UnityAction<string> BuyAction { get; set; }

        #endregion

        #region GridBase

        protected override void OnInitialized()
        {
            base.OnInitialized();
            operatedButton.onClick.RemoveAllListeners();
            operatedButton.onClick.AddListener(() => BuyAction.Invoke(Data.id));
        }

        public override void SetGrid<T>(params object[] args)
        {
            base.SetGrid<T>(args);
            
            Data = (FShopInfo)args[0];
            BuyAction = (UnityAction<string>)args[1];
            
            // TODO: 设置 id 相关数据
            // ...
            SetName(this.GetLocalizedText(Data.id));
            SetNumber(Data.count);
            SetPrice(Data.price);
        }

        #endregion

        #region Function

        private void SetName(string gridName)
        {
            tmpName.text = gridName;
        }

        private void SetNumber(long gridNumber)
        {
            tmpNumber.text = NumberUtils.GetDisplayNumberStringAsCurrency(gridNumber);
        }

        private void SetPrice(decimal gridPrice)
        {
            tmpPrice.text = $"${gridPrice}";
        }

        private void SetOperated(string buttonName)
        {
            tmpOperated.text = buttonName;
        }
        
        #endregion
    }

}