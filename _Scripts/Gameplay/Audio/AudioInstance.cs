using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public class AudioInstance : Singleton<AudioInstance>
    {
        private AudioSettings settings;

        public AudioSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = Resources.Load<AudioSettings>("AudioSettings");
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