using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerCameraController : MonoBehaviour
    {
        #region Define

        public delegate RaycastHit SelectHitResultDelegate(RaycastHit[] hits, int size);

        #endregion
        
        #region Fields

        [Header("Camera Control")] 
        // public float SmoothRate = 16f;
        public float ScrollSensitivity = 2.5f;
        public float MoveSensitivity = 5.0f;
        public float DefaultDistance = 150.0f;
        // public float MinDistance = 100.0f;
        // public float MaxDistance = 200.0f;
        public float MaxOffsetDistance = 20.0f;

        [DisplayAs("Clear Target")] public bool bClearTarget;
        [DisplayAs("Reset Offset")] public bool bResetOffset;
        [DisplayAs("Limit Offset Distance")] public bool bLimitOffsetOffset;
        [DisplayAs("Free View")] public bool bFreeView;
        public LayerMask FreeViewLayer;
        public LayerMask SelectObjectLayer;

        [DisplayAs("Override Rotation")]
        public bool bOverrideRotation = false;
        public Vector3 Rotation;
        public float TransitionDuring = 3.0f;

        [DisplayAs("User Camera Rotation")]
        public bool bUserCameraRotation = false;

        [Header("Events")] 
        public UnityEvent<bool, GameObject> OnCameraSelectEvent = new ();
        public UnityEvent<GameObject> OnCameraClick = new();

        #endregion

        #region Propertoes

        private SelectHitResultDelegate SelectHitResultFunction { get; set; }

        private GameInput InputConfig { get; set; }
        public PlayerCamera CameraRef { get; set; }
        private bool bDisableCameraMoving {get; set;}
        private bool bCameraMoving { get; set; }
        private bool bMobileZooming { get; set; }
        private Coroutine MobileTwoFingerContactCoroutine { get; set; }
        private float CurrentRotationYawValue { get; set; }
        private bool bAdjustingRootRotation { get; set; }

        public static bool bGlobalDisableInput{ get; set;}
        private static RaycastHit[] ScreenSelectCache { get; set; }

        private Vector2 StartMoveScreenPos { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            ScreenSelectCache = new RaycastHit[5];
        }

        private void OnDestroy()
        {
            // Do this to prevent some bugs...
            // ...
            OnCameraSelectEvent.RemoveAllListeners();
            OnCameraSelectEvent = null;
        }

        #endregion
        
        #region Functions

        public void SetupController(SelectHitResultDelegate selectFunc = null)
        {
            CameraRef = World.GetMainCamera().GetComponent<PlayerCamera>();
            SelectHitResultFunction = selectFunc;
            SetupCameraInput();
        }

        public void SetControllerEnable(bool bEnable)
        {
            if (bEnable)
            {
                InputConfig.Enable();
                SetupCamera();
            }
            else
            {
                InputConfig.Disable();
                if (MobileTwoFingerContactCoroutine != null)
                {
                    StopCoroutine(MobileTwoFingerContactCoroutine);
                    MobileTwoFingerContactCoroutine = null;
                }
            }

            bCameraMoving = false;
        }

        public void SetInputEnable(bool bEnable) 
        {
            if (bEnable)
            {
                InputConfig.Enable();
            }
            else
            {
                InputConfig.Disable();
                if (MobileTwoFingerContactCoroutine == null) return;
                StopCoroutine(MobileTwoFingerContactCoroutine);
                MobileTwoFingerContactCoroutine = null;
            }
        }
        
        public void SetOffset(Vector3 offset)
        {
            CameraRef.SetOffset(offset, false, 0);
        }
        
        public void ResetOffset()
        {
            CameraRef.SetOffset(Vector3.zero, false, 0);
        }

        public void SetOverrideTarget(Transform target)
        {
            CameraRef.SetOverrideTarget(target);
        }

        public RaycastHit GetScreenRaycastResult(LayerMask layer, bool bForceDefault = false)
        {
            var screenLocation = GetScreenLocation();
            var castRay = CameraRef.CameraComponent.ScreenPointToRay(screenLocation);
            var size = Physics.RaycastNonAlloc(castRay.origin, castRay.direction, ScreenSelectCache, 5000.0f, layer, QueryTriggerInteraction.Collide);
            if (size == 0) return default;
            if (bForceDefault) return ScreenSelectCache[0];
            return SelectHitResultFunction?.Invoke(ScreenSelectCache, size) ?? ScreenSelectCache[0];
        }

        public Ray GetScreenRay()
        {
            var screenLocation = GetScreenLocation();
            return CameraRef.CameraComponent.ScreenPointToRay(screenLocation);
        }

        public void SetDisableCameraMoving(bool bDisable) 
        {
            bDisableCameraMoving = bDisable;
        }

        private Vector2 GetScreenLocation()
        {
#if UNITY_STANDALONE
            return Mouse.current.position.ReadValue();
#else
            return InputConfig.CameraControl.ScreenFinger01.ReadValue<Vector2>();
#endif
        }
        
        private GameObject SelectObject(LayerMask layer)
        {
            var result = GetScreenRaycastResult(layer);
            return result.collider == null ? null : result.transform.gameObject;
        }

        private void SetupCameraInput()
        {
            InputConfig = new GameInput();
            InputConfig.CameraControl.Move.performed += OnCameraMove;
            // InputConfig.CameraControl.Scroll.performed += OnCameraScroll;
            InputConfig.CameraControl.MoveToggle.started += OnCameraStartMove;
            InputConfig.CameraControl.MoveToggle.canceled += OnCameraStopMove;

            InputConfig.CameraControl.Select.started += OnCameraSelect;
            InputConfig.CameraControl.Select.canceled += OnCameraSelect;

            // InputConfig.CameraControl.TwoFingerContact.started += OnMobileTwoFingerContact;
            // InputConfig.CameraControl.TwoFingerContact.canceled += OnMobileTwoFingerContact;
        }

        public void SetupCamera()
        {
            bAdjustingRootRotation = false;
            if (bClearTarget)
            {
                CameraRef.SetOverrideTarget(null);
            }

            // CameraRef.SmoothRate = SmoothRate;
            CameraRef.SetLimitOffsetDistance(bLimitOffsetOffset, MaxOffsetDistance);
            if (bResetOffset)
            {
                CameraRef.SetOffset(Vector3.zero, false, 0);
            }

            // CameraRef.DistanceMax = MaxDistance;
            // CameraRef.DistanceMin = MinDistance;
            CameraRef.SetDistance(DefaultDistance);

            bCameraMoving = false;
            CurrentRotationYawValue = CameraRef.transform.localRotation.eulerAngles.y;
            
            if (!bOverrideRotation) return;
            bAdjustingRootRotation = true;
            var cameraTrans = CameraRef.transform;
            cameraTrans.DOKill();
            cameraTrans.DOLocalRotate(Rotation, TransitionDuring).SetEase(Ease.InOutCubic).onComplete += () =>
            {
                bAdjustingRootRotation = false;
            };
        }

        private IEnumerator MobileTwoFingerContact()
        {
            var finger01 = InputConfig.CameraControl.ScreenFinger01;
            var finger02 = InputConfig.CameraControl.ScreenFinger02;
            Vector2 CalculateRotationDirection()
            {
                var direction = finger02.ReadValue<Vector2>() - finger01.ReadValue<Vector2>();
                return direction;
            }

            float CalculateDistance()
            {
                return Vector2.Distance(finger01.ReadValue<Vector2>(), finger02.ReadValue<Vector2>());
            }
            
            var originDistance = CameraRef.GetTargetDistance();
            var distanceStart = CalculateDistance();
            var directionStart = CalculateRotationDirection();
            var originLerpValue = CurrentRotationYawValue;
            
            while (true)
            {
                // Distance
                var deltaDistance = distanceStart - CalculateDistance();
                CameraRef.SetDistance(originDistance + deltaDistance * ScrollSensitivity * 0.05f);
                
                // Rotation
                if (bUserCameraRotation && !bAdjustingRootRotation)
                {
                    // Calculate degree next frame
                    // ...
                    var currentDirection = CalculateRotationDirection();
                    var deltaAngle = Vector2.SignedAngle(directionStart, currentDirection);
                    CurrentRotationYawValue = originLerpValue + deltaAngle * 2.0f;
                    CurrentRotationYawValue = MathExtension.NormalizeDegree(CurrentRotationYawValue);

                    // Apply the rotation
                    // ...
                    var cameraTrans = CameraRef.transform;
                    var rotationBefore = cameraTrans.localRotation;
                    var targetRotation = rotationBefore.eulerAngles;
                    targetRotation.y = CurrentRotationYawValue;
                    cameraTrans.localRotation = Quaternion.Lerp(rotationBefore, Quaternion.Euler(targetRotation), 16 * Time.deltaTime);
                }
                
                yield return null;
            }
            
            // ReSharper disable once IteratorNeverReturns
        }

        #endregion

        #region Callbacks

        private void OnCameraMove(InputAction.CallbackContext context)
        {
            if (!bCameraMoving || bMobileZooming || bDisableCameraMoving || bGlobalDisableInput || !CameraRef) return;
            var offset = -context.ReadValue<Vector2>() * MoveSensitivity;
#if (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
            offset *= 0.025f;
#endif
            var cameraTrans = CameraRef.transform;
            CameraRef.AddOffset(cameraTrans.forward.ReflectVectorXOZ().normalized * offset.y +
                                cameraTrans.right.ReflectVectorXOZ().normalized * offset.x, bFreeView, FreeViewLayer);
        }

        private void OnCameraScroll(InputAction.CallbackContext context)
        {
            if(bGlobalDisableInput) return;

            var scrollValue = context.ReadValue<float>() * ScrollSensitivity;
            CameraRef.AddDistance(scrollValue);
        }

        private void OnCameraStartMove(InputAction.CallbackContext context)
        {
            if(bGlobalDisableInput) return;
            bCameraMoving = true;
            StartMoveScreenPos = InputConfig.CameraControl.ScreenFinger01.ReadValue<Vector2>();
        }

        private void OnCameraStopMove(InputAction.CallbackContext context)
        {
            bCameraMoving = false;
        }
        
        private void OnCameraSelect(InputAction.CallbackContext context)
        {
            if(bGlobalDisableInput) return;
            // if (EventSystem.current.IsPointerOverGameObject()) return;
            if (context.started)
            {
                OnCameraSelectEvent?.Invoke(true, SelectObject(SelectObjectLayer));
            }
            else
            {
                OnCameraSelectEvent?.Invoke(false, null);
                var distance = Vector2.Distance(StartMoveScreenPos,
                    InputConfig.CameraControl.ScreenFinger01.ReadValue<Vector2>());
                if (distance < 50.0f)
                {
                    OnCameraClick?.Invoke(SelectObject(SelectObjectLayer));
                }
            }
        }

        private void OnMobileTwoFingerContact(InputAction.CallbackContext context)
        {
            if(bGlobalDisableInput) return;
            bMobileZooming = context.started;
            if (bMobileZooming)
            {
                MobileTwoFingerContactCoroutine = StartCoroutine(MobileTwoFingerContact());
            }
            else
            {
                if (MobileTwoFingerContactCoroutine == null) return;
                StopCoroutine(MobileTwoFingerContactCoroutine);
                MobileTwoFingerContactCoroutine = null;
            }
        }
        
        #endregion

    }
}