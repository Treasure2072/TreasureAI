using _Scripts.UI.Common;
using Data;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class LoginVisitorComponent : UIComponent
    {
        #region Property

        [Header("Settings")]
        [SerializeField] private float timeout = 20f;
        
        private bool IsLoginSucceed { get; set; }

        #endregion

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            IsLoginSucceed = false;
        }

        #endregion
        
        #region Callback

        public void OnVisitorLogin(UIBasicButton sender)
        {
            if (IsLoginSucceed) return;
            
            UILoadingLayer.GetLayer().ShowLayer(() =>
            {
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("error"), 
                    this.GetLocalizedText("connection-timeout"), 
                    UITipLayer.ETipType.Bad);
            }, timeout);
            GameSessionAPI.LoginAPI.SendVisitorLogin(PlayerSandbox.Instance.RegisterAndLoginHandler.VisitorUuid, response =>
            {
                this.LogEditorOnly($"尝试游客登陆， uuid = <color=green>{PlayerSandbox.Instance.RegisterAndLoginHandler.VisitorUuid}</color>");
                UILoginLayer.GetLayer()?.OnVerifyResponseCallback(response, "visitor");
            });
        } 

        #endregion
    }
}