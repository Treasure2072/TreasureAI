using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class ChessGameCharacterMovement : MonoBehaviour
    {
        #region Define

        public delegate void PassTileDelegate(ChessTile tile);
        public delegate void ArriveTileDelegate(ChessTile tile, ChessTile.EArriveAnimationType animationType);

        #endregion

        #region Proeprties

        [Header("Movement")]
        [SerializeField]
        private float stepDuration = 0.3f;
        [SerializeField]
        private float stepWait = 0.0f;
        [SerializeField]
        public float teleportDuration = 0.6f;
        
        public bool IsMoving { get; private set; }
        private Transform CharacterTransform { get; set; }
        private WaitForSeconds StepWait { get; set; }
        private WaitForSeconds StepDuration { get; set; }
        private WaitForSeconds StepDurationHalf { get; set; }
        private WaitForSeconds WaitTeleportDuration { get; set; }
        
        #endregion

        #region API

        public void Initialize(Transform character)
        {
            CharacterTransform = character;
            
            StepWait = new WaitForSeconds(stepWait);
            StepDuration = new WaitForSeconds(stepDuration);
            StepDurationHalf = new WaitForSeconds(stepDuration / 2);
            WaitTeleportDuration = new WaitForSeconds(teleportDuration);
        }

        public IEnumerator MoveForward(List<ChessTile> tiles, ChessTile.EArriveAnimationType animationType, PassTileDelegate onPass, ArriveTileDelegate onArrive)
        {
            if (IsMoving)
            {
                yield break;
            }
            
            yield return StartCoroutine(ProcessMoveForward(tiles, animationType, onPass, onArrive));
        }

        public IEnumerator Teleport(ChessTile tile, ChessTile.EArriveAnimationType animationType, ArriveTileDelegate onArrive)
        {
            yield return StartCoroutine(ProcessTeleport(tile, animationType, onArrive));
        }

        public float GetStepDuration()
        {
            return stepDuration + stepWait;
        }

        #endregion

        #region Functions

        private IEnumerator ProcessMoveForward(List<ChessTile> tiles, ChessTile.EArriveAnimationType animationType, PassTileDelegate onPass, ArriveTileDelegate onArrive)
        {
            IsMoving = true;
            for (var i = 0; i < tiles.Count; i++)
            {
                var tile = tiles[i];
                yield return ProcessCharacterMoveTo(tile, animationType, tiles.Count - i - 1);
                if (i == tiles.Count - 1)
                {
                    IsMoving = false;
                    onArrive(tile, animationType);
                }
                else
                {
                    onPass(tile);
                }
                
                SoundAPI.PlaySound(AudioInstance.Instance.Settings.playerStep[UnityEngine.Random.Range(0, AudioInstance.Instance.Settings.playerStep.Length)]);

                if (stepWait > 0)
                {
                    yield return StepWait;
                }
            }
        }
        
        private IEnumerator ProcessTeleport(ChessTile tile, ChessTile.EArriveAnimationType animationType, ArriveTileDelegate onArrive)
        {
            IsMoving = true;
            CharacterTransform.DOMove(tile.GetStandPosition(), teleportDuration).SetEase(Ease.Linear);
            yield return WaitTeleportDuration;
            IsMoving = false;
            onArrive(tile, animationType);
        }

        private IEnumerator ProcessCharacterMoveTo(ChessTile tile, ChessTile.EArriveAnimationType animationType, int remain)
        {   
            // TODO: 根据动画类型播放不同的玩家移动等
            // ...
            var direction = tile.GetForwardDirection();
            CharacterTransform.DOMove(tile.GetStandPosition(), stepDuration).SetEase(Ease.Linear);
            if(Vector3.Angle(CharacterTransform.forward, direction) > 5.0f)
            {
                CharacterTransform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f).SetEase(Ease.Linear);
            }
            
            yield return StepDuration;
        }

        #endregion
    }
}


