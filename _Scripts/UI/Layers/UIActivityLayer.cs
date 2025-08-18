using _Scripts.UI.Common;
using Data;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIActivityLayer : UILayer
    {
        #region Properties

        [SerializeField] private GameObject annNotice;
        [SerializeField] private GameObject signinNotice;
        [SerializeField] private GameObject dailyNotice;
        [SerializeField] private GameObject characterNotice;
        [SerializeField] private GameObject fingerUpgradeCharacter;
        
        private bool IsProcessing { get; set; }

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnAnnouncement"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickAnnouncement);
            this["BtnTask"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickTask);
            this["BtnSign"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickSign);
            this["BtnBuildArea"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickBuildArea);
            this["BtnTeleport"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickTeleport);
            this["BtnCharacter"].As<UIBasicButton>().OnClickEvent?.AddListener(OnClickCharacter);
            
            ActiveUpgradeCharacterFinger(false);
        }

        protected override void OnShow()
        {
            base.OnShow();
            PlayerSandbox.Instance.CharacterHandler.PlayerCoinChanged += OnPlayerCoinChanged;
            PlayerSandbox.Instance.TemporaryHandler.OnAnnOpenedChanged += OnAnnOpenedChanged;
            PlayerSandbox.Instance.ObjectiveHandler.ObjectiveDailyChanged += OnObjectiveDailyChanged;
            PlayerSandbox.Instance.ObjectiveHandler.ObjectiveCheckInChanged += OnObjectiveCheckInChanged;
            PlayerSandbox.Instance.ObjectiveHandler.OnPaymentRewardIndexChanged += OnPaymentRewardIndexChanged;
        }

        protected override void OnHide()
        {
            base.OnHide();
            PlayerSandbox.Instance.CharacterHandler.PlayerCoinChanged -= OnPlayerCoinChanged;
            PlayerSandbox.Instance.TemporaryHandler.OnAnnOpenedChanged -= OnAnnOpenedChanged;
            PlayerSandbox.Instance.ObjectiveHandler.ObjectiveDailyChanged -= OnObjectiveDailyChanged;
            PlayerSandbox.Instance.ObjectiveHandler.ObjectiveCheckInChanged -= OnObjectiveCheckInChanged;
            PlayerSandbox.Instance.ObjectiveHandler.OnPaymentRewardIndexChanged -= OnPaymentRewardIndexChanged;
            UIStaticsLayer.HideUIStaticsLayer();
            UIChessboardLayer.HideLayer();
        }

        #endregion

        #region Functions

        private void ShowLayer()
        {
            IsProcessing = false;
            Show();
            RefreshAnnNotice();
            RefreshTaskNotice();
            RefreshSignInNotice();
            RefreshCharacterNotice();
        }

        private void RefreshAnnNotice()
        {
            annNotice.SetActive(!PlayerSandbox.Instance.TemporaryHandler.AnnOpened);
        }

        private void RefreshTaskNotice()
        {
            dailyNotice.SetActive(
                PlayerSandbox.Instance.ObjectiveHandler.Daily.GetCanClaimNumber() > 0 
                || PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.CanClaim(PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex));
        }

        private void RefreshSignInNotice()
        {
            signinNotice.SetActive(PlayerSandbox.Instance.ObjectiveHandler.CheckIn.CanSign());
        }

        private void RefreshCharacterNotice()
        {
            characterNotice.SetActive(CharacterAPI.CanUpgrade(PlayerSandbox.Instance.CharacterHandler.CharacterId));
        }

        #endregion

        #region API

        public static UIActivityLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIActivityLayer>(nameof(UIActivityLayer));
            Debug.Assert(layer != null);
            return layer;
        }

        public static void ShowUIActivityLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIActivityLayer>("UIActivityLayer");
            Assert.IsNotNull(layer);
            layer.ShowLayer();
        }

        public static void HideUIActivityLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIActivityLayer>("UIActivityLayer");
            Assert.IsNotNull(layer);
            layer.Hide();
        }

        public void ActiveUpgradeCharacterFinger(bool active = true)
        {
            fingerUpgradeCharacter.SetActive(active);
        }

        #endregion

        #region Callbacks

        private void OnClickAnnouncement(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            UIManager.Instance.GetLayer("UIAnnLayer")?.Show();
            PlayerSandbox.Instance.TemporaryHandler.AnnOpened = true;
        }

        private void OnClickTask(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            UITaskLayer.GetLayer().Show();
            // UIObjectiveLayer.ShowLayer();
            // UIManager.Instance.GetLayer("UIBlackScreen").Show();
            // CoroutineTaskManager.Instance.WaitSecondTodo(UITaskLayer.GetLayer().Show, 1f);
        }

        private void OnClickSign(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            UISignInLayer.GetLayer().Show();
        }

        private void OnClickBuildArea(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            if (IsProcessing) return;
            IsProcessing = true;
            Hide();
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName("AreaSelection", 3f);
            SceneManager.Instance.StartLoad();
        }

        private void OnClickTeleport(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            if (IsProcessing) return;
            IsProcessing = true;
            Hide();
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName("ChessSelection", 3f);
            SceneManager.Instance.StartLoad();
        }

        private void OnClickCharacter(UIBasicButton sender)
        {
            if (GameInstance.Instance.HostingHandler.Hosting) return;
            if (IsProcessing) return;
            IsProcessing = true;
            
            if (TutorialHandler.IsTriggerable(TutorialHandler.FirstUpgradeCharacterKey) && PlayerSandbox.Instance.CharacterHandler.Coin >= 30000)
            {
                ActiveUpgradeCharacterFinger(false);
            }
            
            Hide();
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName("CharacterSelection", 3f);
            SceneManager.Instance.StartLoad();
        }

        private void OnPlayerCoinChanged(int? preValue, int newValue)
        {
            RefreshCharacterNotice();
        }

        private void OnAnnOpenedChanged(bool preValue, bool newValue)
        {
            RefreshAnnNotice();
        }
        
        private void OnObjectiveDailyChanged(FObjectiveDaily preValue, FObjectiveDaily newValue)
        {
            RefreshTaskNotice();
        }

        private void OnObjectiveCheckInChanged(FObjectiveCheckIn preValue, FObjectiveCheckIn newValue)
        {
            RefreshSignInNotice();
        }

        private void OnPaymentRewardIndexChanged(int? arg1, int arg2)
        {
            RefreshTaskNotice();
        }
        
        #endregion
    }

}