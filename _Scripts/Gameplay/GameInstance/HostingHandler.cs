using System;
using System.Collections.Generic;
using Data;
using DragonLi.Core;

namespace Game
{
    public class HostingHandler : SandboxHandlerBase
    {
        private const string kHostingKey = "hosting";

        public event Action<bool, bool> HostingChanged; 
        
        public bool Hosting
        {
            get => SandboxValue.GetValue<bool>(kHostingKey);
            set => SandboxValue.SetValue(kHostingKey, value);
        }

        protected override void OnInit()
        {
            base.OnInit();
            Hosting = false;
        }

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            if (sandboxCallbacks == null)
            {
                throw new ArgumentNullException(nameof(sandboxCallbacks));
            }

            sandboxCallbacks[kHostingKey] = (preValue, nowValue) => HostingChanged?.Invoke((bool)preValue, (bool)nowValue);
        }
    }
}