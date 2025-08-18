using _Scripts.UI.Common.Grids;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIInventoryItem : GridBase
    {
        #region Properties

        [Header("References")]
        [SerializeField] private Image imgIcon;
        [SerializeField] private TextMeshProUGUI txtNum;

        #endregion

        #region Function

        public void SetIcon(Sprite sprite)
        {
            // TODO: 设置物品图片
            // ...
            // imgIcon.sprite = sprite;
        }

        public void SetNum(int num)
        {
            txtNum.text = num.ToString();
        }

        #endregion
    }

}