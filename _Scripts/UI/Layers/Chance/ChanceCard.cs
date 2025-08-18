using System;
using _Scripts.UI.Common.Grids;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ChanceCard : MonoBehaviour
    {
        private static readonly int Play = Animator.StringToHash("Play");

        #region Fields

        [Header("References")]
        [SerializeField] private TextMeshProUGUI description;

        #endregion
        
        #region API
        
        public void SetDescription(string des)
        {
            description.text = des;
        }

        #endregion
    }

}