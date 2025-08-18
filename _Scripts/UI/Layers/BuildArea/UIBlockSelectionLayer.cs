using System;
using _Scripts.UI.Common;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UIBlockSelectionLayer : UILayer, IMessageReceiver
    {
        #region Properties

        private static readonly int AnimHashBlockIndex = Animator.StringToHash("BlockIndex");
        private const string kCameraAnimatorWorldKey = "Camera-Animator";

        [Header("Settings")] 
        [SerializeField] private Sprite lockSprite;
        [SerializeField] private Sprite enterSprite;
        [SerializeField] private Image enterButtonImage;
        [SerializeField] private string[] sceneAssets = {
            "TheWorld01",
            "TheWorld02",
            "TheWorld03",
        };

        private Animator CameraAnimator { get; set; }
        private int Index { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
        }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            Index = 0;
            var cameraAnimatedRoot = World.GetRegisteredObject(kCameraAnimatorWorldKey);
            Debug.Assert(cameraAnimatedRoot != null, "CameraAnimatedRoot != null");
            CameraAnimator = cameraAnimatedRoot.GetComponent<Animator>();
            
            this["BtnNext"].As<UIBasicButton>().OnClickEvent?.AddListener(OnNextButtonPressed);
            this["BtnPrev"].As<UIBasicButton>().OnClickEvent?.AddListener(OnPrevButtonPressed);
            this["BtnBack"].As<UIBasicButton>().OnClickEvent?.AddListener(OnBackButtonPressed);
            this["BtnGo"].As<UIBasicButton>().OnClickEvent?.AddListener(OnGoClicked);
            this["BtnPrev"].gameObject.SetActive(false);
        }

        protected override void OnShow()
        {
            base.OnShow();
            UIStaticsLayer.ShowUIStaticsLayer();
            UpdateButton(Index + 1);
        }

        protected override void OnHide()
        {
            base.OnHide();
            UIStaticsLayer.HideUIStaticsLayer();
        }

        #endregion

        #region Functions

        private void EnterArea(int area)
        {
            Hide();
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName(sceneAssets[area], 1);
            SceneManager.Instance.StartLoad();
        }

        private void UpdateButton(int area)
        {
            var unlocked = PlayerSandbox.Instance.BuildingAreaHandler.BuildAreas.IsUnlocked(area);
            enterButtonImage.sprite = unlocked ? enterSprite : lockSprite;
        }
        
        #endregion
        
        #region Callbacks

        private void OnPrevButtonPressed(UIBasicButton sender)
        {
            Index--;
            CameraAnimator?.SetInteger(AnimHashBlockIndex, Index);
            
            sender.gameObject.SetActive(Index > 0);
            this["BtnNext"].gameObject.SetActive(true);
            
            UpdateButton(Index + 1);
        }

        private void OnNextButtonPressed(UIBasicButton sender)
        {
            Index++;
            CameraAnimator?.SetInteger(AnimHashBlockIndex, Index);
            
            sender.gameObject.SetActive(Index < sceneAssets.Length - 1);
            this["BtnPrev"].gameObject.SetActive(true);
            
            UpdateButton(Index + 1);
        }
        
        private void OnBackButtonPressed(UIBasicButton sender)
        {
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName(ChessBoardAPI.GetCurrentChessBoard(), 1, true);
            SceneManager.Instance.StartLoad();
        }
        
        private void OnGoClicked(UIBasicButton sender)
        {

            UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                this.GetLocalizedText("function-not-available"));
            
            return;
            var id = Index + 1;
            if (!PlayerSandbox.Instance.BuildingAreaHandler.BuildAreas.IsUnlocked(id))
            {
                if (id == 1)
                {
                    GameSessionAPI.BuildAreaAPI.UnlockArea(id);
                    return;
                }

                // TODO: 测试版本暂时不需要这个判断，后面加入弹窗提示
                // ...
                // if (!PlayerSandbox.Instance.BuildArea.IsUnlocked(id - 1)) return;
                GameSessionAPI.BuildAreaAPI.UnlockArea(id);
                return;
            }
            
            EnterArea(Index);
        }
        
        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess()) return;
            if (service != "build" || method != "unlock_area") return;
            
            var id = response.GetAttachmentAsInt("area");
            PlayerSandbox.Instance.BuildingAreaHandler.BuildAreas.Unlock(id);
            EnterArea(Index);
        }

        #endregion
    }
}


