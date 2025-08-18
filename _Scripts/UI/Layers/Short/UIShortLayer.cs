using System.Collections;
using System.Collections.Generic;
using _Scripts.UI.Common;
using Data;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIShortLayer : UILayer
    {
        #region Fields

        [Header("Settings")] 
        [SerializeField] private int defaultRound = 5;
        [SerializeField] private float intervalUnit = 0.2f;

        [Header("References")] 
        [SerializeField] private TextMeshProUGUI tmpDescription;
        [SerializeField] private Transform center;
        [SerializeField] private ShortElement[] shortElements;

        #endregion

        #region Properties
        
        public List<IQueueableEvent> OnHideEvents { get; private set; } = new();
        
        private bool Finished { get; set; }
        private int Result { get; set; }
        
        private float LifeTime { get; set; }

        #endregion

        #region UILayer
        
        protected override void OnInit()
        {
            base.OnInit();
            this["BtnGo"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnPressGo);
            
            for(var i = 0; i < shortElements.Length; i++)
            {
                shortElements[i].SetRate(PlayerSandbox.Instance.ChessBoardHandler.Shorts[i].chance);
            }
        }

        protected override void OnShow()
        {
            base.OnShow();
            OnHideEvents.Clear();
            center.eulerAngles = Vector3.zero;
            UIStaticsLayer.HideUIStaticsLayer();
            UIActivityLayer.HideUIActivityLayer();
            UIChessboardLayer.HideLayer();
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")?.SetElementsVisible(false);
        }

        protected override void OnHide()
        {
            base.OnHide();
            tmpDescription.gameObject.SetActive(false);
            EventQueue.Instance.Enqueue(OnHideEvents);
            UIStaticsLayer.ShowUIStaticsLayer();
            UIActivityLayer.ShowUIActivityLayer();
            UIChessboardLayer.ShowLayer();
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")?.SetElementsVisible(true);
        }

        #endregion

        #region Functions

        private void ShowLayer(int result)
        {
            Finished = false;
            SetResult(result);
            tmpDescription.gameObject.SetActive(false);
            Show();
        }

        private void SetResult(int result)
        {
            Result = result;
            LifeTime = (result + defaultRound * 8) * intervalUnit + 3f;
        }

        private void ProcessTurn(int result)
        {
            var unitAngle = 360f / shortElements.Length;
            var endAngle = unitAngle / 2 + (result + defaultRound * 8) * unitAngle;
            var tween = center
                .DORotate(new Vector3(0, 0, endAngle), (result + defaultRound * 8) * intervalUnit, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic);
            tween.onComplete = () =>
            {
                CoroutineTaskManager.Instance.WaitSecondTodo(Hide, 2f);
            };
        }

        #endregion

        #region API

        public static UIShortLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIShortLayer>("UIShortLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowUIShortLayer(int result)
        {
            GetLayer()?.ShowLayer(result);
        }

        public float Turn()
        {
            OnPressGo(this["BtnGo"].As<UIBasicButton>());
            return LifeTime;
        }

        #endregion

        #region Callabck

        private void OnPressGo(UIBasicButton sender)
        {
            if(Finished) return;
            Finished = true;
            tmpDescription.gameObject.SetActive(true);
            ProcessTurn(Result);
        }

        #endregion
    }

}