using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using _Scripts.Utils;
using AOT;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [Serializable]
    public class LocalizationEntry
    {
        public string title;
        public string des;
        public string image;
    }
    
    public class TestScene : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void OpenInputModal();
#endif
        [SerializeField] private Image test;
        
        #region Properties

        // private AIChatSession ChatSession { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            
            this.LogEditorOnly($"{TimeAPI.GetVietnamTimeStamp()} {TimeAPI.GetUtcTimeStamp()}");
            // var tex = await DataDownloader.DownloadTexture2DAsync("https://sdmntprwestus.oaiusercontent.com/files/00000000-8e54-6230-863d-217c824cce9e/raw?se=2025-06-09T05%3A53%3A49Z&sp=r&sv=2024-08-04&sr=b&scid=473e507a-940c-53ea-b4b9-fb7763e9e6f7&skoid=7399a3a4-0259-4d43-bcd6-a56ceeb4c28b&sktid=a48cca56-e6da-484e-a814-9c849652bcb3&skt=2025-06-08T23%3A13%3A38Z&ske=2025-06-09T23%3A13%3A38Z&sks=b&skv=2024-08-04&sig=ugJbLQd8VmmHK2vPTXDHhwRO29lYpm50kTAz4drPFFw%3D");
            // var sp = DataDownloader.TextureToSprite(tex);
            // var rectComp = test.GetComponent<RectTransform>();
            // rectComp.sizeDelta = new Vector2(sp.rect.width, sp.rect.height);
            //
            // test.sprite = sp;
            //
            // var json = await DataDownloader.DownloadJsonAsync<Dictionary<string, LocalizationEntry>>(
            //     "https://enki-go.s3.ap-southeast-1.amazonaws.com/ann/startup/contents.json");
        }

//         private IEnumerator Start()
//         {
//             yield return CoroutineTaskManager.Waits.TwoSeconds;
//             
//             
//             
// #if UNITY_WEBGL && !UNITY_EDITOR
//             OpenInputModal();
//             WebGLInput.captureAllKeyboardInput = false;
// #endif
//             // ChatSession = GetComponent<AIChatSession>();
//             // yield return CoroutineTaskManager.Waits.TwoSeconds;
//             //
//             // EventDispatcher.AddEventListener(AIChatSession.AIChatSessionOpenEvent, OnSessionOpen);
//             // EventDispatcher.AddEventListener<HttpResponseProtocol>(AIChatSession.AIChatSessionMessageEvent, OnSessionMessage);
//
//             // 玩家需要先正常登陆后才能发起此连接, id传入服务端返回的id
//             // 这里是示例
//             // ...
//             // var playerId = "a5332795-98b5-449b-a3e4-c2442e20ab46";
//             // var connectionUrl = Settings.GetActiveConnectionConfiguration().chatbotServer;
//
//             // ****重要：退出聊天场景会自动断开连接，因为聊天服务无法承载那么多的连接
//             // 此功能暂时只用作展示
//             // ...
//             // ChatSession.Connect(connectionUrl + $"?id={playerId}");
//         }

        #endregion

        #region Callbacks

        private void OnSessionOpen()
        {
            // ChatSession.ChatTo("eliza", "Can you tell me what's my name is?");
        }
        
        private void OnSessionMessage(HttpResponseProtocol response)
        {
            this.LogEditorOnly(JsonConvert.SerializeObject(response));
        }

        #endregion
    }
}

