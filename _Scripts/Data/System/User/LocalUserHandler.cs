using System.Collections.Generic;
using DragonLi.Core;
using Newtonsoft.Json;
using UnityEngine;
using Reown.AppKit;
namespace Data
{
    public class LocalUserHandler : SandboxHandlerBase
    {
        private const string KLocalUserKey = "local-user";

        #region Property - Data

        public HashSet<string> LocalUsers
        {
            get => SandboxValue.GetValue<HashSet<string>>(KLocalUserKey);
            set => SandboxValue.SetValue(KLocalUserKey, value);
        }

        #endregion

        #region Function

        private HashSet<string> ReadUsersFromLocal(string key)
        {
            return PlayerPrefs.HasKey(key) ? JsonConvert.DeserializeObject<HashSet<string>>(PlayerPrefs.GetString(key)) : null;
        }
        
        private void SaveStringToLocal(string key, HashSet<string> value)
        {
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(value));
            PlayerPrefs.Save();
        }

        #endregion

        #region Function - SandboxHandlerBase

        protected override void OnInit()
        {
            base.OnInit();
            LocalUsers = ReadUsersFromLocal(KLocalUserKey);
        }

        #endregion
    }
}