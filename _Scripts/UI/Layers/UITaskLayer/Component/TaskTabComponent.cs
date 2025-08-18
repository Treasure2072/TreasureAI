using System;
using _Scripts.UI.Common;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class TaskTabComponent : UIComponent
    {
        #region Property

        [SerializeField] private Image daily;
        [SerializeField] private Image main;
        

        public event Action OnOpenDaily;
        
        public event Action OnCloseDaily;

        public event Action OnOpenMain;
        
        public event Action OnCloseMain;

        #endregion

        #region API

        public void OnDailyClick(UIBasicButton sender)
        {
            daily.color = Color.white;
            main.color = Color.clear;
            
            OnCloseMain?.Invoke();
            OnOpenDaily?.Invoke();
        }

        public void OnMainClick(UIBasicButton sender)
        {
            daily.color = Color.clear;
            main.color = Color.white;
            
            OnCloseDaily?.Invoke();
            OnOpenMain?.Invoke();
        }

        #endregion
    }
}