using UnityEngine;

namespace Game
{
    public static class BuildingAreaType
    {
        [System.Serializable]
        public enum EBuildAreaType
        {
            /// <summary>
            /// 未解锁
            /// </summary>
            Locked,

            /// <summary>
            /// 未修建
            /// </summary>
            NotBuilt,

            /// <summary>
            /// 未升级
            /// </summary>
            NotUpgraded,

            /// <summary>
            /// 正在升级
            /// </summary>
            Upgrading
        }
        
        [System.Serializable]
        public struct FCard
        {
            [SerializeField] public EBuildAreaType type;
            [SerializeField] public string areaStatusKey;
            [SerializeField] public Sprite icon;
        }
    }
}