using System;
using System.Collections;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Game
{
    public class ConnectToServerComponent : MonoBehaviour
    {
        #region Properties

        [Header("Settings")]
        [SerializeField] private string eventName = "CONNECT-STATUS";
        
        [Header("Debug")] 
        [SerializeField] private bool useDevAccount = false;
        [SerializeField] private string devAccount = "dev-player";
        [SerializeField] private bool debugMessage = false;
        
        #endregion
        
        #region Unity

        private void Awake()
        {
            GameSessionConnection.Instance.DebugMessage = debugMessage;
        }

        private IEnumerator Start()
        {
            if(GameSessionConnection.Instance.IsConnected())
            {
                yield break;
            }
        
            yield return CoroutineTaskManager.Waits.OneSecond;
            
            UnityWebRequest.ClearCookieCache();
        
            var config = Settings.GetConfiguration();
            TextCryptoUtils.SetDefaultVector(config.cryptoVector);
            TextCryptoUtils.SetDefaultPassword(config.cryptoPassword);
            TextCryptoUtils.SetDefaultKey(config.cryptoKey);
        
            while (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return CoroutineTaskManager.Waits.OneSecond;
            }
        }

        #endregion

        #region API

        public void ConnectToServer(string id, string token)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var result =  GameSessionConnection.Instance.ConnectToServer($"{connection.sessionServer}connect?token={token}&user={id}");
            this.LogEditorOnly("连接服务状态: " + result);
        }

        #endregion
        
    }
}


