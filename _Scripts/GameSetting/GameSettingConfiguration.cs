using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    [CreateAssetMenu(fileName = "GameSetting", menuName = "GameSetting/GameConfig", order = -1)]
    public class GameSettingConfiguration : ScriptableObject 
    {
        #region Fields

        [Header("Settings")]
        [SerializeField] public bool useDev = false;
        
        [Header("Crypto")]
        [SerializeField] public string cryptoVector = "default-vector-1";
        [SerializeField] public string cryptoPassword = "paradow888";
        [SerializeField] public string cryptoKey = "paradow666";
        
        [Header("Connection")]
        [SerializeField] private GameConnectionConfiguration productionConnection;
        [SerializeField] private GameConnectionConfiguration devConnection;
        
        #endregion

        #region API

        public GameConnectionConfiguration GetConnectionConfiguration()
        {
#if UNITY_EDITOR
            return useDev ? devConnection : productionConnection;
#else
            return productionConnection;
#endif
        }

        #endregion
    }
}


