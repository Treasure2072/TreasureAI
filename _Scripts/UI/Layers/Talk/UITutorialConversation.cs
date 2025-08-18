using System.Collections;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class UITutorialConversation : UIComponent, ITutorialStep
    {
        #region Property

        [Header("Reference")]
        [SerializeField] private TextMeshProUGUI content;

        [Header("Settings")]
        [SerializeField] private string contentKey;
        [SerializeField] private float displaySpeed = 90f;
        [SerializeField] private float lockTime = 0.25f;
        
        [Header("Event")]
        [SerializeField] private UnityEvent onFinishSpeakEvent = new();
        
        private bool IsProcessing { get; set; }
        
        private float SkipTime { get; set; }
        
        private int DisplayTextIndex { get; set; }
        
        private string FullText { get; set; }
        
        private Coroutine DisplayCoroutine { get; set; }
        
        private UnityAction OnFinishSpeak { get; set; }

        public bool Skippable => SkipTime < Time.unscaledTime;
        
        #endregion


        #region API

        public void Initialize(string key, float speed = 90.0f, float time = 0.25f)
        {
            contentKey = key;
            displaySpeed = speed;
            lockTime = time;
            gameObject.SetActive(false);
        }

        public void StartStep(UnityAction onFinishSpeak = null)
        {
            if (IsProcessing) return;
            IsProcessing = true;

            SkipTime = Time.unscaledTime + lockTime;

            OnFinishSpeak = onFinishSpeak;
            content.SetText("");
            DisplayTextIndex = 0;
            FullText = this.GetLocalizedText(contentKey);
            CoroutineTaskManager.Instance.WaitFrameEnd(() =>
            {
                DisplayCoroutine = StartCoroutine(ProcessDisplay());
            });
        }

        public void InterruptStep()
        {
            if (!IsProcessing || !Skippable) return;
            
            IsProcessing = false;
            if(DisplayCoroutine == null) return;
            StopCoroutine(DisplayCoroutine);
            DisplayCoroutine = null;
        }

        public void FinishStep()
        {
            IsProcessing = false;

            onFinishSpeakEvent?.Invoke();
            
            OnFinishSpeak?.Invoke();
            OnFinishSpeak = null;
        }

        #endregion
        
        #region Coroutines

        private IEnumerator ProcessDisplay()
        {
            while (DisplayTextIndex < FullText.Length)
            {
                var tempIndex = DisplayTextIndex + displaySpeed * Time.fixedDeltaTime;
                
                DisplayTextIndex = (int)tempIndex;
                content.text = FullText[..DisplayTextIndex];
                yield return null;
            }
            
            IsProcessing = false;
        }

        #endregion
    }
}