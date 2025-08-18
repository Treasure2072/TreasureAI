using System;
using System.Collections.Generic;
using System.Linq;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;
using WebSocketSharp;

namespace Data
{
    public class LanguageHandler : SandboxHandlerBase
    {
        public const string kLanguageKey = "language";

        #region Propeties

        public event Action<string, string> OnLanguageChanged;

        #endregion

        #region Properties - Data

        public string LanguageType
        {
            get => SandboxValue.GetValue<string>(kLanguageKey);
            set => SandboxValue.SetValue(kLanguageKey, value);
        }

        #endregion

        #region API

        public bool IsFirstIn()
        {
            return !PlayerPrefs.HasKey(kLanguageKey);
        }

        #endregion
        
        #region Function - SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            sandboxCallbacks[kLanguageKey] = (preValue, newValue) => OnLanguageChanged?.Invoke(preValue.ToString(), newValue.ToString());
        }

        protected override void OnInit()
        {
            base.OnInit();
            if (PlayerPrefs.HasKey(kLanguageKey) 
                && !PlayerPrefs.GetString(kLanguageKey).IsNullOrEmpty()
                && LocalizationManager.Instance.GetLanguages().Contains(PlayerPrefs.GetString(kLanguageKey)))
            {
                LanguageType = PlayerPrefs.GetString(kLanguageKey);
            }
            else
            {
                LanguageType = LocalizationManager.Instance.CurrentLanguage;
            }
        }

        #endregion
    }
}