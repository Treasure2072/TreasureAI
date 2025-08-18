using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIStaticsLayer : UILayer
    {
        #region Fields

        [Header("References")] 
        [SerializeField] private TextMeshProUGUI tmpLevel;
        [SerializeField] private UIAnimatedNumberText animTextDice;
        [SerializeField] private UIAnimatedNumberText animTextCoin;
        [SerializeField] private TextMeshProUGUI tmpToken;
        [SerializeField] private TextMeshProUGUI tmpUsd;

        [Header("Settings")] 
        [SerializeField] private bool canChanceCharacter;

        #endregion

        #region Properties

        private ChessGameBoard ChessBoardRef { get; set; }

        private bool Loaded { get; set; } = false;
        
        private Action<int, int?, int> SetLevelAction { get; set; }
        
        private Action<int?, int> SetDiceAction { get; set; }
        private Action<int?, int> SetCoinAction { get; set; }
        private Action<float?, float> SetTokenAction { get; set; }
        private Action<float?, float> SetUsdAction { get; set; }

        #endregion
        
        #region Unity

        private void Awake()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDestroy()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChanged;
        }

        private IEnumerator Start()
        {
            while (!(ChessBoardRef = World.GetRegisteredObject<ChessGameBoard>(ChessGameBoard.WorldObjectRegisterKey)))
            {
                yield return null;
            }
            
            Loaded = true;
        }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            SetLevelAction = (characterId, oldV, newV) => { SetLevel(newV); };
            SetDiceAction = (oldV, newV) =>
            {
                SetDice(newV);
                // this.LogEditorOnly($"===========QUERY Coin from Server============");
                // GameSessionAPI.CharacterAPI.QueryCurrency();
            };
            SetCoinAction = (oldV, newV) =>
            {
                SetCoin(newV);

                if (TutorialHandler.IsTriggerable(TutorialHandler.FirstUpgradeCharacterKey) && PlayerSandbox.Instance.CharacterHandler.Coin >= 30000)
                {
                    var layer = UITutorialLayer.GetLayer("UITutorialLayer-UpgradeCharacter");
                    if (layer != null)
                    {
                        var task = new List<IQueueableEvent>
                        {
                            new CustomEvent(() =>
                            {
                                TutorialHandler.Triggered(TutorialHandler.FirstUpgradeCharacterKey);
                                UIStaticsLayer.HideUIStaticsLayer();
                                UIActivityLayer.HideUIActivityLayer();
                                UIChessboardLayer.HideLayer();
                            
                                layer.OnHidEvent.AddListener(() =>
                                {
                                    UIActivityLayer.GetLayer()?.ActiveUpgradeCharacterFinger(true);
                                    UIStaticsLayer.ShowUIStaticsLayer();
                                    UIActivityLayer.ShowUIActivityLayer();
                                    UIChessboardLayer.ShowLayer();
                                });
                                layer.Show();
                            }),
                            new WaitForTrueEvent(() => !layer.IsShowing),
                        };
                        EventQueue.Instance.Enqueue(task);
                    }
                }

                if(oldV == null) return;
                var increase = newV - oldV;
                if (increase > 0)
                {
                    PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("coin-01", (int)increase);
                    PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("coin-02", (int)increase);
                    PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("coin-03", (int)increase);
                    PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("coin-04", (int)increase);
                    PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("coin-05", (int)increase);
                }
            };
            SetTokenAction = (oldV, newV) => { SetToken(newV); };
            SetUsdAction = (oldV, newV) => { SetUsd(newV); };
        }

        protected override void OnShow()
        {
            base.OnShow();
            SetLevel(PlayerSandbox.Instance.CharacterHandler.GetLevel());
            SetDice(PlayerSandbox.Instance.CharacterHandler.Dice);
            SetCoin(PlayerSandbox.Instance.CharacterHandler.Coin);
            SetToken(PlayerSandbox.Instance.CharacterHandler.Token);
            SetUsd(PlayerSandbox.Instance.CharacterHandler.USDT);
            PlayerSandbox.Instance.CharacterHandler.CharacterLevelChanged += SetLevelAction;
            PlayerSandbox.Instance.CharacterHandler.PlayerDiceChanged += SetDiceAction;
            PlayerSandbox.Instance.CharacterHandler.PlayerCoinChanged += SetCoinAction;
            PlayerSandbox.Instance.CharacterHandler.PlayerTokenChanged += SetTokenAction;
            PlayerSandbox.Instance.CharacterHandler.PlayerUSDTChanged += SetUsdAction;
        }

        protected override void OnHide()
        {
            base.OnHide();
            PlayerSandbox.Instance.CharacterHandler.CharacterLevelChanged -= SetLevelAction;
            PlayerSandbox.Instance.CharacterHandler.PlayerDiceChanged -= SetDiceAction;
            PlayerSandbox.Instance.CharacterHandler.PlayerCoinChanged -= SetCoinAction;
            PlayerSandbox.Instance.CharacterHandler.PlayerTokenChanged -= SetTokenAction;
            PlayerSandbox.Instance.CharacterHandler.PlayerUSDTChanged -= SetUsdAction;
        }

        #endregion

        #region Function

        private void ShowLayer()
        {
            Show();
        }

        private void SetLevel(int level)
        {
            if (tmpLevel == null) return;
            tmpLevel.text = string.Format(this.GetLocalizedText("level-fmt"), NumberUtils.GetDisplayNumberString(level));
        }
        private void SetDice(int dice)
        {
            if(animTextDice == null) return;
            animTextDice.SetNumber(dice, 0);
        }
        
        private void SetCoin(int coin)
        {
            if(animTextCoin == null) return;
            animTextCoin.SetNumber(coin);
        }

        private void SetToken(float token)
        {
            // if(animTextToken == null) return;
            // animTextToken.SetNumber(token);
            
            if(tmpToken == null) return;
            tmpToken.SetText(NumberUtils.GetDisplayNumberString(token, 1));
        }

        private void SetUsd(float usd)
        {
            if(tmpUsd == null) return;
            tmpUsd.SetText(NumberUtils.GetDisplayNumberString(usd, 1));
        }

        #endregion

        #region API

        public static void ShowUIStaticsLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIStaticsLayer>("UIStaticsLayer");
            Assert.IsNotNull(layer);
            layer.ShowLayer();
        }

        public static void HideUIStaticsLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIStaticsLayer>("UIStaticsLayer");
            Assert.IsNotNull(layer);
            layer.Hide();
        }

        #endregion

        #region Callabck

        private void OnLanguageChanged(string preVal, string newVal)
        {
            
        }

        #endregion
    }
}