using System;
using System.Collections.Generic;
using _Scripts.UI.Common;
using _Scripts.UI.Common.Grids;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UIRanksLayer : UILayer
    {
        [Serializable]
        public enum ERankType
        {
            Coin,
            Dice,
            Token,
        }
        
        #region Properties

        [Header("References")]
        [SerializeField] private GridsContainerBase coinContainer;
        [SerializeField] private GridsContainerBase diceContainer;
        [SerializeField] private GridsContainerBase tokenContainer;
        
        [Header("Settings")]
        [SerializeField] private Color colorSelected;
        [SerializeField] private Color colorUnselected;
        
        private List<UIBasicButton> TableSelected { get; set; } = new();

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnClose"].As<UIBasicButton>()?.OnClickEvent.AddListener(_ => Hide());
            this["BtnCoin"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnCoinClickCallback);
            this["BtnToken"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnTokenClickCallback);
            
            TableSelected.Clear();
            TableSelected.Add(this["BtnCoin"].As<UIBasicButton>());
            TableSelected.Add(this["BtnToken"].As<UIBasicButton>());
        }

        protected override void OnShow()
        {
            base.OnShow();
            // Switcher(ERankType.Coin);
            // OnCoinClickCallback(this["BtnCoin"].As<UIBasicButton>());
            coinContainer.RecycleAllGrids(null, true);
            coinContainer.SpawnAllGrids();
            diceContainer.RecycleAllGrids(null, true);
            diceContainer.SpawnAllGrids();
            tokenContainer.RecycleAllGrids(null, true);
            tokenContainer.SpawnAllGrids();

            Switcher(ERankType.Dice);
        }

        #endregion

        #region API

        public static UIRanksLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIRanksLayer>("UIRanksLayer");
            Debug.Assert(layer != null);
            return layer;
        }

        public static void ShowLayer()
        {
            GetLayer()?.Show();
        }

        public static void HideLayer()
        {
            GetLayer()?.Hide();
        }

        #endregion

        #region Function

        private void Selected(UIBasicButton button)
        {
            foreach (var sender in TableSelected)
            {
                sender.GetComponent<Image>().color = button == sender ? colorSelected : colorUnselected;
            }
        }
        

        private void Switcher(ERankType type)
        {
            coinContainer.gameObject.SetActive(type == ERankType.Coin);
            diceContainer.gameObject.SetActive(type == ERankType.Dice);
            tokenContainer.gameObject.SetActive(type == ERankType.Token);
        }

        #endregion

        #region Callback

        private void OnCoinClickCallback(UIBasicButton sender)
        {
            Selected(sender);
            Switcher(ERankType.Coin);
        }

        private void OnTokenClickCallback(UIBasicButton sender)
        {
            Selected(sender);
            Switcher(ERankType.Token);
        }

        #endregion
    }
}