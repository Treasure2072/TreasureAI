using System;
using System.Collections.Generic;
using System.Linq;
using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    public class DiceComponent : MonoBehaviour
    {
        #region Define

        public enum EDiceType
        {
            White,
            Red,
        }
        
        [Serializable]
        public struct TDice
        {
            [SerializeField] public EDiceType Type;
            [SerializeField] public GameObject Dice;
        }

        #endregion
        
        #region Fields
        
        [Header("References")]
        [SerializeField] private List<TDice> DiceList = new();

        [Header("Settings")] 
        [SerializeField] private Transform animOffset;
        [SerializeField] private Transform diceModel;
        [SerializeField] private List<Vector3> offsets;

        #endregion

        #region Properties

        private Dictionary<EDiceType, GameObject> Dices;
        private GameObject DefaultDiceObject { get; set; }

        #endregion

        // #region Unity
        //
        // private void Awake()
        // {
        //     Initialized();
        //     
        // }
        //
        // #endregion

        #region Functions

        public void Initialized()
        {
            Dices = new Dictionary<EDiceType, GameObject>();
            foreach (var tDice in DiceList)
            {
                Dices.TryAdd(tDice.Type, tDice.Dice);
            }
            
            SetupDefaultDice(EDiceType.White);
        }

        public void SetDiceFace(Vector3 baseOffset, int face)
        {
            animOffset.localRotation = Quaternion.Euler(baseOffset);
            diceModel.localRotation = Quaternion.Euler(offsets[face - 1]);
        }

        #endregion

        #region API

        public void SetupDefaultDice(EDiceType type)
        {
            Dices.TryGetValue(type, out var defaultDice);
            if (defaultDice == null) return;
            
            DefaultDiceObject = defaultDice;
            DefaultDiceObject.SetActive(true);
            foreach (var dice in Dices.Where(dice => dice.Key != type))
            {
                dice.Value.SetActive(false);
            }
        }

        public MaterialBlinker GetDiceBlinker()
        {
            return DefaultDiceObject.GetComponent<MaterialBlinker>();
        }

        #endregion

    }
}
