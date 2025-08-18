using System;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    public class CameraGroup : MonoBehaviour
    {
        #region Properties

        public const string CameraGroupKey = "CameraGroup";
        
        [Header("Effect Camera")] 
        [SerializeField] private Transform effectRootNode;
        [SerializeField] private Transform modeRootNode;
        [SerializeField] private SpriteRenderer effectDarkScreen;
        [SerializeField] private Camera effectCamera;

        private bool IsEffectCameraActive { get; set; }
        
        #endregion

        #region API

        public static CameraGroup GetCameraGroup()
        {
            var cameraGroupObject = World.GetRegisteredObject(CameraGroup.CameraGroupKey);
            Debug.Assert(cameraGroupObject, "CameraGroupObject != null");
            
            var cameraGroup = cameraGroupObject.GetComponent<CameraGroup>();
            Debug.Assert(cameraGroup, "CameraGroup != null");
            return cameraGroup;
        }

        public float EnterEffectMode()
        {
            if (IsEffectCameraActive)
            {
                return 0.0f;
            }
            
            IsEffectCameraActive = true;
            // effectCamera.enabled = true;
            effectDarkScreen.gameObject.SetActive(true);
            effectDarkScreen.DOFade(0.8f, 0.5f);
            return 0.5f;
        }

        public void ExitEffectMode()
        {
            if (!IsEffectCameraActive)
            {
                return;
            }
            
            IsEffectCameraActive = false;
            effectDarkScreen.DOFade(0.0f, 0.5f).onComplete = () =>
            {
                effectDarkScreen.gameObject.SetActive(false);
                // effectCamera.enabled = false;
            };
        }
        
        public DummyPoolObject PlayEffect(GameObject effect)
        {
            var effectInstance = SpawnManager.Instance.GetObjectFromPool(effect);
            Debug.Assert(effectInstance != null, "effectInstance != null");
            var effectTrans = effectInstance.GetGameObject().transform;
            effectTrans.SetParent(effectRootNode);
            effectTrans.localPosition = Vector3.zero;
            effectTrans.localRotation = Quaternion.identity;
            effectTrans.localScale = Vector3.one;

            var poolObject = effectTrans.GetComponent<DummyPoolObject>();
            Debug.Assert(poolObject, "poolObject != null");
            return poolObject;
        }

        public float PlayModel(GameObject model)
        {
            var effectInstance = SpawnManager.Instance.GetObjectFromPool(model);
            Debug.Assert(effectInstance != null, "effectInstance != null");
            var effectTrans = effectInstance.GetGameObject().transform;
            effectTrans.SetParent(modeRootNode);
            effectTrans.localPosition = Vector3.zero;
            effectTrans.localRotation = Quaternion.identity;
            effectTrans.localScale = Vector3.one;

            var poolObject = effectTrans.GetComponent<DummyPoolObject>();
            Debug.Assert(poolObject, "poolObject != null");
            return poolObject.LifeTime;
        }

        #endregion
    }
    
    [Serializable]
    public class OpenEffectCameraEvent : EffectCameraEvent
    {
        private float FinishTime { get; set; }
        public override void OnExecute()
        {
            FinishTime = Time.time + GetCameraGroup().EnterEffectMode();
        }

        public override bool OnTick()
        {
            return FinishTime < Time.time;
        }
    }
    
    [Serializable]
    public class CloseEffectCameraEvent : EffectCameraEvent
    {
        public override void OnExecute()
        {
            GetCameraGroup().ExitEffectMode();
        }
    }

    public class PlayVfxCoinDiceTokenEvent : EffectCameraEvent
    {
        private Func<GameObject> GetEffectPrefab { get; set; }
        private Func<int> GetCoin { get; set; }
        private Func<int> GetDice { get; set; }
        private Func<float> GetToken { get; set; }
        
        private float FinishTime { get; set; }
        public PlayVfxCoinDiceTokenEvent(GameObject prefab, Func<int> getCoin, Func<int> getDice, Func<float> getToken)
        {
            GetEffectPrefab = () => prefab;
            GetCoin = getCoin;
            GetDice = getDice;
            GetToken = getToken;
        }
        
        public PlayVfxCoinDiceTokenEvent(Func<GameObject> prefab, Func<int> getCoin, Func<int> getDice, Func<float> getToken)
        {
            GetEffectPrefab = prefab;
            GetCoin = getCoin;
            GetDice = getDice;
            GetToken = getToken;
        }

        public override void OnExecute()
        {
            base.OnExecute();
            var prefab = GetEffectPrefab();
            var coin = GetCoin();
            var dice = GetDice();
            var token = GetToken();
            if (!prefab || (coin <= 0 && dice <= 0 && token <= 0)) return;

            var pool = GetCameraGroup().PlayEffect(prefab);
            FinishTime = Time.time + pool.LifeTime;
            
            var vfxCoinDiceToken = pool as VFXCoinDiceToken2D;
            if (vfxCoinDiceToken)
            {
                vfxCoinDiceToken.Play(coin / 500, dice, (int)(token * 10));
            }
        }
        
        public override bool OnTick()
        {
            return FinishTime < Time.time;
        }
    }
    
    [Serializable]
    public class PlayFullscreenEffectEvent : EffectCameraEvent
    {
        private Func<GameObject> GetEffectPrefab { get; set; }
        private float FinishTime { get; set; }
        public PlayFullscreenEffectEvent(GameObject prefab)
        {
            GetEffectPrefab = () => prefab;
        }
        public  PlayFullscreenEffectEvent(Func<GameObject> getPrefab)
        {
            GetEffectPrefab = getPrefab;
        }
        public override void OnExecute()
        {
            var prefab = GetEffectPrefab();
            if (!prefab)
            {
                // this.LogEditorOnly($"Effect prefab is null!");
                return;
            }
            FinishTime = Time.time + GetCameraGroup().PlayEffect(prefab).LifeTime;
        }
        public override bool OnTick()
        {
            return FinishTime < Time.time;
        }
    }
    
    [Serializable]
    public abstract class EffectCameraEvent : IQueueableEvent
    {
        public virtual void OnQueue() {}
        public virtual void OnExecute() {}
        public virtual void OnDequeue() {}
        public virtual void OnCancel() {}
        public virtual bool OnTick()
        {
            return true;
        }
        public virtual void OnFinish() {}

        protected CameraGroup GetCameraGroup()
        {
            // var cameraGroupObject = World.GetRegisteredObject(CameraGroup.CameraGroupKey);
            // Debug.Assert(cameraGroupObject, "CameraGroupObject != null");
            //
            // var cameraGroup = cameraGroupObject.GetComponent<CameraGroup>();
            // Debug.Assert(cameraGroup, "CameraGroup != null");
            return CameraGroup.GetCameraGroup();
        }
    }
}


