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
    [RequireComponent(typeof(ChessGameCharacterMovement))]
    public class ChessGameCharacter : GameCharacter
    {
        #region Properties
        
        [Header("Prefabs")]
        [SerializeField] private GameObject jumpEffectPrefab;
        [SerializeField] private GameObject landEffectPrefab;
        
        public delegate bool PauseCheckDelegate();
        private ChessGameCharacterMovement CharacterMovement { get; set; }
        private DragonLiCamera CameraRef { get; set; }

        public event Action<GameCharacter> OnCharacterMoveStart;
        public event Action<GameCharacter> OnCharacterMoveEnd;
        public event Action<GameCharacter> OnCharacterJumpStart;
        public event Action<GameCharacter> OnCharacterJumpEnd;

        #endregion

        #region Unity

        private IEnumerator Start()
        {
            World.SetPlayer(transform);
            
            while (!(CameraRef = World.GetMainCamera()?.GetComponent<DragonLiCamera>()))
            {
                yield return null;
            }
            CameraRef.RegisterPlayer(transform);
            
            CharacterMovement = GetComponent<ChessGameCharacterMovement>();
            CharacterMovement.Initialize(transform);
            
            // TODO: 根据后续需求调整
            // ...
            SetCharacterModel(PlayerSandbox.Instance.CharacterHandler.CharacterId);
        }

        /// <summary>
        /// 转场后不设置为null，获取player会报错
        /// </summary>
        private void OnDestroy()
        {
            World.SetPlayer(null);
            OnCharacterMoveStart = null;
            OnCharacterMoveEnd = null;
            OnCharacterJumpStart = null;
            OnCharacterJumpEnd = null;
        }

        #endregion
        
        #region API
        
        public IEnumerator MoveForward(List<ChessTile> tiles, 
            ChessTile.EArriveAnimationType animationType, 
            ChessGameCharacterMovement.PassTileDelegate onPass,
            ChessGameCharacterMovement.ArriveTileDelegate onArrive,
            PauseCheckDelegate onPauseCheck)
        {
            OnCharacterMoveStart?.Invoke(this);
            GetCharacterAnimatorInterface()?.Move();
            yield return StartCoroutine(CharacterMovement.MoveForward(tiles, animationType, onPass, onArrive));

            yield return null;
            while (onPauseCheck())
            {
                yield return null;
            }
            OnCharacterMoveEnd?.Invoke(this);
        }
        
        public IEnumerator Teleport(ChessTile tile, ChessTile.EArriveAnimationType animationType, ChessGameCharacterMovement.ArriveTileDelegate onArrive)
        {
            OnCharacterJumpStart?.Invoke(this);
            // 做旋转
            var direction = tile.transform.position - transform.position;
            transform.DORotateQuaternion(Quaternion.LookRotation(direction.normalized), 0.15f).SetEase(Ease.Linear);
            
            // 播放跳跃动画
            GetCharacterAnimatorInterface()?.Jump();
            yield return CoroutineTaskManager.Waits.QuarterSecond;
            
            // 播放特效
            SpawnManager.Instance.GetObjectFromPool(jumpEffectPrefab, transform.position, Quaternion.identity);
            
            // 做相对移动动画
            var halfDuration = CharacterMovement.teleportDuration / 3.0f;
            model.DOLocalMoveY(3, halfDuration * 2).SetEase(Ease.OutQuart);
            model.DOLocalMoveY(0, halfDuration).SetEase(Ease.InQuart).SetDelay(halfDuration * 2);
            transform.DORotateQuaternion(Quaternion.LookRotation(tile.GetForwardDirection()), CharacterMovement.teleportDuration).SetEase(Ease.InOutCubic);
            yield return StartCoroutine(CharacterMovement.Teleport(tile, animationType, onArrive));
            
            // 播放特效
            SpawnManager.Instance.GetObjectFromPool(landEffectPrefab, transform.position, Quaternion.identity);
            
            OnCharacterJumpEnd?.Invoke(this);
        }

        public bool IsMoving()
        {
            return CharacterMovement.IsMoving;
        }

        #endregion
    }
}


