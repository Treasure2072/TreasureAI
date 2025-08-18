using System;
using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    public class DissolveOthers : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var dissolve = other.GetComponent<MaterialDissolve>();
            if (!dissolve)
            {
                return;
            }
            
            dissolve.Dissolve();
        }
        
        private void OnTriggerExit(Collider other)
        {
            var dissolve = other.GetComponent<MaterialDissolve>();
            if (!dissolve)
            {
                return;
            }
            
            dissolve.Display();
        }
    }
}


