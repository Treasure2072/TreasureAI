using System.Collections.Generic;
using System.Text.RegularExpressions;
using _Scripts.UI.Common;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using DragonLi.WebGL;
using TMPro;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UIBankLayer : UILayer, IMessageReceiver
    {
        #region Properties
        [Header("Settings")]
        [SerializeField] private Color colorNobody = Color.white;
        [SerializeField] private Color colorPerson = Color.green;
        [SerializeField] private Sprite iconNobody;
        [SerializeField] private Sprite iconPerson;
        

        [Header("References")] 
        [SerializeField] private UIAnimatedNumberText tmpDeposit;
        [SerializeField] private TextMeshProUGUI tmpCode;
        [SerializeField] private UIInvitee[] invitees;

        [SerializeField] private GameObject objCopy;
        [SerializeField] private GameObject objOkay;

        [SerializeField] private GameObject objWithdrawDes;
        [SerializeField] private GameObject objBtnWithdraw;
        
        private List<FEntityUser> Data { get; set; }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnWithdraw"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnWithdrawClickCallback);
            this["PasteBtn"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnPasteClickCallback);
            this["BtnClose"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnHideClickCallback);
            
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
        }

        protected override void OnShow()
        {
            base.OnShow();
            PlayerSandbox.Instance.ChessBoardHandler.OnInvestChanged += OnInvestChanged;
            PlayerSandbox.Instance.ChessBoardHandler.OnChildrenChanged += OnChildrenChanged;
        }

        protected override void OnHide()
        {
            base.OnHide();
            PlayerSandbox.Instance.ChessBoardHandler.OnInvestChanged -= OnInvestChanged;
            PlayerSandbox.Instance.ChessBoardHandler.OnChildrenChanged -= OnChildrenChanged;
        }

        protected override void OnRefesh()
        {
            base.OnRefesh();
            
            SetDeposit(PlayerSandbox.Instance.ChessBoardHandler.InvestCoin);
            SetCode(PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode);
            SetUserInfo(PlayerSandbox.Instance.ChessBoardHandler.Children);

            ActiveDeposit();
            ActiveCopy();
        }

        #endregion

        #region Function
        
        private void ActiveDeposit()
        {
            objWithdrawDes.gameObject.SetActive(!CanWithdraw());
            objBtnWithdraw.gameObject.SetActive(CanWithdraw());
        }

        private void ActiveCopy()
        {
            if (objCopy.activeSelf && !objCopy.activeSelf) return;
            objCopy.SetActive(true);
            objOkay.SetActive(false);
        }

        private void ActiveOkay(float autoDelayClose = -1f)
        {
            if(objOkay.activeSelf && !objOkay.activeSelf) return;
            objCopy.SetActive(false);
            objOkay.SetActive(true);

            if (autoDelayClose > 0)
            {
                CoroutineTaskManager.Instance.WaitSecondTodo(ActiveCopy, autoDelayClose);
            }
        }

        private void SetDeposit(int deposit)
        {
            tmpDeposit.SetNumber(deposit);
        }

        private void SetCode(string code)
        {
            Debug.Assert(tmpCode);
            tmpCode.text = string.Format(this.GetLocalizedText("invite-code-fmt"), code);
        }
        
        private void SetUserInfo(List<FEntityUser> data)
        {
            Data = data;
            for (var i = 0; i < invitees.Length; i++)
            {
                if (i < data.Count)
                {
                    invitees[i].SetInvitee(data[i].name, true);
                    // invitees[i].SetColor(colorPerson);
                    continue;
                }
                
                invitees[i].SetInvitee(this.GetLocalizedText("invite"), false);
                // invitees[i].SetColor(colorNobody);
            }
        }

        private bool CanWithdraw()
        {
            return Data.Count >= invitees.Length;
        }

        #endregion

        #region API

        public static UIBankLayer GetUIBankLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIBankLayer>("UIBankLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowLayer()
        {
            GetUIBankLayer()?.Show();
        }

        public static void HideLayer()
        {
            GetUIBankLayer()?.Hide();
        }

        #endregion

        #region Callback

        private void OnPasteClickCallback(UIBasicButton sender)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLExternalAPI.CopyToClipboard(PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode);
#elif UNITY_ANDROID
            DragonLi.Android.AndroidExternalAPI.CopyToClipboard(PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode);
#else
            GUIUtility.systemCopyBuffer = PlayerSandbox.Instance.RegisterAndLoginHandler.InviteCode;
#endif
            ActiveOkay();
        }

        private void OnHideClickCallback(UIBasicButton sender)
        {
            Hide();
        }

        private void OnWithdrawClickCallback(UIBasicButton sender)
        {
            if (!CanWithdraw()) return;
            
            UILoadingLayer.GetLayer().ShowLayer(() =>
            {
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("error"), 
                    this.GetLocalizedText("connection-timeout"), 
                    UITipLayer.ETipType.Bad);
            }, 30);
            GameSessionAPI.ChessBoardAPI.WithdrawBank(PlayerSandbox.Instance.RegisterAndLoginHandler.Email);
        }

        #endregion

        #region Callback - Data Changed

        private void OnInvestChanged(int? preValue, int newValue)
        {
            Refresh();
        }
        
        private void OnChildrenChanged(List<FEntityUser> preValue, List<FEntityUser> newValue)
        {
            Refresh();
        }

        #endregion

        #region Callback - Socket Receiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if(GameSessionAPI.ChessBoardAPI.ServiceName != service) return;
            if (GSChessBoardAPI.MethodWithdrawBank == method)
            {
                if (response.IsSuccess())
                {
                    var coin = response.GetAttachmentAsInt("coin");
                    if (coin != -1)
                    {
                        PlayerSandbox.Instance.CharacterHandler.Coin += coin;
                    }
                    GameSessionAPI.ChessBoardAPI.QueryBank(PlayerSandbox.Instance.RegisterAndLoginHandler.Email);
                }
                else
                {
                    UITipLayer.DisplayTip(
                        this.GetLocalizedText("error"),
                        this.GetLocalizedText(response.GetError()),
                        UITipLayer.ETipType.Bad);
                }
            }
            else if (GSChessBoardAPI.MethodQueryBank == method)
            {
                if (response.IsSuccess())
                {
                    PlayerSandbox.Instance.ChessBoardHandler.OnReceiveMessage(response, service, method);
                    UILoadingLayer.GetLayer()?.HideLayer();
                    UITipLayer.DisplayTip(this.GetLocalizedText("notice"), this.GetLocalizedText("withdraw-succeed"), UITipLayer.ETipType.Good);
                }
                else
                {
                    UITipLayer.DisplayTip(
                        this.GetLocalizedText("error"),
                        this.GetLocalizedText(response.GetError()),
                        UITipLayer.ETipType.Bad);
                }
            }
        }

        #endregion
    }
}