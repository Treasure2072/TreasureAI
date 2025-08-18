using DragonLi.UI;
using Game;
using UnityEngine;

namespace Data
{
    public enum EErrorType
    {
        ERROR_INVALID_CODE,
        ERROR_USER_NOT_EXISTS,
        ERROR_USER_EXISTS,
        ERROR_NEED_VERIFIED,
        ERROR_SEND_EMAIL_FAILED,
        ERROR_EMAIL_NOT_EXISTS,
    }
    public static class ServerError
    {
        #region API

        public static string GetError(this EErrorType errorType)
        {
            return errorType.ToString();
        }

        public static string GetLocalizationText(this EErrorType errorType)
        {
            return LocalizationManager.Instance.GetContent(GetError(errorType));
        }
        
        public static void PopTipError(string localizationErrorKey, UnityEngine.Events.UnityAction callback = null)
        {
            UITipLayer.DisplayTip( LocalizationManager.Instance.GetContent("notice"),
                LocalizationManager.Instance.GetContent(localizationErrorKey),
                UITipLayer.ETipType.Bad,
                callback);
        }

        #endregion
    }
}