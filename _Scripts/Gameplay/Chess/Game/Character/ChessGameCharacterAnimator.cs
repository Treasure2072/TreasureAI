using System;
using UnityEngine;

namespace Game
{
    public interface IChessGameCharacterAnimator
    {
        void Idle();
        void Move();
        void Happy();
        void Sad();
        void Jump();
    }
    
    
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ChessGameCharacterAnimator : MonoBehaviour, IChessGameCharacterAnimator
    {
        #region Properties

        [Header("Settings")]
        [SerializeField]
        private int maxIdleIndex = 1;
        
        private Animator AnimatorRef { get; set; }
        
        private static readonly int AnimHashIdleIndex = Animator.StringToHash("IdleIndex");
        private static readonly int AnimHashRun = Animator.StringToHash("Run");
        private static readonly int AnimHashHappy = Animator.StringToHash("Happy");
        private static readonly int AnimHashSad = Animator.StringToHash("Sad");
        private static readonly int AnimHashJump = Animator.StringToHash("Jump");

        #endregion

        #region Unity

        private void Start()
        {
            AnimatorRef = GetComponentInChildren<Animator>();
            Idle();
        }

        #endregion
        
        #region IChessGameCharacterAnimator

        public virtual void Idle()
        {
            if (!AnimatorRef)
            {
                return;
            }
            
            AnimatorRef.SetFloat(AnimHashIdleIndex, UnityEngine.Random.Range(0, maxIdleIndex + 1));
            AnimatorRef.SetBool(AnimHashRun, false);
        }

        public virtual void Move()
        {
            if (!AnimatorRef)
            {
                return;
            }
            
            AnimatorRef.SetBool(AnimHashRun, true);
        }

        public virtual void Happy()
        {
            if (!AnimatorRef)
            {
                return;
            }
            
            AnimatorRef.SetTrigger(AnimHashHappy);
        }

        public virtual void Sad()
        {
            if (!AnimatorRef)
            {
                return;
            }
            
            AnimatorRef.SetTrigger(AnimHashSad);
        }

        public virtual void Jump()
        {
            if (!AnimatorRef)
            {
                return;
            }
            
            AnimatorRef.SetTrigger(AnimHashJump);
        }

        #endregion
    }
}


