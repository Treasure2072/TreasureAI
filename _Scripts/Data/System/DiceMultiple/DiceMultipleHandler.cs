using System;
using System.Collections.Generic;
using DragonLi.Core;

namespace Game
{
    public class DiceMultipleHandler : SandboxHandlerBase
    {
        #region Define

        private const string DiceMultipleKey = "dice-multiple-key";

        #endregion

        #region Property - Event

        public event Action<int?, int> OnDiceMultipleChanged;

        #endregion

        #region Property - Data

        /// <summary>
        /// 骰子使用倍率
        /// 有 1，2，3，5，10
        /// </summary>
        public int DiceMultiple
        {
            get => SandboxValue.GetValue<int>(DiceMultipleKey);
            set => SandboxValue.SetValue(DiceMultipleKey, value);
        }

        #endregion

        #region SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            sandboxCallbacks[DiceMultipleKey] = (preValue, newValue) => OnDiceMultipleChanged?.Invoke((int?)preValue, (int)newValue);
        }

        protected override void OnInit()
        {
            base.OnInit();
            DiceMultiple = 1;
        }

        #endregion

        #region API

        public void NextDiceMultiple()
        {
            DiceMultiple =  DiceMultiple switch
            {
                1 => 2,
                2 => 3,
                3 => 5,
                5 => 10,
                10 => 1,
                _ => 1,
            };
        }

        #endregion
    }
}