using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIBankOpenButton : MonoBehaviour
    {
        #region Properties

        [Header("References")]
        [SerializeField] private Button btnOpen;

        #endregion

        #region Unity

        private void Awake()
        {
            btnOpen.onClick.AddListener(OnOpenClick);
        }

        #endregion

        #region Function

        private static void OnOpenClick()
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            UIBankLayer.ShowLayer();
        }

        #endregion
    }
}