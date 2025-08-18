using System;
using System.Collections.Generic;
using DragonLi.Core;
using UnityEngine;

namespace _Scripts.UI.Common.Grids
{
    [RequireComponent(typeof(DummyPoolObject))]
    public class ElementBase : MonoBehaviour
    {
        private GridBase Owner { get; set; }
        private List<object> Args { get; } = new List<object>();
        
        protected virtual void OnInitialized() {  }

        public virtual void Recycle(bool effect = false)
        {
            SpawnManager.Instance.AddObjectToPool(gameObject);
        }

        public void Initialized()
        {
            OnInitialized();
        }

        public void SetOwner(GridBase owner)
        {
            Owner = owner;
        }

        public GridBase GetOwner()
        {
            return Owner;
        }

        public virtual void SetValue(params object[] args)
        {
            Args.Clear();
            Args.AddRange(args);
        }

        public T GetValue<T>(int index)
        {
            // 检查参数是否为空
            if (Args == null)
            {
                throw new InvalidOperationException("Args cannot be null.");
            }

            // 索引是否有效
            if (index < 0 || index >= Args.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            return (T)Args[index];
        }
        
        /// <summary>
        /// 获取所有参数的值作为只读列表。
        /// </summary>
        /// <returns>只读参数列表</returns>
        public IReadOnlyList<object> GetAllValues()
        {
            return Args.AsReadOnly();
        }
    }
}
