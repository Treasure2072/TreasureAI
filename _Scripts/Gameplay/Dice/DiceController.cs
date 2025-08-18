using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(WorldObjectRegister))]
    public class DiceController : MonoBehaviour
    {
        #region Define

        [Serializable]
        public class CentralMapDiceAnimFaceOffset
        {
            public Vector3 dice00;
            public Vector3 dice01;
        }
        
        public static readonly string WorldObjectRegisterKey = "DiceController";
        
        private static readonly int AnimationPropertyRandomID = Animator.StringToHash("AnimIndex");

        #endregion

        #region Fields
        
        [Header("References")]
        [SerializeField] private Transform DropTransform;
        [SerializeField] private Transform ChessBordCenter;

        [Header("Settings")]
        [Range(0f, 1f)] public float areaRate = 0f;
        [SerializeField] private Animator diceAnimator;
        [SerializeField] private DiceComponent dice00;
        [SerializeField] private DiceComponent dice01;
        [SerializeField] private int animationCount = 6;
        
        [Header("Offset")] 
        [SerializeField] private List<CentralMapDiceAnimFaceOffset> animFaces;

        #endregion

        #region Properties

                
        private ChessGameBoard ChessGameBordRef { get; set; }
        
        private GameCharacter GameCharacterRef { get; set; }
        
        private bool bLoaded { get; set; } = false;
        
        private bool bProcessing { get; set; } = false;
        
        private Vector3 InitialPosition { get; set; }
        private float DisappearOffsetY { get; set; }
        
        #endregion

        #region Unity

        private void Awake()
        {
            InitialPosition = transform.position;
            DisappearOffsetY = transform.position.y - 2f;
            transform.position = Vector3.right * transform.position.x + Vector3.forward * transform.position.z +
                                                      Vector3.up * -10;

            dice00.Initialized();
            dice01.Initialized();
        }

        private IEnumerator Start()
        {
            while (!(ChessGameBordRef = World.GetRegisteredObject<ChessGameBoard>(Game.ChessGameBoard.WorldObjectRegisterKey)))
            {
                yield return null;
            }

            while (!(GameCharacterRef = World.GetPlayer()?.GetComponent<GameCharacter>()))
            {
                yield return null;
            }

            bLoaded = true;
        }

        #endregion

        #region Functions

        private void SetDiceLocation(int standIndex)
        {
            transform.position = InitialPosition;
            
            var startVec = (DropTransform.position - ChessBordCenter.position).normalized;
            var tile = ChessGameBordRef.GetTileByIndex(standIndex);
            var endVec = (tile.gameObject.transform.position - ChessBordCenter.position).normalized;
            var angle = Vector3.SignedAngle(startVec, endVec, Vector3.up);
            transform.RotateAround(ChessBordCenter.position, Vector3.up, angle);
            var dirVec = ChessBordCenter.position - DropTransform.position;
            transform.position += dirVec * (1f - areaRate);
        }

        #endregion
        
        #region API

        public void SetDiceType(DiceComponent.EDiceType type)
        {
            dice00.SetupDefaultDice(type);
            dice01.SetupDefaultDice(type);
        }

        public void SpawnDices(int diceA, int diceB, bool bTile = true)
        {
            if (!bLoaded) return;
            if(diceA <= 0 || diceB <= 0) return;
            bProcessing = true;
            SetDiceLocation(PlayerSandbox.Instance.ChessBoardHandler.StandIndex);
            
            dice00.gameObject.SetActive(true);
            dice01.gameObject.SetActive(true);
            diceAnimator.gameObject.SetActive(false);
            diceAnimator.gameObject.SetActive(true);
            
            var randomId = UnityEngine.Random.Range(0, animationCount);
            diceAnimator.SetFloat(AnimationPropertyRandomID, randomId);
            dice00.SetDiceFace(animFaces[randomId].dice00, Mathf.Clamp(diceA, 1, 6));
            dice01.SetDiceFace(animFaces[randomId].dice01, Mathf.Clamp(diceB, 1, 6));
            
            SoundAPI.PlaySound(AudioInstance.Instance.Settings.dice[UnityEngine.Random.Range(0, AudioInstance.Instance.Settings.dice.Length)]);

            NonmainThreadActionDispatcher.Instance.Enqueue(() =>
            {
                CoroutineTaskManager.Instance.WaitSecondTodo(() =>
                {
                    dice00.GetDiceBlinker()?.BlinkOnce();
                    dice01.GetDiceBlinker()?.BlinkOnce();
                    var tween = transform.DOMoveY(DisappearOffsetY, 1f)
                        .SetEase(Ease.InQuart);
                    tween.onComplete = () =>
                    {
                        bProcessing = false;
                    };
                }, bTile ? GameCharacterRef.GetMovement().GetStepDuration() * (diceA + diceB) : 0.5f);
            });
            
            // CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            // {
            //     bProcessing = false;
            //     this.LogEditorOnly($"dice process - : {IsProcessing()}");
            // }, (bTile ? GameCharacterRef.GetMovement().GetStepDuration() * (diceA + diceB) : 0.5f) + 1f);
        }

        public bool IsProcessing()
        {
            return bProcessing;
        }

        // public void HideDice()
        // {
        //     diceAnimator.gameObject.SetActive(false);
        // }

        #endregion
    }
}