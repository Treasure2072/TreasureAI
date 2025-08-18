using System;
using System.Collections.Generic;
using System.Linq;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;
using WebSocketSharp;

namespace Data
{
    public class VolumeHandler : SandboxHandlerBase
    {
        public const string kVolumeKey = "volume";

        #region Propeties - Event

        public event Action<float, float> OnVolumeChanged;

        #endregion

        #region Properties - Data

        public float Volume
        {
            get => Mathf.Clamp(SandboxValue.GetValue<float>(kVolumeKey), 0f, 1f);
            set => SandboxValue.SetValue(kVolumeKey, Mathf.Clamp(value, 0f, 1f));
        }

        #endregion
        
        #region Function - SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            sandboxCallbacks[kVolumeKey] = (preValue, newValue) => OnVolumeChanged?.Invoke((float)preValue, (float)newValue);
        }

        protected override void OnInit()
        {
            base.OnInit();
            if (PlayerPrefs.HasKey(kVolumeKey))
            {
                Volume = PlayerPrefs.GetFloat(kVolumeKey);
            }
            else
            {
                Volume = 0.4f;
            }
        }

        #endregion
    }
}