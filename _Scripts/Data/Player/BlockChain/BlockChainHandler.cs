using System;
using System.Collections.Generic;
using DragonLi.Core;

namespace Data
{
    public class BlockChainHandler : SandboxHandlerBase
    {
        #region Define

        private const string AccountKey = "block-chain-account-key";

        #endregion

        #region Property - Event

        public event Action<string, string> OnAccountChanged;

        #endregion

        #region Property - Data

        public string Account
        {
            get => SandboxValue.GetValue<string>(AccountKey);
            set => SandboxValue.SetValue(AccountKey, value);
        }

        #endregion

        #region MyRegion

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            if (sandboxCallbacks == null)
            {
                throw new ArgumentNullException(nameof(sandboxCallbacks));
            }
            
            // TODO: 监听 sandbox 里面值的改变
            // ...
            sandboxCallbacks[AccountKey] = (preValue, newValue) => OnAccountChanged?.Invoke(preValue as string, newValue as string);
            
        }

        protected override void OnInit()
        {
            base.OnInit();
            Account = System.Guid.NewGuid().ToString();
        }

        #endregion
    }
}