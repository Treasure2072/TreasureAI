using System;
using System.Collections;
using Data;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Game
{
    public class StartScene : MonoBehaviour
    {
        #region Unity

        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Application.targetFrameRate = 45;
#endif
            Settings.LoadSettings();
        }

        private IEnumerator Start()
        {
            
            yield return null;
            UIManager.Instance.GetLayer("UIBlackScreen").Hide();
            
#if UNITY_IOS || UNITY_ANDROID
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
        }


        #endregion
        
    }
}


