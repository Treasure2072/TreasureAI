using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    [CreateAssetMenu(fileName = "GameSetting", menuName = "GameSetting/GameConnection")]
    public class GameConnectionConfiguration : ScriptableObject
    {
        #region Properties

        [Header("Connection")]
        [SerializeField] public string httpServer = "http://localhost:8080";
        [SerializeField] public string sessionServer = "ws://localhost:8080";
        [SerializeField] public string chatbotServer = "ws://localhost:8082/chatbot";

        #endregion
    }
}


