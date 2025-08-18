using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "ChanceSettings", menuName = "Scriptable Objects/ChanceSettings")]

    public class ChanceSettings : ScriptableObject
    {
        [SerializeField] private List<ChanceEvent> chanceEvents = new List<ChanceEvent>();

        /// <summary>
        /// 获取事件相关信息
        /// </summary>
        /// <param name="chanceID">事件id</param>
        /// <returns></returns>
        public ChanceEvent GetChanceEvent(int chanceID)
        {
            return chanceEvents[chanceID - 1];
        }
    }

    [Serializable]
    public class ChanceEvent
    {
        [SerializeField] private string descriptionKey;
        [SerializeField] public ChanceType.EChanceType chanceType;
        [SerializeField] public ChanceType.EEffectType effectType;
        [SerializeField] [Range(0, 100)] public int rate;

        public string GetDescriptionKey()
        {
            return descriptionKey;
        }
    }

    [Serializable]
    public abstract class ChanceType
    {
        [Serializable]
        public enum EChanceType
        {
            None = 0,
            Good,
            Bad,
        }
        
        [Serializable]
        public enum EEffectType
        {
            None = 0,
            CashSmall,
            CashMedium,
            CashBig,
            DiceSmall,
            DiceMedium,
            DiceBig,
        }
    }

}