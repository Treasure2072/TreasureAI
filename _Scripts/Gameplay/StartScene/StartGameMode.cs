using System;
using System.Collections;
using Data;
using DragonLi.Frame;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    [RequireComponent(typeof(WorldObjectRegister))]
    [RequireComponent(typeof(ConnectToServerComponent))]
    public class StartGameMode : GameMode
    {
        #region Property

        [Header("GameCache")]
        [SerializeField] private bool clearTutorialCache;

        private bool IsChessLoaded { get; set; } = false;

        #endregion
        
        #region Unity

        private void Awake()
        {
            if (clearTutorialCache)
            {
                TutorialHandler.ClearCache();
            }
            
#if UNITY_WEBGL && !UNITY_EDITOR
            Application.targetFrameRate = 45;
#endif
            
#if UNITY_IOS || UNITY_ANDROID
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
            Settings.LoadSettings();

            if (GameSessionConnection.Instance.IsConnected())
            {
                GameSessionConnection.Instance.Close();
            }
        }
        
        
        private void OnDestroy()
        {
            PlayerSandbox.Instance.CharacterHandler.ChessboardIdChanged -= OnCharacterDataReceived;
        }

        private IEnumerator Start()
        {
            while (!World.GetRegisteredObject<StartGameMode>(WorldObjectRegisterKey))
            {
                yield return null;
            }

            while (!UIInputLayer.GetLayer())
            {
                yield return null;
            }
            
            yield return null;
            
            // 本地相关配置初始化： 声音，语言
            SystemSandbox.Instance.InitializeSystemSandbox();
            
            UIManager.Instance.GetLayer("UIBlackScreen").Hide();
            UILoginLayer.GetLayer().Show();
        }

        #endregion

        #region API

        public IEnumerator GameStart()
        {
            UILoadingLayer.GetLayer().ShowLayer(() =>
            {
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("error"), 
                    this.GetLocalizedText("connection-timeout"), 
                    UITipLayer.ETipType.Bad);
            }, 30f);
            var id = PlayerSandbox.Instance.RegisterAndLoginHandler.Id;
            var token = PlayerSandbox.Instance.RegisterAndLoginHandler.Token;
            
            GetComponent<ConnectToServerComponent>()?.ConnectToServer(id, token);

            while (!GameSessionConnection.Instance.IsConnected())
            {
                yield return DragonLi.Core.CoroutineTaskManager.Waits.OneSecond;
            }
                        
            this.LogEditorOnly($"Game Start!");
            GameInstance.Instance.Initialize();
            PlayerSandbox.Instance.InitializePlayerSandbox();
            PlayerSandbox.Instance.CharacterHandler.ChessboardIdChanged += OnCharacterDataReceived;
            
            while (!IsChessLoaded || !PlayerSandbox.Instance.ChessBoardHandler.IsPulledGameData || !PlayerSandbox.Instance.ObjectiveHandler.IsPulledData)
            {
                yield return DragonLi.Core.CoroutineTaskManager.Waits.OneSecond;
            }
            
            UILoadingLayer.GetLayer()?.HideLayer();
            
            var blackLayer = UIManager.Instance.GetLayer("UIBlackScreen");
            blackLayer.Show();
            
            DragonLi.Core.SceneManager.Instance.AddSceneToLoadQueueByName(ChessBoardAPI.GetCurrentChessBoard(), 3f);
            DragonLi.Core.SceneManager.Instance.StartLoad();
        }

        #endregion

        #region Callback

        private void OnCharacterDataReceived(int? preVal, int nowVal)
        {
            if (nowVal < 0) return;
            IsChessLoaded = true;
        }

        #endregion

        #region Unity Editor

        [ContextMenu("Clear PlayerPrefs")]
        private void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        #endregion
    }
}
