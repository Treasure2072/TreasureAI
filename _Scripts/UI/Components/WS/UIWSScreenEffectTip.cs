using DG.Tweening;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UIWSScreenEffectTip : UIWorldElement
    {
        [Header("Reference")]
        [SerializeField] private RectTransform animatedBackground;
        
        [Header("References - Coin")]
        [SerializeField] private RectTransform animatedRootCoin;
        [SerializeField] private UIAnimatedNumberText animatedNumberCoin;

        [Header("References - Dice")]
        [SerializeField] private RectTransform animatedRootDice;
        [SerializeField] private UIAnimatedNumberText animatedNumberDice;

        [Header("References - Token")]
        [SerializeField] private RectTransform animatedRootToken;
        [SerializeField] private TextMeshProUGUI tmpToken;
        private void PlayCoinAnimation(int number)
        {
            animatedNumberCoin.SetNumberDirectly(0);
            animatedRootCoin.localScale = Vector3.zero;
            animatedRootCoin.DOScale(Vector3.one, 0.35f)
                .SetEase(Ease.OutBounce)
                .SetDelay(0.2f)
                .onComplete = () =>
            {
                animatedNumberCoin.SetNumber(number);
            };
        }

        private void PlayDiceAnimation(int number)
        {
            animatedNumberDice.SetNumberDirectly(0);
            animatedRootDice.localScale = Vector3.zero;
            animatedRootDice.DOScale(Vector3.one, 0.35f)
                .SetEase(Ease.OutBounce)
                .SetDelay(0.2f)
                .onComplete = () =>
            {
                animatedNumberDice.SetNumber(number);
            };
            
        }

        private void PlayTokenAnimation(float number)
        {
            tmpToken.SetText("0");
            animatedRootToken.localScale = Vector3.zero;
            animatedRootToken.DOScale(Vector3.one, 0.35f)
                .SetEase(Ease.OutBounce)
                .SetDelay(0.2f)
                .onComplete = () =>
            {
                tmpToken.SetText(number.ToString("0.0"));
            };
        }

        public void Play(int coin, int dice, float token)
        {
            animatedRootCoin.gameObject.SetActive(coin > 0);
            animatedRootDice.gameObject.SetActive(dice > 0);
            animatedRootToken.gameObject.SetActive(token > 0);

            if (coin > 0)
            {
                PlayCoinAnimation(coin);
            }

            if (dice > 0)
            {
                PlayDiceAnimation(dice);
            }

            if (token > 0)
            {
                PlayTokenAnimation(token);
            }
            
            var originalHeight = animatedBackground.sizeDelta.y;
            animatedBackground.sizeDelta = new Vector2(1, originalHeight);
            animatedBackground.DOSizeDelta(new Vector2(1500, originalHeight), 0.35f).SetEase(Ease.OutCubic);
        }

        public void PlayCoin(int coin)
        {
            Play(coin, 0, 0);
        }

        public void PlayDice(int dice)
        {
            Play(0, dice, 0);
        }
        
        public void PlayToken(float token)
        {
            Play(0, 0, token);
        }
    }
}


