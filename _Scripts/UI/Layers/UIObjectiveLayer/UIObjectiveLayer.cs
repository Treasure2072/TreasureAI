using System.Collections.Generic;
using _Scripts.UI.Common;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIObjectiveLayer : UILayer
    {
        #region Properties

        [Header("References")]
        [SerializeField] private UIObjectiveItem[] objectives; 
        
        public List<IQueueableEvent> OnHideEvents { get; private set; } = new();

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnClose"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnCloseClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            OnHideEvents.Clear();
            var daily = MissionInstance.Instance.Settings.MissionDaily;
            for (var i = 0; i < daily.Count; i++)
            {
                var id = daily[i].id;
                var objective = objectives[i];
                objective.Initialize(id, null);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            EventQueue.Instance.Enqueue(OnHideEvents);
        }

        #endregion

        #region API

        public static UIObjectiveLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIObjectiveLayer>("UIObjectiveLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowLayer()
        {
            GetLayer()?.Show();
        }

        public static void HideLayer()
        {
            GetLayer()?.Hide();
        }

        #endregion

        #region Callback

        private void OnCloseClick(UIBasicButton sender)
        {
            Hide();
        }

        #endregion
    }
}