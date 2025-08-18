using System;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game
{
    public class ScratchObject : MonoBehaviour, IPointerClickHandler
    {
        #region Fields

        [Header("References")]
        [SerializeField] private Transform door;
        [SerializeField] private Transform itemTransform;

        #endregion

        #region Properties
        
        private bool Opened { get; set; }

        public UnityEvent<ScratchObject> OnUsedOperated { get; set; } = new();

        #endregion

        #region Unity

        private void Awake()
        {
            Opened = false;
        }

        #endregion

        #region Function - Interface

        public void OnPointerClick(PointerEventData eventData)
        {
            OnUsedOperated?.Invoke(this);
            OnUsedOperated?.RemoveAllListeners();
            Opened = true;
        }

        #endregion

        #region API
        
        public void FlyDoor()
        {
            var target = World.GetRegisteredObject("MoveTarget").transform.position;
            door.DOMove(target, 0.3f)
                .SetEase(Ease.InOutCubic);
            door.DORotate(new Vector3(-75, 0, -22), 0.3f)
                .SetEase(Ease.InOutCubic);
            door.DOScale(Vector3.one * 70f, 0.3f)
                .SetEase(Ease.InOutCubic);
        }

        public void SetContent(GameObject itemPrefab)
        {
            var obj = Instantiate(itemPrefab, itemTransform);
            obj.transform.localPosition = Vector3.zero;
        }

        public void SetEffect(GameObject effectPrefab)
        {
            var iPool = SpawnManager.Instance.GetObjectFromPool(effectPrefab);
            var obj = iPool.GetGameObject();
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.forward * 0.02f;
        }

        public bool IsOpened()
        {
            return Opened;
        }

        #endregion
    }
}