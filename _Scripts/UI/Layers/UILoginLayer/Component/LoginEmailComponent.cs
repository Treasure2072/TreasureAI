using _Scripts.UI.Common;
using Data;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using WebSocketSharp;

namespace Game
{
    [RequireComponent(typeof(WebGLInputComponent))]
    public class LoginEmailComponent : UIComponent
    {
        #region Properties
        
        [Header("Settings")]
        [SerializeField] private float timeout = 30f;
        
        [Space(10)]
        
        [Header("Reference - Input")]
        [SerializeField] private TextMeshProUGUI tmpEmail;

        [SerializeField] private LoginEmailDropdown dropdownEmail;
        [SerializeField] private TextMeshProUGUI tmpVerify;

        [Header("Reference - Timer")]
        [SerializeField] private CountdownComponent timerComponent;
        [SerializeField] private TextMeshProUGUI tmpTime;
        [SerializeField] private GameObject getVerify;
        [SerializeField] private GameObject timer;
        
        private bool LoginPass { get; set; } = false;
        
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
        private string Verify { get; set; }
        private bool EmailReceiverInput { get; set; }
        private bool VerifyReceiveInput { get; set; }
        
        private bool IsValidEmail => MathAPI.IsValidEmail(Email);

        private bool IsValidVerify => MathAPI.IsNumeric(Verify) && MathAPI.IsFixedLength(Verify, 6);

        private bool LoginRequest { get; set; }
        private bool VerifyRequest { get; set; }

        #endregion

        #region Property - WebGL

#if UNITY_WEBGL && !UNITY_EDITOR
        private WebGLInputComponent Input { get; set; }
#endif

        #endregion

        #region Unity

        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            Input = GetComponent<WebGLInputComponent>();
#endif
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChanged;
            timerComponent.OnStart += OnTimerStart;
            timerComponent.OnEnd += OnTimerEnd;
            timerComponent.OnValueChanged += OnTimerValueChanged;
            
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
            tmpEmail.SetText(MathAPI.IsValidEmail(Email) ? Email : this.GetLocalizedText("enter-you-email").ToUpper());
            tmpVerify.SetText(Verify.IsNullOrEmpty() ? this.GetLocalizedText("enter-you-verify-code").ToUpper() : Verify);
        }

        private bool IsResendable()
        {
            return timerComponent.IsEnd && !VerifyRequest;
        }

        private bool CheckInput()
        {
            if (MathAPI.IsValidEmail(Email))
            {
                return true;
            }
            
            UILoginLayer.GetLayer()?.PopTipEmailInvalid();

            return false;
        }

        #endregion
        
        #region Callback

        private void OnLanguageChanged(string preValue, string newValue)
        {
            RefreshInputContent();
        }

        private void OnEmailChanged(string email)
        {
            LoginPass = false;
        }

        public void OnInputEmailClick(UIBasicButton sender)
        {
            EmailReceiverInput = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            Input.OpenInput();
#else
            UIInputLayer.GetLayer().Show();
#endif
        }

        public void OnInputVerifyClick(UIBasicButton sender)
        {
            VerifyReceiveInput = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            Input.OpenInput();
#else
            UIInputLayer.GetLayer().Show();
#endif
        }

        public void OnGetVerifyClick(UIBasicButton sender)
        {
            if (!CheckInput()) return;
            
            if (!IsResendable()) return;
            
            if (LoginRequest) return;
            LoginRequest = true;
            UILoadingLayer.GetLayer().ShowLayer(() =>
            {
                LoginRequest = false;
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("error"), 
                    this.GetLocalizedText("connection-timeout"), 
                    UITipLayer.ETipType.Bad);
            }, timeout);
            
            timerComponent.StartCountdown();
            
            // TODO: 发送验证码
            // ...
            if (!LoginPass)
            {
                GameSessionAPI.LoginAPI.SendLogin(Email, "", response =>
                {
                    if (!LoginRequest) return;
                    LoginRequest = false;
                    UILoadingLayer.GetLayer()?.HideLayer();
                    if (response.IsSuccess() || response.GetError().IsNullOrEmpty() || response.GetError().Equals(EErrorType.ERROR_NEED_VERIFIED.GetError()))
                    {
                        this.LogEditorOnly("登录验证码发送成功");
                        LoginPass = true;
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
                    if (!LoginRequest) return;
                    LoginRequest = false;
                    UILoadingLayer.GetLayer()?.HideLayer();
                    if (response.IsSuccess())
                    {
                        this.LogEditorOnly("登录验证码重新发送成功");
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

        public void OnLoginClick(UIBasicButton sender)
        {
            if (!IsValidEmail)
            {
                UILoginLayer.GetLayer()?.PopTipEmailInvalid();
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

        public void OnRegisterClick(UIBasicButton sender)
        {
            if (LoginRequest) return;
            UILoginLayer.GetLayer()?.OpenRegisterEmailModal();
        }

        #endregion

        #region Callback - WebGL Input

        private void OnReceiveInput(string data)
        {
            if (EmailReceiverInput)
            {
                EmailReceiverInput = false;
                data = MathAPI.RemoveAllWhitespace(data);
                if (MathAPI.IsValidEmail(data))
                {
                    dropdownEmail.AddAndSetOption(data);
                }
                else
                {
                    // TODO: 输入邮箱地址不合法
                    // ...
                    UILoginLayer.GetLayer()?.PopTipEmailInvalid();
                }
            }

            if (VerifyReceiveInput)
            {
                VerifyReceiveInput = false;
                if (!MathAPI.IsFixedLength(data, 6) || !MathAPI.IsNumeric(data))
                {
                    UILoginLayer.GetLayer()?.PopTipVerifyCodeInvalid();
                }
                else
                {
                    tmpVerify.SetText(data);
                    Verify = data;
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
