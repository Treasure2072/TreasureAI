using DragonLi.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Scripts.UI.Common
{
    [RequireComponent(typeof(Button))]
    public class UIBasicButton : UIComponent
    {
        #region Fields

        [Header("Events")]
        public UnityEvent<UIBasicButton> OnClickEvent = new();

        #endregion

        #region Properties

        public Button ButtonRef { get; private set; }

        private AudioSource ConfirmDownSource { get; set; }

        #endregion
        
        #region UIComponent

        public override void Init()
        {
            base.Init();
            SetupButton();
        }

        #endregion

        #region Notifications

        protected virtual void OnClickCallback()
        {
            OnClickEvent?.Invoke(this);
        }

        #endregion

        #region Functions

        protected void SetupButton()
        {
            ConfirmDownSource = GetComponent<AudioSource>();
            ButtonRef = GetComponent<Button>();
            ButtonRef.onClick.AddListener(OnClickCallback);
        }

        #endregion
    }
}


