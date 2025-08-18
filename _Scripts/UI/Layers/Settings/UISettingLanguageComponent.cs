using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using DragonLi.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UISettingLanguageComponent : UIComponent
    {
        #region Properties

        [Header("References")]
        [SerializeField] private TMP_Dropdown dropdownLanguage;
        [SerializeField] private TextMeshProUGUI textLanguage;

        #endregion

        #region Unity

        private void Awake()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChanged; 
        }

        private void OnDestroy()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChanged; 
        }

        #endregion

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            textLanguage.SetText(this.GetLocalizedText(SystemSandbox.Instance.LanguageHandler.LanguageType.ToLower()));
            InitOptions();
            dropdownLanguage.onValueChanged.AddListener(OnSwitchLanguage);
            dropdownLanguage.value = GetLanguages().IndexOf(SystemSandbox.Instance.LanguageHandler.LanguageType);
        }

        #endregion

        #region Function

        private List<string> GetLanguages()
        {
            var languages = LocalizationManager.Instance.GetLanguageList().Select(languages => languages.Key).ToList();
            return languages;
        }

        private void InitOptions()
        {
            dropdownLanguage.ClearOptions();
            foreach (var language in GetLanguages())
            {
                dropdownLanguage.options.Add(new TMP_Dropdown.OptionData(this.GetLocalizedText(language.ToLower())));
            }
        }

        private void UpdateDropdownText()
        {
            for (var i = 0; i < dropdownLanguage.options.Count; i++)
            {
                dropdownLanguage.options[i].text = this.GetLocalizedText(GetLanguages()[i].ToLower());
            }
            // foreach (var language in GetLanguages())
            // {
            //     dropdownLanguage.options.Add(new TMP_Dropdown.OptionData(this.GetLocalizedText(language.ToLower())));
            // }
        }

        #endregion

        #region Callback

        private void OnSwitchLanguage(int languageID)
        {
            if (languageID >= LocalizationManager.Instance.GetLanguageList().Count) return;
            UIManager.Instance.SwitchLanguage(languageID, () =>
            {
                SystemSandbox.Instance.LanguageHandler.LanguageType = LocalizationManager.Instance.GetLanguageList()[languageID].Key;
            });
        }

        private void OnLanguageChanged(string preValue, string newValue)
        {
            textLanguage.SetText(this.GetLocalizedText(newValue.ToLower()));
            UpdateDropdownText();
            this.SaveLanguage(newValue);
        }

        #endregion
    }
}