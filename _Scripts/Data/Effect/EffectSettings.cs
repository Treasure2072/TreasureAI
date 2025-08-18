using DragonLi.UI;
using Game;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "EffectSettings", menuName = "Scriptable Objects/EffectSettings")]

    public class EffectSettings : ScriptableObject
    {
        [Header("Settings - FullScreen")] 
        [SerializeField] public VFXCoinDiceToken2D vfxCoinDiceToken;
        [SerializeField] public GameObject vfxCash2DBig;
        [SerializeField] public GameObject vfxCash2DMedium;
        [SerializeField] public GameObject vfxCash2DSmall;
        [SerializeField] public GameObject vfxDice2DBig;
        [SerializeField] public GameObject vfxDice2DMedium;
        [SerializeField] public GameObject vfxDice2DSmall;
        [SerializeField] public GameObject vfxToken2DBig;
        [SerializeField] public GameObject vfxToken2DMedium;
        [SerializeField] public GameObject vfxToken2DSmall;
        [SerializeField] public GameObject vfxCoinToken2DSmall;

        [Space] 
        [SerializeField] public UIWSScreenEffectTip tip;
        [SerializeField] public UIWSScreenEffectTip tipCoin;
        [SerializeField] public UIWSScreenEffectTip tipDice;
        [SerializeField] public UIWSScreenEffectTip tipToken;
        
        [Space]
        [Header("Settings - FullScreen-Chance")]
        [SerializeField] public GameObject vfxChance;
        [SerializeField] public GameObject actionChanceGood;
        [SerializeField] public GameObject actionChanceBad;
        
        [Space]
        
        [Header("Settings - Player")]
        [SerializeField] public GameObject vfxPlayerJump;
        [SerializeField] public GameObject vfxPlayerLand;
        
        [Header("Settings - Land")]
        [SerializeField] public UIWorldElement uiEffectLandLocked;
        [SerializeField] public UIWorldElement uiEffectLandLvLimit;
        [SerializeField] public UIWorldElement uiEffectLandLvMax;
        [SerializeField] public UIWorldElement uiEffectLandNoCoin;
        [SerializeField] public UIWorldElement uiEffectLandUpgradeTimer;
        
        [Header("Settings")]
        [SerializeField] public UIWorldElement uiEffectCoinNumber;
        [SerializeField] public UIWorldElement uiEffectDiceNumber;
        [SerializeField] public UIWorldElement uiEffectTokenNumber;
        [SerializeField] public UIWorldElement uiEffectMinusCoinNumber;

        [Header("Settings - BuildArea")] 
        [SerializeField] public UIWorldElement uiBuildGroup;
        [SerializeField] public GameObject uiBuildAreaUnlockButton;
        [SerializeField] public GameObject uiBuildAreaUpgradeButton;
        [SerializeField] public UIWSTimer uiBuildAreaUpgradeTimer;
        [SerializeField] public GameObject uiBuildAreaBoost;
        [SerializeField] public GameObject uiBuildAreaWithDraw;
        
        [Header("Settings - Bank")]
        [SerializeField] public UIWorldElement uiBankCoinTip;
        [SerializeField] public UIWorldElement uiBankOpenButton;
        
        [Header("Settings - Tutorial")]
        [SerializeField] public GameObject uiFingerPrefab;
    }

}