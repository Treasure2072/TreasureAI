using _Scripts.UI.Common;
using Data;
using DragonLi.Android;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace Game
{
    [RequireComponent(typeof(WebGLInputComponent))]
    public class RegisterEmailComponent : UIComponent
    {
        #region Property

        [Header("Settings")]
        [SerializeField] private float timeout = 30f;
        
        [Space(10)]
        
        [Header("Reference - Input")]
        [SerializeField] private TextMeshProUGUI tmpEmail;
        [SerializeField] private TextMeshProUGUI tmpCode;
        [SerializeField] private TextMeshProUGUI tmpVerify;
        
        [Header("Reference - Timer")]
        [SerializeField] private CountdownComponent timerComponent;
        [SerializeField] private TextMeshProUGUI tmpTime;
        [SerializeField] private GameObject getVerify;
        [SerializeField] private GameObject timer;
        
        private WebGLInputComponent Input { get; set; }

        private bool RegisterPass { get; set; } = false;

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (value == _email) return;
                _email = value;
                OnEmailChanged(value);
            }
        }
        private string Code { get; set; }
        private string Verify { get; set; }
        
        private bool EmailInput { get; set; }
        private bool CodeInput { get; set; }
        private bool VerifyInput { get; set; }

        private bool IsValidEmail => MathAPI.IsValidEmail(Email);

        private bool IsValidCode => !string.IsNullOrEmpty(Code);

        private bool IsValidVerify => MathAPI.IsNumeric(Verify) && MathAPI.IsFixedLength(Verify, 6);
        
        private bool RegisterRequest { get; set; }
        
        private bool VerifyRequest { get; set; }

        #endregion

        #region Unity
        
        private void Awake()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChanged;
            timerComponent.OnStart += OnTimerStart;
            timerComponent.OnEnd += OnTimerEnd;
            timerComponent.OnValueChanged += OnTimerValueChanged;
            
            EmailInput = false;
            CodeInput = false;
            Input = GetComponent<WebGLInputComponent>();
        }

        private void OnDestroy()
        {
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChanged;
            timerComponent.OnStart -= OnTimerStart;
            timerComponent.OnEnd -= OnTimerEnd;
            timerComponent.OnValueChanged -= OnTimerValueChanged;
        }

        private void OnEnable()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Input.OnReceiverInput += OnReceiveInput;
#else
            UIInputLayer.GetLayer().OnSubmit += OnReceiveInput;
#endif
        }

        private void OnDisable()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Input.OnReceiverInput -= OnReceiveInput;
#else
            UIInputLayer.GetLayer().OnSubmit -= OnReceiveInput;
#endif
        }

        #endregion
        
        #region UIComponent

        public override void OnShow()
        {
            base.OnShow();
            CoroutineTaskManager.Instance.WaitSecondTodo(RefreshInputContent, 1);
        }

        #endregion

        #region Function

        private void RefreshInputContent()
        {
            tmpEmail.SetText(Email.IsNullOrEmpty() ? this.GetLocalizedText("enter-you-email").ToUpper() : Email);
            tmpCode.SetText(Code.IsNullOrEmpty() ? this.GetLocalizedText("enter-you-code").ToUpper() : Code);
            tmpVerify.SetText(Verify.IsNullOrEmpty() ? this.GetLocalizedText("enter-you-verify-code").ToUpper() : Verify);
        }

        private bool IsResendable()
        {
            return timerComponent.IsEnd && !VerifyRequest;
        }
        
        private void OpenInput()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Input.OpenInput();
#else
            UIInputLayer.GetLayer().Show();
