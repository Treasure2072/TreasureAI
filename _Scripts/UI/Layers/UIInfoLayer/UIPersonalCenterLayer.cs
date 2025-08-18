using _Scripts.UI.Common;
using Data;
using DragonLi.Core;
using DragonLi.UI;
using DragonLi.WebGL;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UIPersonalCenterLayer : UILayer
    {
        #region Property

        [SerializeField] private TextMeshProUGUI tmpName;
        [SerializeField] private TextMeshProUGUI tmpEmail;
        [SerializeField] private TextMeshProUGUI tmpInviteEmail;
        [SerializeField] private TextMeshProUGUI tmpCoins;
        [SerializeField] private TextMeshProUGUI tmpToken;
        [SerializeField] private TextMeshProUGUI tmpUSDT;
        [SerializeField] private TextMeshProUGUI tmpInviteCode;

        private WebGLInputComponent Input { get; set; }
        
        private bool ReceiverInput { get; set; }
        
        #endregion
        
        #region Unity
        
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

        private void Awake()
        {
            Input = GetComponent<WebGLInputComponent>();
            PlayerSandbox.Instance.RegisterAndLoginHandler.OnPlayerNameChanged += OnPlayerNameChanged;
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChange;
        }

        private void OnDestroy()
        {
            PlayerSandbox.Instance.RegisterAndLoginHandler.OnPlayerNameChanged -= OnPlayerNameChanged;
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChange;
        }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnClose"].As<UIBasicButton>().OnClickEvent.AddListener(OnCloseClick);
            this["BtnCopyInvite"].As<UIBasicButton>().OnClickEvent.AddListener(OnCopyInviteClick);
            this["Cell-Logout"].As<UIBasicButton>().OnClickEvent.AddListener(OnLogoutClick);
            this["BtnModifyName"].As<UIBasicButton>().OnClickEvent.AddListener(OnModifyNameClick);
            this["Cell-ContactUs"].As<UIBasicButton>().OnClickEvent.AddListener(OnContactUsClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            UIChessboardLayer.HideLayer();
            UIStaticsLayer.HideUIStaticsLayer();
            UIActivityLayer.HideUIActivityLayer();
            RefreshContent();
            // UIManager.Instance.GetLayer("UIBlackScreen").Hide();
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            // UIManager.Instance.GetLayer("UIBlackScreen").Show();
            // CoroutineTaskManager.Instance.WaitSecondTodo(UIManager.Instance.GetLayer("UIBlackScreen").Hide, 3f);
            // CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            // {
            //     UIChessboardLayer.ShowLayer();
            //     UIStaticsLayer.ShowUIStaticsLayer();
            //     UIActivityLayer.ShowUIActivityLayer();
            // }, 2f);
            
            UIChessboardLayer.ShowLayer();
            UIStaticsLayer.ShowUIStaticsLayer();
            UIActivityLayer.ShowUIActivityLayer();
        }

        #endregion

        #region Function

        private void RefreshContent()
        {
            SetUserName(PlayerSandbox.Instance.RegisterAndLoginHandler.Name);
            SetEmail(PlayerSandbox.Instance.RegisterAndLoginHandler.Email);
            SetInviteName(PlayerSandbox.Instance.RegisterAndLoginHandler.InviterName);
            SetCoins(PlayerSandbox.Instance.CharacterHandler.Coin);
            SetToken(PlayerSandbox.Instance.CharacterHandler.Token);
            SetUSDT(PlayerSandbox.Instance.CharacterHandler.USDT);
            SetInviteCode(PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode);
        }
        

        private void SetUserName(string user)
        {
            tmpName.SetText(user);
        }

        private void SetEmail(string email)
        {
            tmpEmail.SetText($"{this.GetLocalizedText("email")} : {email}");
        }

        private void SetInviteName(string email)
        {
            tmpInviteEmail.SetText($"{this.GetLocalizedText("introducer")} : {email}");
        }

        private void SetCoins(int coins)
        {
            tmpCoins.SetText(NumberUtils.GetDisplayNumberString(coins));
        }

        private void SetToken(float token)
        {
            tmpToken.SetText(NumberUtils.GetDisplayNumberString(token, 1));
        }

        private void SetUSDT(float usdt)
        {
            tmpUSDT.SetText(usdt.ToString("0.00"));
        }

        private void SetInviteCode(string code)
        {
            tmpInviteCode.SetText(string.Format(this.GetLocalizedText("invite-code-fmt"), code));
        }

        #endregion
        
        #region API

        public static UIPersonalCenterLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIPersonalCenterLayer>("UIPersonalCenterLayer");
            Debug.Assert(layer);
            return layer;
        }

        #endregion

        #region Callback

        private void OnCopyInviteClick(UIBasicButton sender)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLExternalAPI.CopyToClipboard(PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode);
#elif UNITY_ANDROID
            DragonLi.Android.AndroidExternalAPI.CopyToClipboard(PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode);
#else
            GUIUtility.systemCopyBuffer = PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode;
#endif
            UITipLayer.DisplayTip(this.GetLocalizedText("succeed"), this.GetLocalizedText("copy-succeed"));
            
        }

        private void OnLogoutClick(UIBasicButton sender)
        {
            UIConfirmLayer.DisplayConfirmation(this.GetLocalizedText("log-out-des"), option =>
            {
                if (!option) return;
                AudioManager.Instance.StopAllSound(0.5f);
                UIManager.Instance.GetLayer("UIBlackScreen").Show();
                DragonLi.Core.SceneManager.Instance.AddSceneToLoadQueueByName("StartScene", 0.5f);
                DragonLi.Core.SceneManager.Instance.StartLoad();
            });
        }

        private void OnModifyNameClick(UIBasicButton sender)
        {
            ReceiverInput = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            Input.OpenInput();
#else
            UIInputLayer.GetLayer().Show();
#endif
        }

        private void OnContactUsClick(UIBasicButton sender)
        {
            UITipLayer.DisplayTip(this.GetLocalizedText("notice"), 
                string.Format(
                    this.GetLocalizedText("contact-us-des-fmt"), 
                    URLInstance.Instance.URLSettings.email)
                );
        }

        private void OnCloseClick(UIBasicButton sender)
        {
            Hide();
        }
        
        private void OnLanguageChange(string preVal, string newVal)
        {
            RefreshContent();
        }

        private void OnPlayerNameChanged(string preVal, string newVal)
        {
            SetUserName(newVal);
        }

        private void OnReceiveInput(string data)
        {
            if(!ReceiverInput) return;
            ReceiverInput = false;

            if (!MathAPI.IsLengthInRange(data, 2, 16))
            {
                UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                    string.Format(this.GetLocalizedText("input-invalid-length-range-fmt"), 2, 16),
                    UITipLayer.ETipType.Bad);
            }
            else
            {
                GameSessionAPI.LoginAPI.ChangeName(PlayerSandbox.Instance.RegisterAndLoginHandler.Id, data, response =>
                {
                    if (response.IsSuccess())
                    {
                        PlayerSandbox.Instance.RegisterAndLoginHandler.Name = data;
                        PlayerSandbox.Instance.RegisterAndLoginHandler.SaveToLocal();
                    }
                    else
                    {
                        ServerError.PopTipError(response.GetError());
                    }
                });
            }
        }

        #endregion
    }
}
