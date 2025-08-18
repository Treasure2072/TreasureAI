using System.Collections.Generic;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UIObjectiveRechargeItem : MonoBehaviour, IObjective, IMessageReceiver
    {
        #region Property

        [Header("References")]
        [SerializeField] private TextMeshProUGUI tmpDescription;
        
        [Header("Reference - Claim")]
        [SerializeField] private GameObject objCoin;
        [SerializeField] private TextMeshProUGUI tmpCoin;
        [SerializeField] private GameObject objDice;
        [SerializeField] private TextMeshProUGUI tmpDice;
        [SerializeField] private GameObject objToken;
        [SerializeField] private TextMeshProUGUI tmpToken;
        
        [Header("References - Sign")]
        [SerializeField] private Button btnGet;
        [SerializeField] private GameObject objCollected;

        private UnityAction<IObjective> OnCollectAction { get; set; }
        
        #endregion

        #region Unity

        private void Awake()
        {
            btnGet.onClick.AddListener(OnClickGet);
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged += OnLanguageChanged;
            PlayerSandbox.Instance.ObjectiveHandler.OnRechargeChanged += OnRechargeChanged;
            PlayerSandbox.Instance.ObjectiveHandler.OnPaymentRewardIndexChanged += OnPaymentRewardIndexChanged;
        }

        private void OnDestroy()
        {
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler -= OnReceiveMessage;
            SystemSandbox.Instance.LanguageHandler.OnLanguageChanged -= OnLanguageChanged;
            PlayerSandbox.Instance.ObjectiveHandler.OnRechargeChanged -= OnRechargeChanged;
            PlayerSandbox.Instance.ObjectiveHandler.OnPaymentRewardIndexChanged -= OnPaymentRewardIndexChanged;
        }

        #endregion
        
        #region API

        public void Initialize(UnityAction<IObjective> onCollectAction)
        {
            OnCollectAction = onCollectAction;
            Refresh();
        }

        #endregion

        #region Function

        private bool CanClaim(int index)
        {
            //超出奖励档次
            if(PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count <= index) return false;
            
            // 当前领取的不是当前档次
            if (PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex != index) return false;
            
            var info = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.GetPaymentByIndex(index);
            var recharge = PlayerSandbox.Instance.ObjectiveHandler.Recharge;
            return recharge >= info.sum;
        }

        private void Refresh()
        {
            SetContent();
            RefreshCollectStatus();
            SetClaimNums();
        }

        #endregion

        #region Function - Interface

        public Transform GetTransform()
        {
            return this.transform;
        }

        public void SetContent()
        {
            // 支付过的数额
            var recharge = PlayerSandbox.Instance.ObjectiveHandler.Recharge;

            // 当前可以领取的奖励等级
            var index = PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex;

            if (index >= PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count)
            {
                index = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count - 1;
            }
            // 当前等级可以领取的详细
            var info = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.GetPaymentByIndex(index);
            
            tmpDescription.SetText(string.Format(this.GetLocalizedText("shop-payment-fmt"), info.sum, recharge));
        }

        public void RefreshCollectStatus()
        {
            var index = PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex;
            var info = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.GetPaymentByIndex(index);
            var recharge = PlayerSandbox.Instance.ObjectiveHandler.Recharge;

            if (CanClaim(index))
            {
                // 可以被领取
                objCollected.SetActive(false);
                btnGet.gameObject.SetActive(true);
                btnGet.interactable = true;
            }
            else if(recharge < info.sum)
            {
                // 还未完成
                objCollected.SetActive(false);
                btnGet.gameObject.SetActive(false);
                btnGet.interactable = false;
            }
            else if(index >= PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count)
            {
                // TODO: 已经被领取
                objCollected.SetActive(true);
                btnGet.gameObject.SetActive(false);
                btnGet.interactable = false;
            }
        }

        public void SetClaimNums()
        {
            var index = PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex;
            if (index >= PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count)
            {
                index = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count - 1;
            }
            var info = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.GetPaymentByIndex(index);
            objCoin.gameObject.SetActive(false);
            objDice.gameObject.SetActive(false);
            objToken.gameObject.SetActive(info.token > 0);
            tmpToken.SetText(NumberUtils.GetDisplayNumberString(info.token, 1));
        }

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess())
            {
                this.LogErrorEditorOnly(response.error);
                return;
            }

            if (service != GameSessionAPI.ObjectiveAPI.ServiceName || method != GSObjectiveAPI.MethodRewardCharge) return;

            var rewardIndex = response.GetAttachmentAsInt("level");
            var token = response.GetAttachmentAsFloat("token");
            var tasks = new List<IQueueableEvent>
            {
                EffectsAPI.CreateTip(() => 0, () => 0, () => token),
                EffectsAPI.CreateSoundEffect(() => EffectsAPI.EEffectType.Coin),
                EffectsAPI.CreateScreenFullEffect(() => 0, () => 0, () => token),
                new CustomEvent(() =>
                {
                    PlayerSandbox.Instance.CharacterHandler.Token += Mathf.Approximately(token, -1) ? 0 : token;
                })
            };
            EventQueue.Instance.Enqueue(tasks);
            PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex = rewardIndex;
            if (rewardIndex >= PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.Count)
            {
                OnCollectAction?.Invoke(this);
            }
        }

        #endregion

        #region Callback

        /// <summary>
        /// 领取奖励
        /// </summary>
        private void OnClickGet()
        {
            var index = PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex;
            if (!CanClaim(index))
            {
                this.LogErrorEditorOnly($"不能领取 index = {index}");
                return;
            }
            
            GameSessionAPI.ObjectiveAPI.RewardCharge();
        }

        #endregion

        #region Callback - Sandbox
        
        /// <summary>
        /// 语言切换
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="current"></param>
        private void OnLanguageChanged(string previous, string current)
        {
            Refresh();
        }

        /// <summary>
        /// 充值累计金额发生变化
        /// </summary>
        /// <param name="preVal"></param>
        /// <param name="newVal"></param>
        private void OnRechargeChanged(float? preVal, float newVal)
        {
            Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preVal"></param>
        /// <param name="newVal"></param>
        private void OnPaymentRewardIndexChanged(int? preVal, int newVal)
        {
            Refresh();
        }

        #endregion

    }
}