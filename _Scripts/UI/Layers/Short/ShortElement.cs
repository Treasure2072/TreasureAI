using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ShortElement : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Image elementIcon;
        [SerializeField] private TextMeshProUGUI elementName;
        [SerializeField] private Image rewardIcon;

        #endregion

        #region API

        public void SetUp(Sprite icon, string rewardName, Sprite reward)
        {
            elementIcon.sprite = icon;
            elementName.text = rewardName;
            rewardIcon.sprite = reward;
        }

        public void SetRate(float rate)
        {
            elementName.text = $"{rate * 100} %";
        }

        #endregion
    }

}