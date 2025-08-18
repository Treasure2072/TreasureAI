using System.Collections.Generic;
using System.Linq;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class UIBuildingGroup : UIWorldElement
    {
        #region Properties

        private List<GameObject> Router { get; set; } = new();

        #endregion

        #region API

        public T Spawn<T>(GameObject prefab)
        {
            var iObj = SpawnManager.Instance.GetObjectFromPool(prefab);
            Debug.Assert(iObj != null);

            var obj = iObj.GetGameObject();
            Debug.Assert(obj != null);
            Router.Add(obj);
            
            obj.transform.SetParent(transform);
            
            return obj.GetComponent<T>();
        }

        public void Remove(GameObject obj)
        {
            if(obj == null) return;
            foreach (var o in Router.ToList())
            {
                if (o.Equals(obj))
                {
                    SpawnManager.Instance.AddObjectToPool(o);
                    Router.Remove(obj);
                }
            }
        }
        
        public void RemoveAll()
        {
            foreach (var o in Router)
            {
                SpawnManager.Instance.AddObjectToPool(o);
            }
            Router.Clear();
        }

        #endregion
    }
}

