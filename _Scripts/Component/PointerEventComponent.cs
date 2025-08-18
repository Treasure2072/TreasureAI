using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class PointerEventComponent : 
        MonoBehaviour, 
        IPointerDownHandler, 
        IPointerUpHandler, 
        IPointerMoveHandler, 
        IPointerClickHandler, 
        IPointerExitHandler, 
        IPointerEnterHandler
    {
        #region Properties - Event

        public event Action<PointerEventData> OnPointerDownHandler;
        public event Action<PointerEventData> OnPointerUpHandler;
        public event Action<PointerEventData> OnPointerMoveHandler;
        public event Action<PointerEventData> OnPointerClickHandler;
        public event Action<PointerEventData> OnPointerExitHandler;
        public event Action<PointerEventData> OnPointerEnterHandler;

        #endregion

        #region Properties

        public bool IsPressed { get; private set; }
        public bool IsEnter { get; private set; }
        public bool IsMoving { get; private set; }

        #endregion

        #region Unity

        private void Awake()
        {
            IsPressed = false;
            IsEnter = false;
        }

        private void OnDestroy()
        {
            ClearAllHandlers();
        }

        #endregion

        #region API

        /// <summary>
        /// 清空所有 Pointer 事件的订阅者
        /// </summary>
        public void ClearAllHandlers()
        {
            OnPointerDownHandler = null;
            OnPointerUpHandler = null;
            OnPointerMoveHandler = null;
            OnPointerClickHandler = null;
            OnPointerExitHandler = null;
            OnPointerEnterHandler = null;
        }

        #endregion

        #region Function

        /// <summary>
        /// 安全调用事件，捕获订阅者的异常
        /// </summary>
        /// <param name="handler">事件处理方法</param>
        /// <param name="eventData">Pointer 事件数据</param>
        private void SafeInvoke(Action<PointerEventData> handler, PointerEventData eventData)
        {
            try
            {
                handler?.Invoke(eventData);
            }
            catch (Exception ex)
            {
                this.LogErrorEditorOnly($"[PointerEventComponent] Error invoking handler: {ex.Message}\n{ex.StackTrace}");
            }
        }

        #endregion

        #region Callback - Pointer

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            SafeInvoke(OnPointerDownHandler, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            IsMoving = false;
            SafeInvoke(OnPointerUpHandler, eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            IsMoving = true;
            SafeInvoke(OnPointerMoveHandler, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            IsPressed = false;
            SafeInvoke(OnPointerClickHandler, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsEnter = false;
            SafeInvoke(OnPointerExitHandler, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsEnter = true;
            SafeInvoke(OnPointerEnterHandler, eventData);
        }

        #endregion
    }
}