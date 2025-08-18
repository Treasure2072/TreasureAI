using System;
using System.Collections.Generic;
using DragonLi.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.UI.Common.Grids
{
    [RequireComponent(typeof(DummyPoolObject))]
    public class GridBase : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private Transform ElementRoot;

        private ElementBase ElementPrefab { get; set; }
        
        private object[] Args { get; set; }
        
        private ElementBase Element { get; set; }
        
        protected virtual void OnInitialized() { }
        
        protected virtual void OnRecycle() {  }

        protected virtual T SpawnElement<T>() where T : ElementBase
        {
            if (!ElementPrefab)
            {
                this.LogEditorOnly($"No element prefab assigned to {nameof(GridBase)}");
                return null;
            }
            var interfacePool = SpawnManager.Instance.GetObjectFromPool(ElementPrefab.gameObject);
            var elementObj = interfacePool.GetGameObject();
            if (!elementObj)
            {
                throw new NullReferenceException($"elementObj is null");
            }
            
            elementObj.transform.SetParent(ElementRoot, false);
            elementObj.transform.localPosition = Vector3.zero;
            elementObj.transform.localScale = Vector3.one;
            elementObj.transform.localRotation = Quaternion.identity;
            
            var element = elementObj.GetComponent<T>();
            if (!element)
            {
                throw new NullReferenceException($"element is null");
            }
            
            element.Initialized();
            return element;
        }

        public void Initialize()
        {
            OnInitialized();
        }

        public void Recycle()
        {
            OnRecycle();
        }

        public void SetElementPrefab(ElementBase elementPrefab)
        {
            ElementPrefab = elementPrefab;
        }

        public virtual void SetGrid(params object[] args)
        {
            SetGrid<ElementBase>(args);
        }
        
        public virtual void SetGrid<T>(params object[] args) where T : ElementBase
        {
            Args = args;
            if (!ElementPrefab)
            {
                this.LogEditorOnly($"Element prefab is null");
                return;
            }

            Element = SpawnElement<T>();
            Element.SetOwner(this);
            Element.SetValue(args);
        }

        public virtual void RecycleElement(bool effect = false, bool bReDirection = true)
        {
            if (!Element) return;
            Element.Recycle(effect);
            
            Args = null;
            Element = null;
        }
        
        /// <summary>
        /// 获取所有参数的值作为只读列表。
        /// </summary>
        /// <returns>只读参数列表</returns>
        public object[] GetAllValues()
        {
            return Args;
        }

        public ElementBase GetElement()
        {
            return Element;
        }

        public bool IsEmpty()
        {
            return !Element;
        }
    }
}
