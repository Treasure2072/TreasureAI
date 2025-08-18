using System.Collections;
using _Scripts.UI.Common;
using Data;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game
{
    public class UIChessboardLayer : UILayer
    {
        #region Propeties
        
        [Header("References")]
        [SerializeField] private Image imgGo;
        [SerializeField] private TextMeshProUGUI tmpDiceMultiple;
        
        [Header("References - AI")]
        [SerializeField] private Toggle toggleHost;
        [SerializeField] private Transform ai;
        [SerializeField] private GameObject aiOn;
        [SerializeField] private GameObject aiOff;
        
        [Header("Tutorial")]
        [SerializeField] private GameObject fingerRoll;
        [SerializeField] private GameObject fingerShop;
        
        public bool Loaded { get; private set; } = false;

        private ChessGameMode GameModeRef { get; set; }
        
        private Coroutine UpdateGoCoroutine { get; set; }
        
        private Tween BreathTween { get; set; }

        #endregion

        #region Unity

        private IEnumerator Start()
        {
            while (!(GameModeRef = GameMode.GetGameMode<ChessGameMode>(ChessGameMode.WorldObjectRegisterKey)))
            {
                yield return null;
            }

            Loaded = true;
        }

        private void OnEnable()
        {
            if (UpdateGoCoroutine != null)
            {
                StopCoroutine(UpdateGoCoroutine);
            }
            UpdateGoCoroutine = StartCoroutine(UpdateGoIEnumerator());
        }

        private void OnDisable()
        {
            if(UpdateGoCoroutine == null) return;
            StopCoroutine(UpdateGoCoroutine);
        }

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnGO"].As<UIBasicButton>().OnClickEvent?.AddListener(OnGoClicked);
            this["ORDERS"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickOrders);
            this["SHOP"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickShop);
            this["RANK"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickRank);
            this["Settings"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickSettings);
            this["BtnMultiple"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickMultiple);
            toggleHost.onValueChanged.AddListener(val =>
            {
                GameInstance.Instance.HostingHandler.Hosting = val;
                aiOn.SetActive(val);
                aiOff.SetActive(!val);
                SetAIBreath(val);
            });
        }

        protected override void OnShow()
        {
            base.OnShow();
            PlayerSandbox.Instance.CharacterHandler.PlayerDiceChanged += OnDiceChange;
            SystemSandbox.Instance.DiceMultipleHandler.OnDiceMultipleChanged += OnDiceMultipleChanged;
            toggleHost.isOn = GameInstance.Instance.HostingHandler.Hosting;
            GameMode.GetGameMode<ChessGameMode>(ChessGameMode.WorldObjectRegisterKey).PlayerCameraControllerRef.SetControllerEnable(true);
            RefreshDiceMultiple();
        }

        protected override void OnHide()
        {
            base.OnHide();
            PlayerSandbox.Instance.CharacterHandler.PlayerDiceChanged -= OnDiceChange;
            SystemSandbox.Instance.DiceMultipleHandler.OnDiceMultipleChanged -= OnDiceMultipleChanged;
            GameMode.GetGameMode<ChessGameMode>(ChessGameMode.WorldObjectRegisterKey).PlayerCameraControllerRef.SetControllerEnable(false);
            UIShopLayer.HideLayer();
            UIInventoryLayer.HideLayer();

        }

        #endregion

        #region Functions

        private IEnumerator UpdateGoIEnumerator()
        {
            while (!Loaded)
            {
                yield return null;
            }

            while (!GameModeRef.RollDiceRef)
            {
                yield return null;
            }
            
            while (true)
            {
                // if (imgGo)
                // {
                //     imgGo.color = GameModeRef.RollDiceRef.CanRollDice() ? Color.white : Color.black;
                // }

                if (this["BtnGo"])
                {
                    this["BtnGo"].GetComponent<Button>().interactable = GameModeRef.RollDiceRef.CanRollDice();
                }

                yield return CoroutineTaskManager.Waits.HalfSecond;
            }
        }

        public void SetAIBreath(bool isBreath)
        {
            // 如果已有 tween，先停止它
            if (BreathTween != null && BreathTween.IsActive())
            {
                BreathTween.Kill();
                BreathTween = null;
            }

            if (isBreath)
            {
                // 开始呼吸动画
                BreathTween = ai.DOScale(1.2f, 1f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
            else
            {
                // 停止后恢复原始大小
                ai.localScale = Vector3.one;
            }
        }

        private void RefreshDiceMultiple()
        {
            tmpDiceMultiple.SetText($"X{SystemSandbox.Instance.DiceMultipleHandler.DiceMultiple}");
        }

        #endregion

        #region API
        
        public static UIChessboardLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIChessboardLayer>("UIChessboardLayer");
            Assert.IsNotNull(layer);
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

        public void ActiveFingerRoll(bool active = false)
        {
            fingerRoll.SetActive(active);
        }

        public void ActiveFingerShop(bool active = false)
        {
            fingerShop.SetActive(active);
        }

        #endregion

        #region Callbacks

        private void OnGoClicked(UIBasicButton sender)
        {
            if (!Loaded)
            {
                this.LogErrorEditorOnly("Loaded is false!");
                return;
            }

            if (TutorialHandler.IsTriggerable(TutorialHandler.FirstEnterKey))
            {
                TutorialHandler.Triggered(TutorialHandler.FirstEnterKey);
                ActiveFingerRoll(false);
            }
            
            if (PlayerSandbox.Instance.CharacterHandler.Dice <= 0)
            {
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("notice"), 
                    this.GetLocalizedText("dice-shot-des"), 
                    UITipLayer.ETipType.Bad,
                    ShowLayer);
                return;
            }
            
            if (!GameSessionConnection.Instance.IsConnected())
            {
                UITipLayer.DisplayTip("Ops", "The connection is closed. Please exit the game and try again.", UITipLayer.ETipType.Bad);
                return;
            }

            GameModeRef?.RollDiceRef.Move();
        }

        private void OnClickOrders(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            UIOrdersLayer.GetLayer()?.Show();
        }

        private void OnClickShop(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            if (TutorialHandler.IsTriggerable(TutorialHandler.FirstLackDiceKey) && PlayerSandbox.Instance.CharacterHandler.Dice <= 0)
            {
                TutorialHandler.Triggered(TutorialHandler.FirstLackDiceKey);
                ActiveFingerShop(false);
            }
            
            UIShopLayer.ShowLayer();
        }

        private void OnClickRank(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            UIRanksLayer.ShowLayer();
        }

        private void OnClickSettings(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            UIPersonalCenterLayer.GetLayer().Show();
            // UIManager.Instance.GetLayer("UIBlackScreen").Show();
            // CoroutineTaskManager.Instance.WaitSecondTodo(UIPersonalCenterLayer.GetLayer().Show, 3f);
        }

        private void OnClickMultiple(UIBasicButton sender)
        {
            SystemSandbox.Instance.DiceMultipleHandler.NextDiceMultiple();
        }

        private void OnDiceChange(int? preValue, int newValue)
        {
            if (TutorialHandler.IsTriggerable(TutorialHandler.FirstLackDiceKey) && newValue <= 0)
            {
                ActiveFingerShop(true);
            }
        }

        private void OnDiceMultipleChanged(int? preValue, int newValue)
        {
            RefreshDiceMultiple();
        }

        #endregion
    }

}