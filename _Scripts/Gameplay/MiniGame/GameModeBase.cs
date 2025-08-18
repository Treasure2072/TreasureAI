using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    public class GameModeBase : MonoBehaviour
    {

        public static readonly string WorldObjectRegisterKey = "GameMode";
        
        #region Fields

        [Header("Settings - Debug")]
        [SerializeField] private bool debugMode;
        
        [Header("Settings - GameModeBase")] 
        [SerializeField] private bool AutoStart = true;
        [SerializeField] public bool IsPaused = false;
        
        #endregion

        #region Properties
        private bool IsEndGame { get; set; }

        protected float GameTime { get; private set; } = 0f;


        #endregion
        
        #region Events

        public event Action OnGameStart;    // 游戏开始事件
        public event Action OnGameEnd;      // 游戏结束事件
        public event Action OnGamePause;    // 游戏暂停事件
        public event Action OnGameResume;   // 游戏恢复事件
        public event Action<float> OnTimeUpdated; // 游戏时间更新事件
        #endregion

        #region Unity

        protected virtual void Awake()
        {
            Initialized();
        }
        
        protected virtual void Start()
        {
            if (AutoStart)
            {
                StartGame();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (IsPaused) return;
            
            GameTime += Time.deltaTime;
            OnTimeUpdated?.Invoke(GameTime);
        }
        
        protected virtual void OnDestroy()
        {
            World.UnregisterWorldObject(WorldObjectRegisterKey);
        }

        #endregion

        #region API

        /// <summary>
        /// 开始游戏
        /// </summary>
        protected void StartGame()
        {
            ResetGame();
            this.LogEditorOnly($"Starting game...");
            OnGameStart?.Invoke();
            OnGameStartInternal();
        }

        /// <summary>
        /// 结束游戏
        /// </summary>
        public void EndGame()
        {
            IsEndGame = true;
            OnGameEnd?.Invoke();
            OnGameEndInternal();
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            if (IsPaused) return;
            IsPaused = true;
            OnGamePause?.Invoke();
            OnGamePauseInternal();
        }

        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            if (!IsPaused) return;
            IsPaused = false;
            OnGameResume?.Invoke();
            OnGameResumeInternal();
        }

        /// <summary>
        /// 重置游戏
        /// </summary>
        public void ResetGame()
        {
            IsEndGame = false;
            GameTime = 0f;
            OnTimeUpdated?.Invoke(GameTime);
        }

        public bool IsEnd()
        {
            return IsEndGame;
        }
        
        #endregion

        #region Function
        

        private void Initialized()
        {
#if UNITY_EDITOR
            if (debugMode)
            {
                PlayerSandbox.Instance.DebugInitializePlayerSandbox();
                SystemSandbox.Instance.DebugInitializeSystemSandbox();
                GameInstance.Instance.Initialize();
            }
#else
            debugMode = false;
#endif
            OnInit();
            StartCoroutine(OnIEnumeratorInit());
        }
        
        #endregion
        
        #region Virtual Methods for Extension
        
        protected virtual void OnInit() { }

        protected virtual IEnumerator OnIEnumeratorInit()
        {
            yield return null;
        }


        /// <summary>
        /// 游戏开始时的自定义逻辑
        /// </summary>
        protected virtual void OnGameStartInternal() { }

        /// <summary>
        /// 游戏结束时的自定义逻辑
        /// </summary>
        protected virtual void OnGameEndInternal() { }

        /// <summary>
        /// 游戏暂停时的自定义逻辑
        /// </summary>
        protected virtual void OnGamePauseInternal() { }

        /// <summary>
        /// 游戏恢复时的自定义逻辑
        /// </summary>
        protected virtual void OnGameResumeInternal() { }

        #endregion

        
    }

}