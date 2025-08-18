using System;
using System.Collections.Generic;
using DragonLi.Core;
using Game;
using Newtonsoft.Json;
using UnityEngine;

namespace Data.Data
{
    public class RegisterAndLoginHandler : SandboxHandlerBase
    {
        #region Define

        private const string VisitorUUidKey = "visitor-uuid";
        private const string LoggedEmailsKey = "logged-emails-token-key";
        private const string EmailKey = "email-key";
        private const string InviteCodeKey = "invite-code-key";
        private const string IdKey = "id-key";
        private const string TokenKey = "token-key";
        private const string NameKey = "name-key";
        private const string InviterNameKey = "inviter-name-key";

        #endregion

        #region Property - Event

        public event Action<string, string> OnPlayerNameChanged; 

        #endregion

        #region Data

        /// <summary>
        /// 登陆过的邮箱
        /// </summary>
        public Dictionary<string, string> LoggedEmails
        {
            get => SandboxValue.GetValue<Dictionary<string, string>>(LoggedEmailsKey);
            set => SandboxValue.SetValue(LoggedEmailsKey, value);
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get => SandboxValue.GetValue<string>(EmailKey);
            set => SandboxValue.SetValue(EmailKey, value);
        }

        /// <summary>
        /// 自己邀请码
        /// </summary>
        public string InviteCode
        {
            get => SandboxValue.GetValue<string>(InviteCodeKey);
            set => SandboxValue.SetValue(InviteCodeKey, value);
        }

        /// <summary>
        /// 登录Id
        /// </summary>
        public string Id
        {
            get => SandboxValue.GetValue<string>(IdKey);
            set => SandboxValue.SetValue(IdKey, value);
        }

        /// <summary>
        /// 登录令牌
        /// </summary>
        public string Token
        {
            get => SandboxValue.GetValue<string>(TokenKey);
            set => SandboxValue.SetValue(TokenKey, value);
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name
        {
            get => SandboxValue.GetValue<string>(NameKey);
            set => SandboxValue.SetValue(NameKey, value);
        }

        public string InviterName
        {
            get => SandboxValue.GetValue<string>(InviterNameKey);
            set => SandboxValue.SetValue(InviterNameKey, value);
        }

        public string VisitorUuid
        {
            get
            {
                var uuid = ReadStringFromLocal(VisitorUUidKey);
                if (uuid == null)
                {
                    uuid = MathAPI.GenerateUUid();
                    SaveStringToLocal(VisitorUUidKey, uuid);
                }
                return uuid;
            }
        }

        #endregion

        #region SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);

            sandboxCallbacks[NameKey] = (preValue, newValue) => OnPlayerNameChanged?.Invoke(preValue as string, newValue as string);

            try
            {
                var jsonLogged = ReadStringFromLocal(LoggedEmailsKey);
                if (string.IsNullOrWhiteSpace(jsonLogged))
                {
                    LoggedEmails = new Dictionary<string, string>();
                }
                else
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonLogged);
                    LoggedEmails = data ?? new Dictionary<string, string>();
                }
            }
            catch (Exception e)
            {
                LoggedEmails = new Dictionary<string, string>();
                this.LogErrorEditorOnly($"[Logged Emails]: {e.Message}");
            }

            
            Email = string.IsNullOrEmpty(ReadStringFromLocal(EmailKey)) ? "" : ReadStringFromLocal(EmailKey);
            Id = string.IsNullOrEmpty(ReadStringFromLocal(IdKey)) ? "" : ReadStringFromLocal(IdKey);
            Token = string.IsNullOrEmpty(ReadStringFromLocal(TokenKey)) ? "none" : ReadStringFromLocal(TokenKey);
            Name = string.IsNullOrEmpty(ReadStringFromLocal(NameKey)) ? "" : ReadStringFromLocal(NameKey);
            InviterName = string.IsNullOrEmpty(ReadStringFromLocal(InviterNameKey)) ? "" : ReadStringFromLocal(InviterNameKey);
            InviteCode = string.IsNullOrEmpty(ReadStringFromLocal(InviteCodeKey)) ? "" : ReadStringFromLocal(InviteCodeKey);
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        #endregion

        #region Function

        private string ReadStringFromLocal(string key)
        {
            return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null;
        }
        
        private void SaveStringToLocal(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public void SaveToLocal()
        {
            SaveStringToLocal(LoggedEmailsKey, JsonConvert.SerializeObject(LoggedEmails));
            SaveStringToLocal(EmailKey, Email);
            SaveStringToLocal(InviteCodeKey, InviteCode);
            SaveStringToLocal(IdKey, Id);
            SaveStringToLocal(TokenKey, Token);
            SaveStringToLocal(NameKey, Name);
            SaveStringToLocal(InviterNameKey, InviterName);
            PlayerPrefs.Save();
        }

        #endregion
    }
}