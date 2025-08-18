using System.Collections;
using DragonLi.Core;
using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    public class BuildingArea : MonoBehaviour
    {
        #region Define

        private static readonly int AnimHashBuild = Animator.StringToHash("Build");
        private static readonly int AnimHashBuildReverse = Animator.StringToHash("BuildReverse");

        #endregion
        
        #region Fields

        [Header("References")]
        [SerializeField] private GameObject lockObject;
        [SerializeField] private GameObject[] levelBuildings;
        [SerializeField] private Animator buildingAnimator;
        [SerializeField] private MaterialBlinker landMaterialBlinker;
        [SerializeField] private MaterialComponent[] levelMaterials;
        
        #endregion

        #region Properties
        
        private WaitForSeconds ThreeSeconds { get; } = new(3f);

        private int Level { get; set; }

        #endregion
        
        #region API

        public int GetLevel()
        {
            return Level;
        }

        public void Blink()
        {
            landMaterialBlinker?.BlinkOnce();
        }

        public void SetLevel(int level)
        {
            Level = level;
            Level = Mathf.Clamp(Level, 0, levelBuildings.Length);
            SetBlinkerMaterials(level);
            SetBuilding(level);
        }

        public void Upgrading()
        {
            Build();
        }

        public void UpgradeEnd()
        {
            Level++;
            Level = Mathf.Clamp(Level, 0, levelBuildings.Length);
            SetBlinkerMaterials(Level);
            SetBuilding(Level);
            Build(true);
        }

        public void SetUILockState(bool display)
        {
            lockObject.SetActive(display);
        }

        #endregion

        #region Function

        private void SetBlinkerMaterials(int level)
        {
            for (var i = 0; i < levelMaterials.Length; i++)
            {
                var levelMaterial = i < level
                    ? BuildingAreaInstance.Instance.Settings.ShiningMaterial
                    : BuildingAreaInstance.Instance.Settings.GrayMaterial;
                levelMaterials[i].SetMaterial(new[]{ new Material(levelMaterial) });
            }
        }

        private void SetBuilding(int level)
        {
            for (var i = 0; i < levelBuildings.Length; i++)
            {
                levelBuildings[i].SetActive(level - 1 == i);
            }
        }

        private void Build(bool reverse = false)
        {
            buildingAnimator.SetTrigger(reverse ? AnimHashBuildReverse : AnimHashBuild);
        }

        #endregion
    }
}