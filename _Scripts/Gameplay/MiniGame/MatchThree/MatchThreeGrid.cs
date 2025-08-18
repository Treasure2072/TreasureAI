using System;
using _Scripts.UI.Common.Grids;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Gameplay.MiniGame.MatchThree
{
    public class MatchThreeGrid : GridBase
    {
        #region Properties
        
        private string ElementName { get; set; }
        private Action<MatchThreeElement> OnSelectOperated { get; set; } 

        #endregion
        
        #region GridBase

        public override void SetGrid<T>(params object[] args)
        {
            base.SetGrid<T>(args);
            ElementName = args[0] as string;
        }

        public override void RecycleElement(bool effect = false, bool bReDirection = true)
        {
            base.RecycleElement(effect, bReDirection);
            ElementName = null;
        }

        #endregion

        #region API

        public string GetName()
        {
            return this.ElementName;
        }

        #endregion
    }

}