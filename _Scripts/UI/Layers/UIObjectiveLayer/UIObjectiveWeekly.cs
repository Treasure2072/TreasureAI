using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Utils;
using Data;
using DragonLi.Network;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UIObjectiveWeekly : UIComponent
    {
        #region Properties

        [Header("References")] 
        [SerializeField] private TextMeshProUGUI textRemainTime;
        [SerializeField] private Image imgFill;
        [SerializeField] private Button btn20;
        [SerializeField] private Button btn40;
        [SerializeField] private Button btn70;
        
        /// <summary>
        /// 20 分是否被领取
        /// 已领取：true
        /// 未领取：false
        /// </summary>
        private bool Reward20 { get; set; }
        
        /// <summary>
        /// 40 分是否被领取
        /// 已领取：true
        /// 未领取：false
        /// </summary>
        private bool Reward40 { get; set; }
        
        /// <summary>
        /// 70 分是否被领取
        /// 已领取：true
        /// 未领取：false
        /// </summary>
        private bool Reward70 { get; set; }

        #endregion

        #region Unity

        private void FixedUpdate()
        {
            // SetRemainingTime(PlayerSandbox.Instance.ObjectiveHandler.GetWeekFinishTimeStamp());
        }

        #endregion

        #region UIComponent

        public override void Init()
        {
            base.Init();
            btn20.onClick.AddListener(() => OnClaimCallback("1"));
            btn40.onClick.AddListener(() => OnClaimCallback("2"));
            btn70.onClick.AddListener(() => OnClaimCallback("3"));
            PlayerSandbox.Instance.ObjectiveHandler.ObjectiveWeeklyChanged += OnObjectiveWeeklyChanged;
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessageCallback;
        }

        public override void OnShow()
        {
            base.OnShow();
            
            SetFillAmount(PlayerSandbox.Instance.ObjectiveHandler.Weekly.score / (MissionInstance.Instance.Settings.GetMaxScore() * 1.0f));
            SetClaims(PlayerSandbox.Instance.ObjectiveHandler.Weekly.rewarded);
            // SetRemainingTime(PlayerSandbox.Instance.ObjectiveHandler.GetWeekFinishTimeStamp());
        }

        #endregion

        #region Function

        private void SetRemainingTime(int finishTimeStamp)
        {
            var remainTime = finishTimeStamp - TimeAPI.GetUtcTimeStamp();
            var day = remainTime / 12 / 3600;
            var hour = (remainTime % (12 * 3600)) / 3600;
            var minute = (remainTime % 3600) / 60;
            var second = remainTime % 60;

            var timeString = $"{day}{this.GetLocalizedText("day-acronym")}{hour}{this.GetLocalizedText("hour-acronym")}{minute}{this.GetLocalizedText("minute-acronym")}{second}{this.GetLocalizedText("seconds-acronym")}";
            textRemainTime.text = string.Format(this.GetLocalizedText("objective-weekly-remain-time-fmt"), timeString);
        }

        private void SetFillAmount(float fillAmount)
        {
            imgFill.fillAmount = fillAmount;
        }

        private void SetClaims(IReadOnlyCollection<int> rewards)
        {
            Reward20 = rewards.Contains(1);
            Reward40 = rewards.Contains(2);
            Reward70 = rewards.Contains(3);
        }

        #endregion

        #region Callback

        private void OnObjectiveWeeklyChanged(FObjectiveWeekly preValue, FObjectiveWeekly nowValue)
        {
            if (preValue.score != nowValue.score)
            {
                SetFillAmount(nowValue.score / (MissionInstance.Instance.Settings.GetMaxScore() * 1.0f));
            }

            if (preValue.rewarded.Equals(nowValue.rewarded))
            {
                SetClaims(nowValue.rewarded);
            }
        }

        private void OnClaimCallback(string rank)
        {
            if (rank.Equals("1") && (!PlayerSandbox.Instance.ObjectiveHandler.Weekly.IsUnlockById(1) || Reward20)) return;
            if (rank.Equals("2") && (!PlayerSandbox.Instance.ObjectiveHandler.Weekly.IsUnlockById(2) || Reward40)) return;
            if (rank.Equals("3") && (!PlayerSandbox.Instance.ObjectiveHandler.Weekly.IsUnlockById(3) || Reward70)) return;
            
            GameSessionAPI.ObjectiveAPI.RewardWeekly(rank);
        }
        
        private void OnReceiveMessageCallback(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess())
            {
                this.LogErrorEditorOnly(response.error);
                return;
            }
            
            if (service == GameSessionAPI.ObjectiveAPI.ServiceName && method == GSObjectiveAPI.MethodRewardWeekly)
            {
                var coin = response.GetAttachmentAsInt("coin");
                var dice = response.GetAttachmentAsInt("dice");
                var itemsString = response.GetAttachmentAsString("items");
                var rank = response.GetAttachmentAsString("rank");
                
                PlayerSandbox.Instance.CharacterHandler.Coin += coin;
                PlayerSandbox.Instance.CharacterHandler.Dice += dice;
                
                // TODO: 本地数据修改 - 周任务领取
                var tempWeekly = PlayerSandbox.Instance.ObjectiveHandler.Weekly;
                tempWeekly.rewarded = new List<int>(tempWeekly.rewarded);
                tempWeekly.rewarded.Add(int.Parse(rank));
                PlayerSandbox.Instance.ObjectiveHandler.Weekly = tempWeekly;
                
                // TODO: 本地数据修改 - 物品添加
                // ...
                var items = itemsString.Split('|');
                var tempItems = new Dictionary<string, int>(PlayerSandbox.Instance.CharacterHandler.Items);
                foreach (var itemKv in items)
                {
                    var item = itemKv.Split(':');
                    var itemName = item[0];
                    var itemCount = int.Parse(item[1]);
                    if (!tempItems.TryAdd(itemName, itemCount))
                    {
                        tempItems[itemName] += itemCount;
                    }
                }
                PlayerSandbox.Instance.CharacterHandler.Items = tempItems;
            }
        }

        #endregion
    }
}
