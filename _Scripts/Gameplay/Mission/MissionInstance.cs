using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class MissionInstance : Singleton<MissionInstance>
    {
        private MissionSettings settings;

        public MissionSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = Resources.Load<MissionSettings>("MissionSettings");
                }

                return settings;
            }
            set
            {
                if (settings) return;
                settings = value;
            }
        }
    }
}