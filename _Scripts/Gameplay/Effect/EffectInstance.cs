using Data;
using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class EffectInstance : Singleton<EffectInstance>
    {
        private EffectSettings settings;

        public EffectSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = Resources.Load<EffectSettings>("EffectSettings");
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