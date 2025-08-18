using System;
using _Scripts.UI.Common;
using _Scripts.Utils;
using Data;
using DG.Tweening;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UISignInLayer : UILayer
    {
        #region Proeprty

        [SerializeField] private UITimer timerRemain;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private SignCell[] signCells; 
        
        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnSign"].As<UIBasicButton>().OnClickEvent.AddListener(OnSignClick);
            this["BtnClose"].As<UIBasicButton>().OnClickEvent.AddListener(OnCloseClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            UIChessboardLayer.HideLayer();
            UIActivityLayer.HideUIActivityLayer();
            UIStaticsLayer.ShowUIStaticsLayer();

            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")?.SetElementsVisible(false);
            for (var i = 0; i < signCells.Length; i++)
            {
                signCells[i].Init(i);
            }
            
            // timerRemain.Initialize(PlayerSandbox.Instance.ObjectiveHandler.GetDailyFinishTimeStamp() - TimeAPI.GetUtcTimeStamp());
        }

        protected override void OnHide()
        {
            base.OnHide();
            UIChessboardLayer.ShowLayer();
            UIActivityLayer.ShowUIActivityLayer();
            UIStaticsLayer.ShowUIStaticsLayer();
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")?.SetElementsVisible(true);
        }

        #endregion

        #region Function

        private void SetRemainTime()
        {
            
        }

        #endregion
        
        #region API

        public static UISignInLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UISignInLayer>("UISignInLayer");
            Debug.Assert(layer);
            return layer;
        }

        public float CanvasGroupAlphaAnim(float endVal, float duration)
        {
            canvasGroup.DOFade(endVal, duration)
                .SetEase(Ease.Linear);
            return duration;
        }

        #endregion

        #region Callback

        private void OnSignClick(UIBasicButton sender)
        {
            // GameSessionAPI.ObjectiveAPI.CompleteCheckIn();
        }

        private void OnCloseClick(UIBasicButton sender)
        {
            Hide();
        }

        #endregion
    }
}