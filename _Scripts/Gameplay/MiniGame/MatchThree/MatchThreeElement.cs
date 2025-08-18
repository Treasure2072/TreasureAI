using System;
using _Scripts.UI.Common.Grids;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Gameplay.MiniGame.MatchThree
{
    public class MatchThreeElement : ElementBase, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
    {
        #region Fields

        [Header("References")]
        [SerializeField] private TextMeshProUGUI tmpPos;
        [SerializeField] private Transform modelRoot;

        #endregion

        #region Properties
        
        private string ElementName { get; set; }
        private Vector3 InitScale { get; set; }

        private bool selected;
        private bool Selected
        {
            get => selected;
            set
            {
                if (selected == value) return;
                
                OnSelectedStatusChanged(selected, value);
                selected = value;
            }
        }
        private UnityEvent OnSelectedOperated { get; set; }
        
        private GameObject Model { get; set; }

        #endregion

        #region ElementBase

        protected override void OnInitialized()
        {
            base.OnInitialized();
            InitScale = transform.localScale;
            OnSelectedOperated = new UnityEvent();
        }

        public override void SetValue(params object[] args)
        {
            base.SetValue(args);
            if (Model)
            {
                Destroy(Model);
            }
            
            var gameMode = GetMatchThreeGameMode();
            if (!gameMode)
            {
                this.LogErrorEditorOnly($"game mode is not set!");
                return;
            }
            
            ElementName = args[0] as string;
            OnSelectedOperated.AddListener(() =>
            {
                (args[1] as Action<MatchThreeElement>)?.Invoke(this);
            });
            Model = Instantiate(MatchThreeInstance.Instance.GetMatchThreeElement(ElementName), modelRoot);
            
            tmpPos.text = $"{gameMode.container.GetGridPosition(GetOwner(), gameMode.maxRow, gameMode.maxColumn)}";
        }

        public override void Recycle(bool effect = false)
        {
            UnSelect();
            OnSelectedOperated.RemoveAllListeners();

            transform.DOScale(Vector3.one * 1.2f, 0.1f);
            transform.DOScale(Vector3.zero, 0.1f).SetDelay(0.1f);
            
            SpawnManager.Instance.AddObjectToPool(gameObject, 0.5f);

            if (effect)
            {
                var ipool = SpawnManager.Instance.GetObjectFromPool(MatchThreeInstance.Instance.GetMatchThreeEffect(ElementName));
                ipool.GetGameObject().transform.position = transform.position;
            }
            
        }

        #endregion

        #region Functions

        private MatchThreeGameMode GetMatchThreeGameMode()
        {
            return World.GetRegisteredObject<MatchThreeGameMode>(MatchThreeGameMode.WorldObjectRegisterKey);
        }

        #endregion

        #region API

        public void UnSelect()
        {
            Selected = false;
        }

        #endregion

        #region Callback

        private void OnSelectedStatusChanged(bool oldV, bool newV)
        {
            var gameMode = GetMatchThreeGameMode();
            if (gameMode.IsEnd()) return;
            if (newV)
            {
                // 选中
                transform.DOKill();
                transform.DOLocalMoveZ(0.1f, 0.1f);
                transform.DOScale(InitScale * 1.1f, 0.3f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                // 取消选中
                transform.DOKill();
                transform.localPosition = Vector3.zero;
                transform.localScale = InitScale;
            }
        }

        #endregion

        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     if (Selected)
        //     {
        //         Selected = false;
        //     }
        //     else
        //     {
        //         Selected = true;
        //     }
        //     OnSelectedOperated?.Invoke();
        // }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(GetMatchThreeGameMode().Processing) return;
            GetMatchThreeGameMode().Press = true;
            if (Selected)
            {
                Selected = false;
            }
            else
            {
                Selected = true;
            }
            OnSelectedOperated?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!GetMatchThreeGameMode().Press || GetMatchThreeGameMode().Processing) return;
            if (Selected)
            {
                Selected = false;
            }
            else
            {
                Selected = true;
            }
            OnSelectedOperated?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            GetMatchThreeGameMode().Press = false;
        }
    }
}