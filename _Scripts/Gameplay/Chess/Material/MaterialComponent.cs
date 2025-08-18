using System;
using UnityEngine;

namespace Game
{
    public class MaterialComponent : MonoBehaviour
    {
        #region MyRegion

        [Header("Settings")]
        [SerializeField] private string materialName;

        #endregion
        
        #region Properties

        private MeshRenderer ModelMeshRenderer { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            ModelMeshRenderer = GetComponent<MeshRenderer>();
        }

        #endregion

        #region API

        public string GetMaterialName()
        {
            return materialName;
        }

        public void SetMaterial(Material[] materials)
        {
            ModelMeshRenderer.materials = materials;
        }

        #endregion
    }

}