using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ScratchElement : MonoBehaviour
    {
        #region Fields

        [Header("Settings")] 
        [SerializeField] private Color showColor;
        [SerializeField] private Color hideColor;
        
        [Header("References0")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject border;
        [SerializeField] private Image iconImage;

        #endregion

        #region Unity

        private void Awake()
        {
            Inactive();
        }

        #endregion

        #region API

        public void Active()
        {
            backgroundImage.color = showColor;
            border.SetActive(true);
            border.transform.DOScale(Vector3.one * 1.3f, 0.15f).SetEase(Ease.OutCubic);
            border.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.InCubic).SetDelay(0.15f);
            iconImage.color = Color.white;
        }

        public void Inactive()
        {
            backgroundImage.color = hideColor;
            border.SetActive(false);
            iconImage.color = hideColor;
        }

        #endregion
    }
}
