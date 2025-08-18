using Data;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class BuildingAreaInstance : Singleton<BuildingAreaInstance>
    {
        #region Properties

        private BuildingAreaSettings settings;
        public BuildingAreaSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = Resources.Load<BuildingAreaSettings>("BuildingAreaSettings");
                }

                return settings;
            }
            set
            {
                if (settings) return;
                settings = value;
            }
        }

        #endregion

    }
}
