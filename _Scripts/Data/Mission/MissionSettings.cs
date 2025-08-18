using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "MissionSettings", menuName = "Scriptable Objects/MissionSettings")]
    public class MissionSettings : ScriptableObject
    {
        [Header("Daily")]
        [SerializeField] private List<FMissionDaily> missionDaily;
        
        
        [Space]
        
        [Header("Weekly")]
        [SerializeField] private int maxScore = 70;
        [SerializeField] private List<FMissionWeekly> missionWeekly;

        [SerializeField] private List<FTaskDaily> taskDaily;
        
        [SerializeField] private List<FTaskMain> taskMain;
        
        [Obsolete]
        public IReadOnlyList<FMissionDaily> MissionDaily => missionDaily.AsReadOnly();
        
        [Obsolete]
        public IReadOnlyList<FMissionWeekly> MissionWeekly => missionWeekly.AsReadOnly();

        #region Function - Daily

        public FMissionDaily GetMissionDaily(string id)
        {
            foreach (var daily in MissionDaily)
            {
                if(daily.id == id) return daily;
            }
            return default;
        }

        #endregion


        #region Function - Weekly

        public FMissionWeekly GetMissionWeekly(string id)
        {
            foreach (var weekly in MissionWeekly)
            {
                if(weekly.id == id) return weekly;
            }
            return default;
        }

        public int GetMaxScore()
        {
            return maxScore;
        }

        #endregion

        #region Fucntion

        public FTaskDaily GetTaskDailyRouteById(string id)
        {
            foreach (var task in taskDaily.Where(task => task.id == id))
            {
                return task;
            }

            return default;
        }

        public FTaskMain GetTaskMainRouteById(EFollowSocialPlatform platform)
        {
            foreach (var task in taskMain.Where(task => task.platform == platform))
            {
                return task;
            }
            
            return default;
        }

        #endregion
    }

    [Obsolete]
    [System.Serializable]
    public struct FMissionDaily
    {
        [SerializeField] public string id;
        [SerializeField] public string descriptionKey;
        [SerializeField] public int maxProgress;
        [SerializeField] public int coin;
        [SerializeField] public int dice;
        [SerializeField] public int score;
    }

    [Obsolete]
    [System.Serializable]
    public struct FMissionWeekly
    {
        [SerializeField] public string id;
        [SerializeField] public int progress;
        [SerializeField] public string description;
        [SerializeField] public int coin;
        [SerializeField] public int dice;
    }

    [System.Serializable]
    public struct FTaskDaily
    {
        [SerializeField] public string id;
        [SerializeField] public long coin;
        [SerializeField] public int dice;
        [SerializeField] public float token;
    }

    [System.Serializable]
    public struct FTaskMain
    {
        [SerializeField] public EFollowSocialPlatform platform;
        [SerializeField] public long coin;
        [SerializeField] public int dice;
        [SerializeField] public float token;
    }
}