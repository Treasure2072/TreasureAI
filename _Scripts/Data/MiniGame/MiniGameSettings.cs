using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = nameof(MiniGameSettings), menuName = "Scriptable Objects/MiniGameSettings")]

    public class MiniGameSettings : ScriptableObject
    {
        [SerializeField] public List<TMatchThreeElement> elements;
    }
}
