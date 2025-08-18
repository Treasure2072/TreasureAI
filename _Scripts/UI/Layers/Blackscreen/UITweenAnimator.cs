using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class UITweenAnimator : UITweener
    {
        [Header("Tween Animator")]
        [SerializeField] private string showKey = "Close";
        [SerializeField] private string hideKey = "Open";

        [SerializeField] private Animator animator;
        
        protected override void OnPlayForward()
        {
            base.OnPlayForward();
            animator.Play("Show", 0, 0f);
            animator.SetTrigger(showKey);
        }

        protected override void OnPlayBack()
        {
            base.OnPlayBack();
            animator.Play("Hide", 0, 0f);
            animator.SetTrigger(hideKey);
        }
    }
}