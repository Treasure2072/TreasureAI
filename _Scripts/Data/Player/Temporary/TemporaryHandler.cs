using System;
using System.Collections.Generic;
using DragonLi.Core;

namespace Data
{
    public class TemporaryHandler : SandboxHandlerBase
    {
        #region Define

        private const string AnnOpenedKey = "temporary-opened-key";

        #endregion

        #region Property - Event

        public event Action<bool, bool> OnAnnOpenedChanged;

        #endregion
        
        #region Property - Data

        /// <summary>
        /// 公告是否被打开过
        /// </summary>
        public bool AnnOpened
        {
            get => SandboxValue.GetValue<bool>(AnnOpenedKey);
            set => SandboxValue.SetValue(AnnOpenedKey, value);
        }

        #endregion

        #region SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            
            sandboxCallbacks[AnnOpenedKey] = (preValue, newValue) => OnAnnOpenedChanged?.Invoke((bool)preValue, (bool)newValue);
        }

        protected override void OnInit()
        {
            base.OnInit();

            AnnOpened = false;
        }

        #endregion
    }
}