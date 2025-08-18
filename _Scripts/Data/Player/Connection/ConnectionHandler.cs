using System;
using System.Collections.Generic;
using DragonLi.Core;

namespace Data
{
    public class ConnectionHandler : SandboxHandlerBase
    {

        private const string kUserIdKey = "user-id";
        private const string kUserTokenKey = "user-token";

        #region Properties - Event

        public event Action<string, string> UserIdChanged;
        public event Action<string, string> UserTokenChanged;

        #endregion
        
        #region Properties - Data

        public string UserId
        {
            get => SandboxValue.GetValue<string>(kUserIdKey);
            set => SandboxValue.SetValue(kUserIdKey, value);
        }

        public string UserToken
        {
            get => SandboxValue.GetValue<string>(kUserTokenKey);
            set => SandboxValue.SetValue(kUserTokenKey, value);
        }

        #endregion

        #region Function - SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            sandboxCallbacks[kUserIdKey] = (preValue, nowValue) => UserIdChanged?.Invoke(preValue as string, nowValue as string);
            sandboxCallbacks[kUserTokenKey] = (preValue, nowValue) => UserTokenChanged?.Invoke(preValue as string, nowValue as string);
        }

        #endregion
    }
}