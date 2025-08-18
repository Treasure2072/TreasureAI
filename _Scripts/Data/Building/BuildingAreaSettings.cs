using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "BuildingAreaSettings", menuName = "Scriptable Objects/BuildingAreaSettings")]

    public class BuildingAreaSettings : ScriptableObject
    {
        [SerializeField] private Material grayMaterial;
        [SerializeField] private Material shiningMaterial;
        [SerializeField] private List<BuildingAreaType.FCard> areasCards;
        
        public Material GrayMaterial => grayMaterial;
        public Material ShiningMaterial => shiningMaterial;

        public IReadOnlyList<BuildingAreaType.FCard> AreasCards => areasCards;

        public BuildingAreaType.FCard GetCardByType(BuildingAreaType.EBuildAreaType type)
        {
            return areasCards.FirstOrDefault(card => card.type == type);
        }
    }
}