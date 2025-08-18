using System.Collections.Generic;
using System.Linq;
using _Scripts.UI.Common;
using _Scripts.Utils;
using Data;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UITaskLayer : UILayer
    {
        #region Property

        [SerializeField] private GameObject dailyContent;
        [SerializeField] private GameObject mainContent;
        
        [Header("Daily")]
        [SerializeField] private Transform root;

        [SerializeField] private GameObject rechargePrefab;
        [SerializeField] private GameObject dicePrefab;
        [SerializeField] private GameObject coinPrefab;
        
        [Header("Reference - layout")]
        [SerializeField] private Transform contentTrans;
        [SerializeField] private VerticalLayoutGroup layoutGroup;
        [SerializeField] private ContentSizeFitter contentSizeFitter;
        
        [Header("Settings")]
        [SerializeField] private UITimer timerRemain;
        
        private TaskTabComponent Tab { get; set; }
        
        private TaskMainComponent Main { get; set; }
        
        public List<IQueueableEvent> OnHideEvents { get; private set; } = new();

        #endregion

        #region Property - Daily

        private List<IObjective> Objectives { get; set; } = new();

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnClose"].As<UIBasicButton>().OnClickEvent.AddListener(OnCloseClick);

            Tab = this["Tab"].As<TaskTabComponent>();
            this["BtnDaily"].As<UIBasicButton>().OnClickEvent.AddListener(Tab.OnDailyClick);
            this["BtnMain"].As<UIBasicButton>().OnClickEvent.AddListener(Tab.OnMainClick);
            
            Main = this["Main"].As<TaskMainComponent>();
            this["BtnX"].As<UIBasicButton>().OnClickEvent.AddListener(Main.OnTwitterClick);
            // this["BtnDiscard"].As<UIBasicButton>().OnClickEvent.AddListener(Main.OnDiscordClick);
            this["BtnTelegram"].As<UIBasicButton>().OnClickEvent.AddListener(Main.OnTelegramClick);
            this["BtnYoutube"].As<UIBasicButton>().OnClickEvent.AddListener(Main.OnYoutubeClick);
        }

        protected override void OnShow()
        {
            base.OnShow();

            Tab.OnOpenDaily += OnOpenDaily;
            Tab.OnCloseDaily += OnCloseDaily;
            Tab.OnOpenMain += OnOpenMain;
            Tab.OnCloseMain += OnCloseMain;
            
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")
                ?.SetElementsVisible(false);
            
            UIChessboardLayer.HideLayer();
            UIActivityLayer.HideUIActivityLayer();
            UIStaticsLayer.ShowUIStaticsLayer();
            UIManager.Instance.GetLayer("UIBlackScreen").Hide();
            
            Tab.OnDailyClick(this["BtnDaily"].As<UIBasicButton>());
            
            OnHideEvents.Clear();
            RefreshContent();
            
            timerRemain.Initialize(PlayerSandbox.Instance.ObjectiveHandler.GetDailyFinishTimeStamp() - TimeAPI.GetUtcTimeStamp());
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            Tab.OnOpenDaily -= OnOpenDaily;
            Tab.OnCloseDaily -= OnCloseDaily;
            Tab.OnOpenMain -= OnOpenMain;
            Tab.OnCloseMain -= OnCloseMain;
            
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")
                ?.SetElementsVisible(true);
            
            UIActivityLayer.HideUIActivityLayer();
            
            UIChessboardLayer.ShowLayer();
            UIActivityLayer.ShowUIActivityLayer();
            UIStaticsLayer.ShowUIStaticsLayer();
            EventQueue.Instance.Enqueue(OnHideEvents);
        }

        #endregion

        #region Function

        private List<string> GetSequence()
        {
            var missions = PlayerSandbox.Instance.ObjectiveHandler.Daily.progress;
            var res = new List<string>(10);
            
            // 可以被领取的
            foreach (var (id, pr) in missions)
            {
                if (PlayerSandbox.Instance.ObjectiveHandler.Daily.CanClaim(id))
                {
                    res.Add(id);
                }
            }

            // 未完成，正在进行的
            foreach (var (id, pr) in missions)
            {
                if (!PlayerSandbox.Instance.ObjectiveHandler.Daily.IsCompletedById(id))
                {
                    res.Add(id);
                }
            }

            // 已经被领取的
            foreach (var (id, pr) in missions)
            {
                if (PlayerSandbox.Instance.ObjectiveHandler.Daily.IsCompletedById(id) &&
                    PlayerSandbox.Instance.ObjectiveHandler.Daily.IsCollectedById(id))
                {
                    res.Add(id);
                }
            }
            return res;
        }

        private IObjective SpawnMission(GameObject missionPrefab)
        {
            var obj = Instantiate(missionPrefab, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(root, false);
            obj.transform.localScale = Vector3.one;
                
            return obj.GetComponent<IObjective>();
        }

        private void RefreshContent()
        {
            foreach (Transform child in root)
            {
                Destroy(child.gameObject);
                Objectives.Clear();
            }

            if (PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex <
                PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count)
            {
                //充值任务
                var chargeComp = SpawnMission(rechargePrefab) as UIObjectiveRechargeItem;
                chargeComp?.Initialize(OnCollectAction);
                Objectives.Add(chargeComp);
            }
            
            // 日常任务
            foreach (var mission in GetSequence())
            {
                GameObject missionPrefab = null;
                if (mission.StartsWith("dice"))
                {
                    missionPrefab = dicePrefab;
                }

                if (mission.StartsWith("coin"))
                {
                    missionPrefab = coinPrefab;
                }

                var comp = SpawnMission(missionPrefab) as UIObjectiveItem;
                comp?.Initialize(mission, OnCollectAction);
                Objectives.Add(comp);
            }

            if (PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex >=
                PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count)
            {
                //充值任务
                var chargeComp = SpawnMission(rechargePrefab) as UIObjectiveRechargeItem;
                chargeComp?.Initialize(OnCollectAction);
                Objectives.Add(chargeComp);
            }
        }

        #endregion

        #region Function - Animation
        
        private int GetIndexOfObjective(UIObjectiveItem item)
        {
            return Objectives.IndexOf(item);
        }

        private void OnCollectAction(IObjective item)
        {
            // 暂停布局系统，防止干扰动画
            layoutGroup.enabled = false;
            contentSizeFitter.enabled = false;
            
            // 获取最终位置
            var endPos = Objectives[^1].GetTransform().localPosition;

            var trans = item.GetTransform();
            var startPos = trans.localPosition;

            // 把 item 移动到底部（注意：先不刷新布局）
            trans.SetSiblingIndex(Objectives.Count - 1);

            // 强制刷新布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            
            // 回到初始位置以便播放动画
            trans.localPosition = startPos;

            this.LogEditorOnly($"StartPos = {startPos}  EndPos = {endPos}");
            var duration = Objectives.Count * 0.1f;
            // 播放动画
            trans.DOLocalMove(endPos, duration).SetEase(Ease.InOutCubic).SetEase(Ease.InOutCubic).OnComplete(() =>
            {
                // 动画结束后恢复布局系统
                layoutGroup.enabled = true;
                contentSizeFitter.enabled = true;

                // 强制更新一次，确保一切对齐
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            });

            // var space = layoutGroup.spacing + 188f;
            // if (Objectives.Count < 5) return;
            // contentTrans.DOLocalMoveY((Objectives.Count - 5) * space, duration);
        }

        #endregion

        #region API

        public static UITaskLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UITaskLayer>("UITaskLayer");
            Debug.Assert(layer);
            return layer;
        }

        #endregion

        #region Callback

        private void OnCloseClick(UIBasicButton sender)
        {
            Hide();
        }

        private void OnOpenDaily()
        {
            dailyContent?.SetActive(true);
        }

        private void OnCloseDaily()
        {
            dailyContent?.SetActive(false);
        }

        private void OnOpenMain()
        {
            mainContent?.SetActive(true);
        }

        private void OnCloseMain()
        {
            mainContent?.SetActive(false);
        }
        
        #endregion
    }
}