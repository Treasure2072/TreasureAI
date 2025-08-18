using UnityEngine;

namespace Data.Type
{
    public static class AIChatType
    {
        [SerializeField]
        public enum EChatType
        {
            Owner,
            Agent,
        }
        
        [System.Serializable]
        public struct TChatMessage
        {
            public long timestamp;
            public EChatType chatType;
            public string message;
        }
    }
}