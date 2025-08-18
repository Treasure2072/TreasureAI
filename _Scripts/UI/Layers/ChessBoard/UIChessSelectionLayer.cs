using _Scripts.UI.Common;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class UIChessSelectionLayer : UILayer
    {

        #region Define

        private static readonly int AnimHashBlockIndex = Animator.StringToHash("SceneIndex");
        private const string kCameraAnimatorWorldKey = "Camera-Animator";

        #endregion

        #region Properties
        private Animator CameraAnimator { get; set; }

        private int _index;

        private int Index
        {
            get { return _index; }
            set
            {
                if(_index == value) return;
                OnSelectChanged(_index, value);
                _index = value;
            }
        }
        
        private bool IsProcessing { get; set; }

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
            this["BtnLock"].As<UIBasicButton>().OnClickEvent.AddListener(OnLocked);
            this["BtnGo"].As<UIBasicButton>().OnClickEvent?.AddListener(OnGoClicked);
            this["BtnPrev"].gameObject.SetActive(false);
        }

        protected override void OnShow()
        {
            base.OnShow();
            IsProcessing = false;
            UIStaticsLayer.ShowUIStaticsLayer();
        }

        protected override void OnHide()
        {
            base.OnHide();
            UIStaticsLayer.HideUIStaticsLayer();
        }

        #endregion

        #region Function

        private void SetLockStatus(bool lockStatus)
        {
            this["BtnGo"].gameObject.SetActive(!lockStatus);
            this["BtnLock"].gameObject.SetActive(lockStatus);
        }

        #endregion

        #region Callbacks

        private void OnPrevButtonPressed(UIBasicButton sender)
        {
            if (IsProcessing) return;
            IsProcessing = true;
            CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                IsProcessing = false;
            }, 0.5f);
            Index--;
            CameraAnimator?.SetInteger(AnimHashBlockIndex, Index);
            sender.gameObject.SetActive(Index > 0);
            this["BtnNext"].gameObject.SetActive(true);
        }

        private void OnNextButtonPressed(UIBasicButton sender)
        {
            if (IsProcessing) return;
            IsProcessing = true;
            CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                IsProcessing = false;
            }, 0.5f);
            
            Index++;
            sender.gameObject.SetActive(Index < ChessBoardAPI.GetChessboardRouter().Count - 1);
            this["BtnPrev"].gameObject.SetActive(true);
        }

        private void OnSelectChanged(int preSelected, int nextSelected)
        {
            CameraAnimator?.SetInteger(AnimHashBlockIndex, nextSelected);
            SetLockStatus(nextSelected != 0);
        }
        
        private void OnBackButtonPressed(UIBasicButton sender)
        {
            if (IsProcessing) return;
            IsProcessing = true;
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName(ChessBoardAPI.GetCurrentChessBoard(), 1, true);
            SceneManager.Instance.StartLoad();
        }

        private void OnLocked(UIBasicButton sender)
        {
            UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                this.GetLocalizedText("function-not-available"));
        }

        private void OnGoClicked(UIBasicButton sender)
        {
            // TODO : 暂时只开放第一个棋盘
            if (Index != 0)
            {
                UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                    this.GetLocalizedText("function-not-available"));
                return;
            }
            
            if (IsProcessing) return;
            IsProcessing = true;
            Hide();
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            if (PlayerSandbox.Instance.CharacterHandler.ChessboardId != Index)
            {
                GameSessionAPI.CharacterAPI.SetChessboard(Index);
                PlayerSandbox.Instance.CharacterHandler.ChessboardId = Index;
            }
            SceneManager.Instance.AddSceneToLoadQueueByName(ChessBoardAPI.GetCurrentChessBoard(), 3);
            SceneManager.Instance.StartLoad();
        }

        #endregion
    }
}


