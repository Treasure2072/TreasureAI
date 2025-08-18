using System;
using System.Collections;
using System.Runtime.InteropServices;
using _Scripts.UI.Common;
using _Scripts.Utils;
using Crosstales.RTVoice;
using Data;
using Data.Type;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.UI;
using HuggingFace.API;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class UIConversationLayer : UILayer
    {
        [FormerlySerializedAs("inputTransform")] [SerializeField] private RectTransform inputRect;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private UIConversationContainer container;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private AwsTTS tts;
        [SerializeField] private string debugMessage;
        
 #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void FocusUnityCanvas();
        [DllImport("__Internal")]
        private static extern void ExpandTelegramWindow();
        [DllImport("__Internal")]
        private static extern string PromptInput(string message, string hint);
        [DllImport("__Internal")]
        private static extern void OpenInputModal();
#endif

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnSend"].As<UIBasicButton>().OnClickEvent?.AddListener(OnSendClick);
            this["BtnBack"].As<UIBasicButton>().OnClickEvent?.AddListener(OnBackClick);
            this["BtnMicphone"].As<UIBasicButton>().OnClickEvent?.AddListener(OnMicphoneClick);
            
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            // this["BtnBack"].gameObject.SetActive(false);
            if (inputRect)
            {
                inputRect.sizeDelta = inputRect.sizeDelta.y * Vector2.up + 450f * Vector2.right;
            }
            var btnSendRect = this["BtnSend"].GetComponent<RectTransform>();
            if (btnSendRect)
            {
                btnSendRect.sizeDelta = btnSendRect.sizeDelta.y * Vector2.up + 100f * Vector2.right;
            }
            if (inputRect)
            {
                inputField.gameObject.SetActive(true);
            }
#endif
            PlayerSandbox.Instance.AIChatHandler.OnSessionMessage += OnSessionMessage;
// #if UNITY_WEBGL && !UNITY_EDITOR
//             inputField.onEndEdit.AddListener(e =>
//             {
//                 StartCoroutine(ProcessRestoreFocus());
//             });
// #endif
        }

//         private IEnumerator ProcessRestoreFocus()
//         {
//             yield return null;
// #if UNITY_WEBGL && !UNITY_EDITOR
//             EventSystem.current.SetSelectedGameObject(sendButton.gameObject);
//             FocusUnityCanvas();
//             ExpandTelegramWindow();
//             
//             yield return null;
//             // 获取或创建 Touchscreen 设备
//             var touchscreen = Touchscreen.current;
//             if (touchscreen == null)
//             {
//                 this.LogConsole("No touchscreen found.");
//                 yield break;
//             }
//             this.LogConsole("Simulated touchscreen: Start.");
//             // 模拟触摸按下
//             InputSystem.QueueStateEvent(touchscreen, new TouchState
//             {
//                 touchId = 0,
//                 phase = UnityEngine.InputSystem.TouchPhase.Began,
//                 position = new Vector2(100, 100) // 起始位置
//             });
//
//             yield return CoroutineTaskManager.Waits.ZeroOneSecond;
//             InputSystem.QueueStateEvent(touchscreen, new TouchState
//             {
//                 touchId = 0,
//                 phase = UnityEngine.InputSystem.TouchPhase.Moved,
//                 position = new Vector2(300, 300) // 结束位置
//             });
//
//             yield return CoroutineTaskManager.Waits.ZeroOneSecond;
//             InputSystem.QueueStateEvent(touchscreen, new TouchState
//             {
//                 touchId = 0,
//                 phase = UnityEngine.InputSystem.TouchPhase.Ended,
//                 position = new Vector2(300, 300) // 结束位置
//             });
//             this.LogConsole("Simulated touchscreen: End.");
// #endif
//         }

        protected override void OnShow()
        {
            base.OnShow();
            container.RecycleAllGrids();
            container.SpawnAllGrids(PlayerSandbox.Instance.AIChatHandler.GetMessages());
            EventDispatcher.AddEventListener<string>(WebGLInputReceiver.WEBGL_INPUT_RECEIVED, OnWebGLInputReceived);
        }

        protected override void OnHide()
        {
            base.OnHide();
            EventDispatcher.RemoveEventListener<string>(WebGLInputReceiver.WEBGL_INPUT_RECEIVED, OnWebGLInputReceived);
        }

        private static UIConversationLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIConversationLayer>("UIConversationLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowLayer()
        {
            GetLayer().Show();
        }

        public static void HideLayer()
        {
            GetLayer().Hide();
        }

        private void OnWebGLInputReceived(string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            GameMode.GetGameMode<TuringBarGameMode>().ChatTo(data);
            sendButton.interactable = false;
        }
        

        private void OnTextChanged(string oldV, string newV)
        {
            inputField.text = newV;
        }
        
        private void OnSendClick(UIBasicButton sender)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
            OpenInputModal();
#elif UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            var message = inputField.text;
            if(string.IsNullOrEmpty(message)) return;
            GameMode.GetGameMode<TuringBarGameMode>().ChatTo(message);
#endif
        }
        
        private void OnBackClick(UIBasicButton sender)
        {
            // return;
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName(ChessBoardAPI.GetCurrentChessBoard(), 2, true);
            SceneManager.Instance.StartLoad();
            Hide();
        }

        private void OnMicphoneClick(UIBasicButton sender)
        {
            UIMicphoneLayer.GetLayer()?.RecordResultEvent.AddListener((res, text) =>
            {
                if (res)
                {
                    GameMode.GetGameMode<TuringBarGameMode>().ChatTo(text);
                }
                else
                {
                    UITipLayer.DisplayTip(this.GetLocalizedText("turing"), "Speech recognition was not successful!");
                }
                
                UIMicphoneLayer.HideLayer();
            });
            UIMicphoneLayer.ShowLayer();
        }
        
        private async void OnSessionMessage(AIChatType.TChatMessage message)
        {
            // message.timestamp = TimeAPI.GetUtcTimeStamp();
            AudioClip clip = null;
            if (message.chatType == AIChatType.EChatType.Owner)
            {
                if (inputField)
                {
                    inputField.text = "";
                }
            }
            
            if (message.chatType == AIChatType.EChatType.Agent)
            {
                sendButton.interactable = true;
                clip = await tts.TTS(message.message, message.timestamp);
            }
            container.SpawnMessage(message, clip);

            CoroutineTaskManager.Instance.WaitFrameEnd(() =>
            {
                if(scrollRect)
                    scrollRect.verticalNormalizedPosition = 0;
            });
        }
    }
}