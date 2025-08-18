using System;
using System.Collections;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class CameraMoveEnableComponent : UIComponent
    {
        private ChessGameMode GameMode { get; set; }
        private PlayerCameraController PlayerCameraControllerRef { get; set; }
        
        public override void OnShow()
        {
            if (!GameMode)
            {
                GameMode = ChessGameMode.GetGameMode<ChessGameMode>(ChessGameMode.WorldObjectRegisterKey);
            }

            if (GameMode && !PlayerCameraControllerRef)
            {
                PlayerCameraControllerRef = GameMode.GetComponent<PlayerCameraController>();
            }

            if (!GameMode || !PlayerCameraControllerRef) return;
            PlayerCameraControllerRef.SetControllerEnable(false);
            // PlayerCameraControllerRef.SetOverrideTarget(null);
            // PlayerCameraControllerRef.SetupCamera();
        }

        public override void OnHide()
        {
            if(!PlayerCameraControllerRef) return;
            PlayerCameraControllerRef.SetControllerEnable(true);
        }
    }
}