#endif
        }

        #endregion

        
        #region Callback
        
        private void OnLanguageChanged(string preValue, string newValue)
        {
            RefreshInputContent();
        }
        
        private void OnEmailChanged(string email)
        {
            RegisterPass = false;
        }

        private void OnInviteCodeChanged(string code)
        {
            
        }
        public void OnInputEmailClick(UIBasicButton sender)
        {
            EmailInput = true;
            OpenInput();
        }

        public void OnInputCodeClick(UIBasicButton sender)
        {
            CodeInput = true;
            OpenInput();
        }
        
        public void OnInputVerifyClick(UIBasicButton sender)
        {
            VerifyInput = true;
            OpenInput();
        }

        public void OnPasteCodeClick(UIBasicButton sender)
        {
            CodeInput = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            OnReceiveInput(GUIUtility.systemCopyBuffer);
#elif UNITY_ANDROID
            OnReceiveInput(AndroidExternalAPI.GetFromClipboard());
#endif
        }
        
        public void OnGetVerifyClick(UIBasicButton sender)
        {
            if (!IsValidEmail)
            {
                UILoginLayer.GetLayer()?.PopTipEmailInvalid();
                return;
            }

            if (!IsValidCode)
            {
                UILoginLayer.GetLayer()?.PopTipInviteCodeInvalid();
                return;
            }

            if (!IsResendable()) return;
            
            if (RegisterRequest) return;
            RegisterRequest = true;
            UILoadingLayer.GetLayer().ShowLayer(() =>
            {
                RegisterRequest = false;
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("error"), 
                    this.GetLocalizedText("connection-timeout"), 
                    UITipLayer.ETipType.Bad);
            }, timeout);
            
            timerComponent.StartCountdown();

            // TODO: 发送验证码
            // ...
            if (!RegisterPass)
            {
                GameSessionAPI.LoginAPI.SendRegister(Email, Code, response =>
                {
                    if (!RegisterRequest) return;
                    RegisterRequest = false;
                    UILoadingLayer.GetLayer()?.HideLayer();
                    if (response.IsSuccess())
                    {
                        this.LogEditorOnly("注册验证码发送成功");
                        RegisterPass = true;
                    }
                    else
                    {
                        var error = response.GetError();
                        var localized = this.GetLocalizedText(response.GetError());
                        UITipLayer.DisplayTip(
                            this.GetLocalizedText("error"),
                            localized.IsNullOrEmpty() ? error : this.GetLocalizedText(error),
                            UITipLayer.ETipType.Bad);
                    }
                });
            }
            else
            {
                GameSessionAPI.LoginAPI.SendResend(Email, response =>
                {
                    if (!RegisterRequest) return;
                    RegisterRequest = false;
                    UILoadingLayer.GetLayer()?.HideLayer();
                    if (response.IsSuccess())
                    {
                        this.LogEditorOnly("注册验证码重新发送成功");
                    }
                    else
                    {
                        var error = response.GetError();
                        var localized = this.GetLocalizedText(response.GetError());
                        UITipLayer.DisplayTip(
                            this.GetLocalizedText("error"),
                            localized.IsNullOrEmpty() ? error : this.GetLocalizedText(error),
                            UITipLayer.ETipType.Bad);
                    }
                });
            }
        }

        public void OnNextClick(UIBasicButton sender)
        {
            if (!IsValidEmail)
            {
                UILoginLayer.GetLayer()?.PopTipEmailInvalid();
                return;
            }

            if (!IsValidCode)
            {
                UILoginLayer.GetLayer()?.PopTipInviteCodeInvalid();
                return;
            }

            if (!IsValidVerify)
            {
                UILoginLayer.GetLayer()?.PopTipVerifyCodeInvalid();
                return;
            }
            
            if (VerifyRequest) return;
            VerifyRequest = true;
            UILoadingLayer.GetLayer().ShowLayer(() =>
            {
                VerifyRequest = false;
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("error"), 
                    this.GetLocalizedText("connection-timeout"), 
                    UITipLayer.ETipType.Bad);
            }, timeout);
            
            // TODO: 验证码，获取token
            // ...
            GameSessionAPI.LoginAPI.SendVerify(Email, Verify, response =>
            {
                if (!VerifyRequest) return;
                VerifyRequest = false;
                UILoadingLayer.GetLayer()?.HideLayer();
                UILoginLayer.GetLayer()?.OnVerifyResponseCallback(response, Email);
            });
        }

        public void OnBackClick(UIBasicButton sender)
        {
            if (RegisterRequest) return;
            UILoginLayer.GetLayer()?.OpenLoginEmailModal();
        }

        #endregion
        
        #region Callback - WebGL Input

        private void OnReceiveInput(string data)
        {
            if (EmailInput)
            {
                EmailInput = false;
                data = MathAPI.RemoveAllWhitespace(data);
                if (MathAPI.IsValidEmail(data))
                {
                    tmpEmail.SetText(data);
                    Email = data;
                }
                else
                {
                    // TODO: 输入邮箱地址不合法
                    // ...
                    UILoginLayer.GetLayer().PopTipEmailInvalid();
                }
            }

            if (CodeInput)
            {
                CodeInput = false;
                if (!string.IsNullOrEmpty(data))
                {
                    tmpCode.SetText(data);
                    Code = data;
                }
                else
                {
                    UILoginLayer.GetLayer().PopTipInviteCodeInvalid();
                }
                
            }

            if (VerifyInput)
            {
                VerifyInput = false;
                if (MathAPI.IsNumeric(data) && MathAPI.IsFixedLength(data, 6))
                {
                    tmpVerify.SetText(data);
                    Verify = data;
                }
                else
                {
                    UILoginLayer.GetLayer().PopTipVerifyCodeInvalid();
                }
            }
        }

        #endregion
        
        #region Callback - Timer

        private void OnTimerStart()
        {
            getVerify.SetActive(false);
            timer.SetActive(true);
        }

        private void OnTimerEnd()
        {
            getVerify.SetActive(true);
            timer.SetActive(false);
        }

        private void OnTimerValueChanged(float val)
        {
            tmpTime.SetText(val.ToString("0"));
        }

        #endregion
    }
}
