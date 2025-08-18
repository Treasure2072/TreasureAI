using System.Collections.Generic;
using _Scripts.UI.Common;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Game
{
    public class UIBuildingAreaSelectionLayer : UILayer
    {
        #region Define

        private static readonly int AnimHashAreaIndex = Animator.StringToHash("AreaIndex");
        private const string kCameraAnimatorWorldKey = "Camera-Animator";

        #endregion
        
        #region Fields

        [Header("References")]
        [SerializeField] private BuildingCardContainer container;

        #endregion
        
        #region Properties
        
        private int Count { get; set; }
        private Dictionary<int, BuildAreaSlot> BuildAreaSlotsData  { get; set; }
        private Animator CameraAnimator { get; set; }
        
        private BuildingAreaCard SelectedBuildingArea { get; set; }
        
        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnBack"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnBackClick);
            
            var cameraAnimatedRoot = World.GetRegisteredObject(kCameraAnimatorWorldKey);
            Debug.Assert(cameraAnimatedRoot != null, "CameraAnimatedRoot != null");
            CameraAnimator = cameraAnimatedRoot.GetComponent<Animator>();
        }

        protected override void OnShow()
        {
            base.OnShow();
            UIStaticsLayer.ShowUIStaticsLayer();
        }

        protected override void OnHide()
        {
            base.OnHide();
            UIStaticsLayer.HideUIStaticsLayer();
        }

        #endregion

        #region Function

        private void ShowLayer(Dictionary<int, BuildAreaSlot> buildAreaSlots, int count)
        {
            Count = count;
            BuildAreaSlotsData = buildAreaSlots;
            container.RecycleAllGrids(null, true);
            container.SpawnAllGrids(BuildAreaSlotsData, (UnityAction<BuildingAreaCard>)OnSelectArea, Count);
            Show();
        }

        private BuildingAreaGameMode GetGameMode()
        {
            return World.GetRegisteredObject<BuildingAreaGameMode>(BuildingAreaGameMode.WorldObjectRegisterKey);
        }

        private void MoveCamera(int index)
        {
            CameraAnimator?.SetInteger(AnimHashAreaIndex, index);
        }

        private void LandBlink(int index)
        {
            var gameMode = GetGameMode();
            var buildingArea = gameMode?.GetBuildingArea(index);
            if (!buildingArea) return;
            buildingArea.Blink();
        }
        
        #endregion

        #region API

        public static BuildingCardContainer GetContainer()
        {
            var layer = UIManager.Instance.GetLayer<UIBuildingAreaSelectionLayer>("UIBuildingAreaSelectionLayer");
            Assert.IsNotNull(layer);
            return layer.container;
        }

        public static void ShowPreLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIBuildingAreaSelectionLayer>("UIBuildingAreaSelectionLayer");
            Assert.IsNotNull(layer);
            layer.Show();
        }

        public static void ShowUIBuildingAreaSelectionLayer(Dictionary<int, BuildAreaSlot> buildAreaSlots, int count)
        {
            var layer = UIManager.Instance.GetLayer<UIBuildingAreaSelectionLayer>("UIBuildingAreaSelectionLayer");
            Assert.IsNotNull(layer);
            layer.ShowLayer(buildAreaSlots, count);
        }

        public static void HideUIBuildingAreaSelectionLayer()
        {
            var layer = UIManager.Instance.GetLayer("UIBuildingAreaSelectionLayer");
            Assert.IsNotNull(layer);
            layer.Hide();
        }

        #endregion

        #region Callback

        private void OnSelectArea(BuildingAreaCard card)
        {
            if (SelectedBuildingArea)
            {
                SelectedBuildingArea.ClearWorldElements();
            }
            if (card == SelectedBuildingArea) return;
            SelectedBuildingArea = card;
            card.transform.DOPunchScale(Vector3.one * 0.1f, 0.15f, 0);
            var index = card.GetIndex();
            MoveCamera(index);
            LandBlink(index);
        }

        private void OnBackClick(UIBasicButton sender)
        {
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName("AreaSelection", 2);
            SceneManager.Instance.StartLoad();
        }

        #endregion
    }
}