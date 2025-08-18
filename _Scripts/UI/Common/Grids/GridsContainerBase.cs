using System;
using System.Collections.Generic;
using DragonLi.Core;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.UI.Common.Grids
{
    public class GridsContainerBase : MonoBehaviour
    {
        [Header("Grids Settings")]
        [SerializeField] private Transform gridRoot;
        
        [Header("Prefab Settings")] 
        [SerializeField] private GridBase gridPrefab;
        [SerializeField] private ElementBase elementPrefab;
        
        protected List<GridBase> Grids { get; set; } = new List<GridBase>();

        protected virtual void Awake()
        {
            Initialized();
        }
        
        private void Initialized()
        {
            OnInitialized();
        }
        
        protected virtual void OnInitialized() { }
        
        protected virtual T SpawnGrid<T>() where T : GridBase
        {
            var objIPool = SpawnManager.Instance.GetObjectFromPool(gridPrefab.gameObject);
            var obj = objIPool.GetGameObject();
            if (!obj)
            {
                throw new NullReferenceException($"Object of type {gridPrefab.gameObject.name} could not be spawned.");
            }
            
            obj.transform.SetParent(gridRoot, false);
            obj.transform.localScale = Vector3.one;
            T objComp = obj.GetComponent<T>();
            
            objComp.Initialize();
            objComp.SetElementPrefab(elementPrefab);
            
            Grids.Add(objComp);
            return objComp;
        }
        
        public virtual void SpawnAllGrids(params object[] args){ }
        
        public virtual void RecycleAllGrids(Action onSucceed = null, bool bRedirection = false)
        {
            foreach (var gridComp in Grids)
            {
                Assert.IsNotNull(gridComp);

                if (bRedirection)
                {
                    gridComp.transform.SetParent(null);
                }
                
                gridComp.RecycleElement();
                gridComp.Recycle();
                SpawnManager.Instance.AddObjectToPool(gridComp.gameObject);
            }
            Grids.Clear();
            onSucceed?.Invoke();
        }

        protected void RecycleGrid(GridBase grid, bool bRedirection = false)
        {
            Assert.IsNotNull(grid);
            if (bRedirection)
            {
                grid.transform.SetParent(null);
            }
            grid.Recycle();
            SpawnManager.Instance.AddObjectToPool(grid.gameObject);
            Grids.Remove(grid);
        }

        protected virtual void LayoutRebuild()
        {
            LayoutRebuilder.MarkLayoutForRebuild(gridRoot as RectTransform);
        }

        protected Transform GetRoot()
        {
            return gridRoot;
        }

        protected T GetGridByIndex<T>(int index) where T : GridBase
        {
            if(index < 0 || index >= Grids.Count) return null;
            return Grids[index] as T;
        }
    }
}
