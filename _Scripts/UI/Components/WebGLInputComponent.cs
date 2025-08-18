using System;
using DragonLi.Core;
using DragonLi.Frame;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Game
{
    public class WebGLInputComponent : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void OpenInputModal();
#endif

        #region Property

        public event Action<string> OnReceiverInput; 

        #endregion
        
        #region Unity

        private void Awake()
        {
            EventDispatcher.AddEventListener<string>(WebGLInputReceiver.WEBGL_INPUT_RECEIVED, OnWebGLInputReceived);
        }

        private void OnDestroy()
        {
            EventDispatcher.RemoveEventListener<string>(WebGLInputReceiver.WEBGL_INPUT_RECEIVED, OnWebGLInputReceived);
        }

        #endregion
        
        #region Function-WebInput
        
        private void OnWebGLInputReceived(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            OnReceiverInput?.Invoke(data);
        }

        public void OpenInput()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
            OpenInputModal();
#endif
        }
        
        #endregion

    }
}