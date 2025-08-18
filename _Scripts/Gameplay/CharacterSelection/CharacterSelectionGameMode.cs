using System;
using System.Collections;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(WorldObjectRegister))]
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class CharacterSelectionGameMode : GameMode, IMessageReceiver
    {
        private void Awake()
        {
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
        }

        private IEnumerator Start()
        {
            yield return null;
            GameSessionAPI.CharacterAPI.QueryCurrency();
            
            yield return CoroutineTaskManager.Waits.QuarterSecond;
            UIManager.Instance.GetLayer("UIBlackScreen").Hide();
            
            yield return CoroutineTaskManager.Waits.HalfSecond;
            UIManager.Instance.GetLayer("UICharacterSelectionLayer").Show();
        }

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if(service == GameSessionAPI.CharacterAPI.ServiceName && method == GSCharacterAPI.MethodQueryCurrency)
            {
                var coin = response.GetAttachmentAsInt("coin");
                var dice = response.GetAttachmentAsInt("dice");
                var token = response.GetAttachmentAsFloat("token");
                
                PlayerSandbox.Instance.CharacterHandler.Coin = coin;
                PlayerSandbox.Instance.CharacterHandler.Dice = dice;
                PlayerSandbox.Instance.CharacterHandler.Token = Mathf.Approximately(token, -1) ? 0 : token;
            }
        }
    }
}


