using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class UIArrowTweener : MonoBehaviour
    {
        #region Proeprty
        
        [SerializeField] private Ease ease = Ease.InOutCubic;
        [SerializeField] private float distance = 20;
        [SerializeField] private float duration = 1.0f;
        [SerializeField] private Vector3 direction = Vector2.up;
        
        #endregion

        #region Unity

        private void Start()
        {
            transform.DOLocalMove(transform.localPosition + direction * distance, duration).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        }

        #endregion        

    }
}