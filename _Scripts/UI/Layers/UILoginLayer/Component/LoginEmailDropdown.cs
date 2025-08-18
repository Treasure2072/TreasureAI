using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LoginEmailDropdown : UIComponent
    {
        #region Property

        [Header("Reference")]
        [SerializeField] private LoginEmailComponent loginEmailComponent;
        [SerializeField] private TMP_Dropdown dropdown;

        #endregion

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            
            dropdown.ClearOptions();

            var options = PlayerSandbox.Instance.RegisterAndLoginHandler.LoggedEmails;
            if (options.Count == 0)
            {
                dropdown.interactable = false;
            }
            else
            {
                dropdown.AddOptions(options.Keys.ToList());
                dropdown.interactable = true;
            }
        }

        public override void OnShow()
        {
            base.OnShow();

            if (dropdown.options.Count > 0)
            {
                loginEmailComponent.Email = dropdown.options[dropdown.value].text;
            }
        }

        #endregion

        #region API

        public void AddAndSetOption(string email)
        {
            dropdown.AddOptions(new List<string>{email});
            dropdown.value = dropdown.options.Count - 1;
            dropdown.interactable = true;
            dropdown.RefreshShownValue();

            // 第一个不会调用OnDropdownValueChanged，需手动调用
            if (dropdown.options.Count == 1)
            {
                OnDropdownValueChanged(0);
            }
        }

        #endregion

        #region Callback

        private void OnDropdownValueChanged(int val)
        {
            loginEmailComponent.Email = dropdown.options[val].text;
        }

        #endregion

    }
}