using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class UIWSBankTip : UIWorldElement
    {
        #region Properties

        [Header("Setting")]
        [SerializeField] private CanvasGroup numberGroup;
        [SerializeField] private CanvasGroup addGroup;
        [SerializeField] private RectTransform addTrans;

        [SerializeField] private float addTextOffsetY = 60;
        [SerializeField] private UIAnimatedNumberText numberText;
        [SerializeField] private UIAnimatedNumberText addText;

        private int Number { get; set; } = 0;
        private WaitForSeconds WaitToAdd { get; } = new(0.7f);
        private WaitForSeconds WaitFinish { get; } = new(0.8f);

        #endregion

        #region API

        public void AddCoin(int number)
        {
            Number += number;
            StartCoroutine(ProcessAddCoin(number));
        }
        
        public void SetCoin(int number)
        {
            Number = number;
            numberText.SetNumberDirectly(number);
        }

        #endregion

        #region Functions

        private IEnumerator ProcessAddCoin(int number)
        {
            addGroup.alpha = 0;
            addTrans.localScale = Vector3.zero;
            addTrans.anchoredPosition = Vector3.up * addTextOffsetY;
            numberGroup.alpha = 0;

            addGroup.DOFade(1, 0.2f);
            addTrans.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic);
            addText.SetNumber(number);

            numberGroup.DOFade(1, 0.2f);
            yield return WaitToAdd;
            addTrans.DOAnchorPos3D(Vector3.zero, 0.5f);
            addGroup.DOFade(0, 0.5f);
            
            numberText.SetNumber(Number);
            yield return WaitFinish;
            numberGroup.transform.DOScale(Vector3.one * 1.5f, 0.5f);
            numberGroup.DOFade(0, 0.5f);
        }

        #endregion
    }
}


