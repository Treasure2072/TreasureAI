using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Scriptable Objects/AudioSettings")]
public class AudioSettings : ScriptableObject
{
    [Header("Prefab")]
    [SerializeField] private GameObject soundPrefab;
    public GameObject SoundPrefab { get => soundPrefab; set => soundPrefab = value; }

    [Space(10)] 
    [Header("SFX Background")] 
    [SerializeField] public AudioClip turingBar;
    [SerializeField] public AudioClip building;
    [SerializeField] public AudioClip chessboard;
    [SerializeField] public AudioClip matchThree;
    [SerializeField] public AudioClip scratch;

    [Space(10)]
    [Header("SFX Player Step")]
    [SerializeField] public AudioClip[] playerStep;
    
    [Space(10)]
    [Header("SFX Dice")]
    [SerializeField] public AudioClip[] dice;
    
    [Space] 
    [SerializeField] public AudioClip comboFinish;

    [SerializeField] public AudioClip bad;
    [SerializeField] public AudioClip goodSmall;
    [SerializeField] public AudioClip goodBig;
    
    [Space(10)]
    [Header("SEF Gain")]
    [SerializeField] public AudioClip moneyGain;
    [SerializeField] public AudioClip diceGain;
}
