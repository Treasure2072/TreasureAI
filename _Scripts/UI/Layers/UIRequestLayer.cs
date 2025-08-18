using System;
using System.Collections;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class UIRequestLayer: UILayer
    {
        #region Fields

        [Header("References")]
        [SerializeField]
        private GameObject processNode;
        
        [Header("Settings")]
        [SerializeField]
        private float fadeTime = 0.5f;

        #endregion

        #region Properties

        private bool IsRequesting { get; set; }
        private bool HideOnFinish { get; set; }
        private WaitForSeconds WaitFadeTime { get; set; }
        private Coroutine DelayDisplayProcessCoroutine { get; set; }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            HideOnFinish = true;
            WaitFadeTime = new WaitForSeconds(fadeTime);
            processNode?.SetActive(false);
        }

        protected override void OnShow()
        {
            base.OnShow();
            if (DelayDisplayProcessCoroutine == null)
            {
                StopCoroutine(DelayDisplayProcessCoroutine);
                DelayDisplayProcessCoroutine = null;
            }
            
            StartCoroutine(DelayDisplayProcess(fadeTime));
        }

        #endregion

        #region Functions

        private IEnumerator DelayDisplayProcess(float delay)
        {
            yield return WaitFadeTime;
            processNode?.SetActive(true);
        }
        
        private bool SetRequesterAndSend<T>(HttpRequest<T> requester, Action<T> onResponse, bool autoHide) where T : IResponseProtocol
        {
            HideOnFinish = autoHide;
            IsRequesting = true;
            requester.AddCallback(onResponse);
            requester.AddCallback(OnRequestFinish);
            return requester.SendRequestAsync();
        }

        private void SetHideOnFinish(bool autoHide)
        {
            HideOnFinish = autoHide;
        }

        private void OnRequestFinish<T>(T response) where T: IResponseProtocol
        {
            IsRequesting = false;
            
            // TODO: Do global error handle, such as login failed, etc.
            // ...
            
            // 隐藏此页面
            // ...
            if (HideOnFinish)
            {
                Hide();
            }
        }

        #endregion
        
        #region API

        /// <summary>
        /// 发起请求
        /// </summary>
        /// <param name="requester">请求体</param>
        /// <param name="onResponse">结果回调</param>
        /// <param name="autoHide">成功后页面是否自动隐藏</param>
        /// <typeparam name="T">返回结构体类型</typeparam>
        /// <returns>是否成功发送</returns>
        public static bool MakeRequest<T>(HttpRequest<T> requester, Action<T> onResponse = null, bool autoHide = true) where T : IResponseProtocol
        {
            if (requester == null)
            {
                "UIRequestingLayer".LogErrorEditorOnly("Requester is null, making request failed.");
                return false;
            }
            
            var layer = UIManager.Instance.GetLayer<UIRequestLayer>("UIRequestLayer");
            if (layer == null)
            {
                "UIRequestingLayer".LogErrorEditorOnly("Failed to get UIRequestLayer, making request failed.");
                return false;
            }

            if (layer.IsRequesting)
            {
                "UIRequestingLayer".LogWarningEditorOnly("Failed to make request, this is already requesting.");
                return false;
            }

            if (!layer.SetRequesterAndSend(requester, onResponse, autoHide))
            {
                "UIRequestingLayer".LogErrorEditorOnly("Failed to send request, please check network settings and requester.");
                return false;
            }
            
            layer.SetHideOnFinish(autoHide);
            layer.Show();
            return true;
        }

        #endregion
    }
    
}