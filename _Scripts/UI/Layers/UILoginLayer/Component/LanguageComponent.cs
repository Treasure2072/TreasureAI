using _Scripts.UI.Common;
using Data;
using DragonLi.UI;
using UnityEngine;

namespace  Game 
{
    public class LanguageComponent : UIComponent
    {
        #region Define

        [System.Serializable]
        public struct FLanguageObj
        {
            public string language;
            public GameObject obj;

            public FLanguageObj(string lan, GameObject o)
            {
                language = lan;
                obj = o;
            }
        }

        #endregion
        
        #region Property

        [Header("Reference")] [SerializeField] private FLanguageObj[] languages = new FLanguageObj[3]
        {
            new FLanguageObj("English", null),
            new FLanguageObj("Chinese", null),
            new FLanguageObj("Vietnam", null)
        };

        #endregion
        
        #region Unity

        private void Awake()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChanged; 
        }

        private void OnDestroy()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChanged; 
        }

        #endregion

        #region UIComponent

        public override void OnShow()
        {
            base.OnShow();
            var index = GetCurrentLanguageIndex();
            ActiveLanguageByIndex(index);
        }

        #endregion

        #region Function

        private void SetLanguage(string language)
        {
            UIManager.Instance.SwitchLanguage(language, () =>
            {
                SystemSandbox.Instance.LanguageHandler.LanguageType = language;
            });
        }

        private int GetCurrentLanguageIndex()
        {
            for (var i = 0; i < languages.Length; i++)
            {
                if (languages[i].language == SystemSandbox.Instance.LanguageHandler.LanguageType)
                {
                    return i;
                }
            }

            return 0;
        }

        private int GetNextLanguageIndex()
        {
            var index = GetCurrentLanguageIndex();
            if (index == -1)
            {
                return 0;
            }

            return (index + 1) % languages.Length;
        }

        private void ActiveLanguageByIndex(int index)
        {
            if (index < 0 || index >= languages.Length) return;
            for (var i = 0; i < languages.Length; i++)
            {
                languages[i].obj.SetActive(i == index);
            }
        }

        #endregion
        
        #region Callback

        public void OnNextLanguageClick(UIBasicButton sender)
        {
            var index = GetNextLanguageIndex();
            ActiveLanguageByIndex(index);
            SetLanguage(languages[index].language);
        }

        private void OnLanguageChanged(string preValue, string newValue)
        {
            this.SaveLanguage(newValue);
        }


        #endregion
    }
}