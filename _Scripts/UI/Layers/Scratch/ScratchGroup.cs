using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class ScratchGroup : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] private ScratchElement[] scratchElements;

        [SerializeField] private UIAnimatedNumberText tmpCoin;

        #endregion

        #region Function

        private void SetCoin(int coin)
        {
            tmpCoin.SetNumber(coin);
        }

        #endregion

        #region API
        
        public void Init(int coin)
        {
            tmpCoin.SetNumberDirectly(0);
            foreach (var element in scratchElements)
            {
                element.Inactive();
            }

            SetCoin(coin);
        }
        public void ActiveForNumber(int number)
        {
            for (var i = 0; i < scratchElements.Length; i++)
            {
                if (i < number)
                {
                    scratchElements[i].Active();
                }
            }
        }

        #endregion
    }

}