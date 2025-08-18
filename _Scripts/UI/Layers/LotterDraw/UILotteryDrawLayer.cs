using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.UI.Common;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public enum ELotteryDrawType
    {
        Blue,
        Green,
        Yellow,
    }
    
    [System.Serializable]
    public struct FLotteryDraw
    {
        public ELotteryDrawType type;
        public GameObject prefab;
    }
    
    public class UILotteryDrawLayer : UILayer
    {
        #region Property

        [Header("Setting")]
        [SerializeField] private float itemHigh = 150f;
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private int defaultNum = 20;
        [SerializeField] private int tolerance = 5;
        [SerializeField] private Ease ease = Ease.Linear;
        [SerializeField] private FLotteryDraw[] lotteryDraws = new FLotteryDraw[3];
        
        [Header("Reference - Countdown")]
        [SerializeField] private CountdownComponent countdownComponent;
        [SerializeField] private TextMeshProUGUI tmpCountdown;
        
        [Header("Reference")]
        [SerializeField] private LotteryDrawContainer[] containers = new LotteryDrawContainer[5];
        
        private Coroutine Pull { get; set; }
        
        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnStart"].As<UIBasicButton>().OnClickEvent.AddListener(OnStartClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            if (countdownComponent != null)
            {
                countdownComponent.OnValueChanged += OnCountdownChanged;
                countdownComponent.OnEnd += OnCountdownEnd;
            }
            UIActivityLayer.HideUIActivityLayer();
            UIChessboardLayer.HideLayer();

            if (Pull != null)
            {
                StopCoroutine(Pull);
                Pull = null;
            }
            
            var res = GetNonUniformList(containers.Length);
            for (var i = 0; i < containers.Length; i++)
            {
                var types = GetRandomLotteryDraws(defaultNum + i * tolerance);
                types[1] = res[i];
                containers[i].RecycleAllGrids(null, true);
                containers[i].SpawnAllGrids(types);
            }
            
            foreach (var container in containers)
            {
                SetContainerY(container, itemHigh * (container.GetElementsCount() - 3));
            }
            
            if (GameInstance.Instance.HostingHandler.Hosting)
            {
                CoroutineTaskManager.Instance.WaitSecondTodo(() =>
                {
                    OnStartClick(this["BtnStart"].As<UIBasicButton>());
                }, 2f);
            }
            else
            {
                // countdownComponent.StartCountdown();
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            if (countdownComponent != null)
            {
                countdownComponent.OnValueChanged -= OnCountdownChanged;
                countdownComponent.OnEnd -= OnCountdownEnd;
            }
            UIActivityLayer.ShowUIActivityLayer();
            UIChessboardLayer.ShowLayer();
            UIStaticsLayer.ShowUIStaticsLayer();
        }

        #endregion

        #region API

        public static UILotteryDrawLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UILotteryDrawLayer>(nameof(UILotteryDrawLayer));
            Debug.Assert(layer);
            return layer;
        } 

        public FLotteryDraw GetLotteryDraw(ELotteryDrawType type)
        {
            foreach (var e in lotteryDraws)
            {
                if(e.type == type) return e;
            }

            return default;
        }

        #endregion

        #region Function

        private List<ELotteryDrawType> GetRandomLotteryDraws(int length)
        {
            var draws = new List<ELotteryDrawType>();
            for (var i = 0; i < length; i++)
            {
                var values = Enum.GetValues(typeof(ELotteryDrawType));
                var rang = UnityEngine.Random.Range(0, values.Length);
                var type = (ELotteryDrawType)rang;
                draws.Add(type);
            }
            return draws;
        }
        
        public static List<ELotteryDrawType> GetNonUniformList(int length)
        {
            var values = Enum.GetValues(typeof(ELotteryDrawType));
            List<ELotteryDrawType> result;
            do
            {
                result = new List<ELotteryDrawType>(length);
                for (var i = 0; i < length; i++)
                {
                    var rang = UnityEngine.Random.Range(0, values.Length);
                    var value = (ELotteryDrawType)rang;
                    result.Add(value);
                }
            }
            while (result.All(x => x == result[0]));

            return result;
        }

        private void SetContainerY(LotteryDrawContainer container, float moveY)
        {
            var rect = container.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, moveY);
        }

        private float ContainerTweenMoveY(LotteryDrawContainer container)
        {
            var moveDistanceY = itemHigh;
            var during = duration * (container.GetElementsCount() - 3);
            
            var rect = container.GetComponent<RectTransform>();
            rect.DOAnchorPosY(0, during).SetEase(ease);
            return during;
        }

        private IEnumerator StartPull()
        {
            var time = 0f;
            foreach (var container in containers)
            {
                var tempTime = ContainerTweenMoveY(container);
                time = System.Math.Max(time, tempTime);
            }

            yield return new WaitForSeconds(time + 1f);
            foreach (var container in containers)
            {
                var grid = container.GetGrid(1);
                grid.gameObject.GetComponent<BreathEffect>().SetBreath(true);
            }

            yield return CoroutineTaskManager.Waits.TwoSeconds;
            yield return CoroutineTaskManager.Waits.OneSecond;

            if (GameInstance.Instance.HostingHandler.Hosting)
            {
                Hide();
            }
            else
            {
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("notice"), 
                    this.GetLocalizedText("no-gains-reward"), 
                    UITipLayer.ETipType.Bad, 
                    Hide);
            }
        }

        #endregion

        #region Callback

        private void OnCountdownChanged(float value)
        {
            tmpCountdown.SetText(value.ToString("0"));
        }

        private void OnCountdownEnd()
        {
            OnStartClick(this["BtnStart"].As<UIBasicButton>());
        }

        private void OnStartClick(UIBasicButton sender)
        {
            if (Pull != null) return;
            
            Pull = StartCoroutine(StartPull());
            // this["LotteryLight"].As<LotteryLight>()?.Acc();
        }

        #endregion
    }
}