using System;
using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class ChessTile : MonoBehaviour
    {
        #region Define

        public enum EArriveAnimationType
        {
            None = 0,
            Teleport,
            Short,
            Game,
        }

        #endregion
        
        #region Fields
        
        [Header("ChessTile")]
        [SerializeField] private EArriveAnimationType arriveAnimationType = EArriveAnimationType.None;
        
        #endregion

        #region Properties

        // ReSharper disable once MemberCanBePrivate.Global
        protected MaterialBlinker Blinker { get; set; }
        public int TileIndex { get; private set; }

        #endregion

        #region Unity

        protected virtual void Awake()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChanged;
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
        }

        private void OnDestroy()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChanged;
        }

        #endregion

        #region Functin
        
        protected virtual void OnLanguageChanged(string previousLanguage, string newLanguage) {  }

        protected virtual void OnReceiveMessage(HttpResponseProtocol response, string service, string method) { }

        #endregion
        
        #region API

        /// <summary>
        /// 初始化格子
        /// </summary>
        /// <param name="tileIndex"></param>
        public virtual void Initialize(int tileIndex)
        {
            Blinker = GetComponentInChildren<MaterialBlinker>();
            TileIndex = tileIndex;
        }
        
        /// <summary>
        /// 当玩家经过的时候
        /// </summary>
        /// <returns>经过时需要抛出的事件</returns>
        public virtual List<IQueueableEvent> OnPass()
        {
            // Blinker?.BlinkOnce();
            return null;
        }
        
        /// <summary>
        /// 玩家到达的时候
        /// </summary>
        /// <returns>到达时需要抛出的事件</returns>
        public virtual List<IQueueableEvent> OnArrive()
        {
            return null;
        }

        /// <summary>
        /// 播放玩家到达时的动画
        /// </summary>
        /// <param name="animationType">动画类型</param>
        /// <param name="playerTileIndex">当前玩家所在的位置</param>
        public virtual void PlayArriveAnimation(EArriveAnimationType animationType, int playerTileIndex)
        {
            
        }

        /// <summary>
        /// 获得玩家站立的位置
        /// </summary>
        /// <returns>世界坐标</returns>
        public virtual Vector3 GetStandPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// 获得当前格子的正方向，既是下一个格子的方向，玩家也会朝向这个方向
        /// </summary>
        /// <returns>方向</returns>
        public virtual Vector3 GetForwardDirection()
        {
            return -transform.forward;
        }

        public EArriveAnimationType GetArriveAnimationType()
        {
            return arriveAnimationType;
        }
        
        #endregion
    }
}


