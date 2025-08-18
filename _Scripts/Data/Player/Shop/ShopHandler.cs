using System;
using System.Collections.Generic;
using DragonLi.Core;
using DragonLi.Network;
using UnityEngine;

namespace Data
{
    public class ShopHandler : SandboxHandlerBase, IMessageReceiver
    {

        #region Function - SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            if (sandboxCallbacks == null)
            {
                throw new ArgumentNullException(nameof(sandboxCallbacks));
            }
            
        }

        protected override void OnInit()
        {
            base.OnInit();
            
        }

        #endregion
        
        #region Function - IMessageReceiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            
        }

        #endregion
    }
}