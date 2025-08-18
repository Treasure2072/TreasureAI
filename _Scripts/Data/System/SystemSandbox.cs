using DragonLi.Core;
using DragonLi.UI;
using Game;

namespace Data
{
    public class SystemSandbox : Singleton<SystemSandbox>
    {
        #region Properties - Handler
        
        /// <summary>
        /// 语言类型缓存
        /// </summary>
        public LanguageHandler LanguageHandler { get; private set; }
        
        /// <summary>
        /// 系统音量
        /// </summary>
        public VolumeHandler VolumeHandler { get; private set; }
        
        /// <summary>
        /// 教程
        /// </summary>
        public TutorialHandler TutorialHandler { get; private set; }
        
        /// <summary>
        /// 骰子
        /// </summary>
        public DiceMultipleHandler DiceMultipleHandler { get; private set; }
        
        #endregion

        #region Function

        private void InitData()
        {
            LanguageHandler = new LanguageHandler();
            VolumeHandler = new VolumeHandler();
            TutorialHandler = new TutorialHandler();
            DiceMultipleHandler = new DiceMultipleHandler();
        }

        #endregion

        #region API

        public void InitializeSystemSandbox()
        {
            InitData();
            
            LocalizationManager.Instance.SetLanguage(LanguageHandler.LanguageType);
        }
        
        public void DebugInitializeSystemSandbox()
        {
#if UNITY_EDITOR
            InitializeSystemSandbox();
#endif
        }

        #endregion
    }
}