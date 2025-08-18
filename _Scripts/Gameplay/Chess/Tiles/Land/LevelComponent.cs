using System;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class LevelComponent : MonoBehaviour
    {
        public enum EHouseLocationType
        {
            Disappear,
            Part,
            All,
        }
        
        [SerializeField] private MeshRenderer houseMeshRenderer;

        [SerializeField] private float halfY = 0.115f;
        [SerializeField] private float disappearY = -0.26f;
        
        private float InitY { get; set; }

        private void Awake()
        {
            InitY = transform.localPosition.y;
        }

        private Material GetMaterial()
        {
            return houseMeshRenderer.materials[0];
        }

        public void SetColor(Color color)
        {
            GetMaterial().SetColor("_BaseColor", color);
        }

        public void SetState(EHouseLocationType type)
        {
            var y =
                type switch
                {
                    EHouseLocationType.Disappear => disappearY,
                    EHouseLocationType.Part => halfY,
                    EHouseLocationType.All => InitY,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
            transform.DOLocalMoveY(y, 1f).SetEase(Ease.OutSine);

            // transform.localPosition = type switch
            // {
            //     EHouseLocationType.Disappear => new Vector3(transform.localPosition.x, disappearY, transform.localPosition.z),
            //     EHouseLocationType.Part => new Vector3(transform.localPosition.x, halfY, transform.localPosition.z),
            //     EHouseLocationType.All => new Vector3(transform.localPosition.x, InitY, transform.localPosition.z),
            //     _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            // };
        }
    }

}