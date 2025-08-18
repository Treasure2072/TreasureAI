using System;
using System.Collections;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class PlayerCamera : DragonLiCameraTopdown
    {
        #region Defines

        public enum ECameraAnimationType
        {
            Welcome,
            AroundChessBoard,
            FocusLand,
        }

        #endregion

        #region Fields

        [Header("Settings")] 
        public Transform AnimationTargetRef;
        
        #endregion

        #region Properties

        private PhysicsRaycaster Raycaster { get; set; }

        #endregion
        
        #region DragonLiCameraTopdown

        protected override void OnAwake()
        {
            base.OnAwake();

            Raycaster = GetComponent<PhysicsRaycaster>();
            // CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            // {
            //     StartAnimation(ECameraAnimationType.Welcome);
            // }, 3.0f);
        }

        #endregion
        
        #region API

        public void StartAnimation(ECameraAnimationType type)
        {
            // this.LogEditorOnly(type);
            // Only works in single player
            switch (type)
            {
                case ECameraAnimationType.Welcome:
                    StartCoroutine(ProcessWelcomeAnimation());
                    break;
                case ECameraAnimationType.AroundChessBoard:
                    break;
                case ECameraAnimationType.FocusLand:
                    StartCoroutine(ProcessFocusLandAnimation());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void SetBuildingTouchEnable(bool bEnable)
        {
            Raycaster.enabled = bEnable;
        }
        
        #endregion

        #region Coroutines

        private IEnumerator ProcessWelcomeAnimation()
        {
            SetOverrideTarget(AnimationTargetRef);

            var centerLoc = World.GetRegisteredObject("central-map-center").transform.position;
            
            AnimationTargetRef.position = centerLoc + Vector3.up * 300.0f;
            AnimationTargetRef.DOMove(centerLoc, 3.8f);
            yield return new WaitForSeconds(3.8f);
            
            // SetOverrideTarget(World.GetPlayer());
        }
        
        private IEnumerator ProcessFocusLandAnimation()
        {
            // AnimationController.gameObject.SetActive(true);
            // AnimationController.SetTrigger(AK_FocusLand);
            // SetOverrideTarget(AnimationTargetRef);
            // yield return new WaitForSeconds(7.2f);
            // AnimationController.gameObject.SetActive(false);
            // SetOverrideTarget(World.GetPlayer());
            yield return null;
        }

        #endregion
    }
}


