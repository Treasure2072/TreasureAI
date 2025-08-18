using DragonLi.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Game
{
    public class UITutorialLayer : UILayer, IPointerClickHandler
    {
        #region Property

        [Header("Settings")]
        [SerializeField] private GameObject[] steps;

        [Header("Event")]
        [SerializeField] private UnityEvent onTutorialFinishEvent = new();

        public UnityEvent OnHidEvent { get; set; } = new();
        
        private int StepIndex { get; set; }
        
        #endregion

        #region UILayer

        protected override void OnShow()
        {
            base.OnShow();
            
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")?.SetElementsVisible(false);
            
            StepIndex = -1;
            foreach (var step in steps)
            {
                step.SetActive(false);
            }

            NextStep();
        }
        
        protected override void OnHide()
        {
            base.OnHide();
            UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")?.SetElementsVisible(true);
            onTutorialFinishEvent?.Invoke();
            onTutorialFinishEvent?.RemoveAllListeners();
            
            OnHidEvent?.Invoke();
            OnHidEvent?.RemoveAllListeners();
        }

        #endregion

        #region API

        public static UITutorialLayer GetLayer(string layerName)
        {
            var layer = UIManager.Instance.GetLayer<UITutorialLayer>(layerName);
            Debug.Assert(layer);
            return layer;
        }

        #endregion
        
        #region Function

        private void NextStep(bool bForce = false)
        {
            if (StepIndex >= 0 && StepIndex < steps.Length)
            {
                var current = steps[StepIndex];
                var conversation = current.GetComponent<ITutorialStep>();
                if (conversation != null)
                {
                    if (!conversation.Skippable && !bForce) return;
                    conversation.InterruptStep();
                    current.SetActive(false);
                    conversation.FinishStep();
                }
                else
                {
                    current.SetActive(false);
                }
            }

            StepIndex++;

            if (StepIndex >= steps.Length)
            {
                Hide();
                return;
            }
            var newStepObject = steps[StepIndex];
            newStepObject.SetActive(true);
            var newConversation = newStepObject.GetComponent<ITutorialStep>();
            if (newConversation != null)
            {
                newConversation.StartStep(() =>
                {
                    // NextStep(true);
                });
            }
        }

        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            NextStep();
        }
    }
}