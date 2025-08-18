using System;
using System.Collections.Generic;
using _Scripts.Utils;
using DragonLi.Core;
using DragonLi.Network;
using Game;
using Newtonsoft.Json;

namespace Data
{
    public class ObjectiveHandler : SandboxHandlerBase, IMessageReceiver
    {
        private const string kDayExpKey = "day-exp";
        private const string kWeekExpKey = "week-exp";
        private const string kObjectiveDailyKey = "objective-daily";
        private const string kObjectiveWeeklyKey = "objective-weekly";
        private const string kObjectiveCheckInKey = "objective-check-in";
        private const string kObjectiveAccountKey = "objective-account";
        private const string kObjectiveRechargeKey = "objective-recharge";
        private const string kObjectivePaymentRewardIndexKey = "objective-payment-reward-index";

        #region Properties - Event

        public event Action<int?, int> DayExpChanged;
        public event Action<int?, int> WeekExpChanged;
        public event Action<FObjectiveDaily, FObjectiveDaily> ObjectiveDailyChanged;
        public event Action<FObjectiveWeekly, FObjectiveWeekly> ObjectiveWeeklyChanged;
        public event Action<FObjectiveCheckIn, FObjectiveCheckIn> ObjectiveCheckInChanged;
        public event Action<float?, float> OnRechargeChanged;
        public event Action<int?, int> OnPaymentRewardIndexChanged;

        #endregion

        #region Properties

        private int QueryTimeStamp { get; set; }
        
        public bool IsPulledData { get; private set; }

        #endregion

        #region Properties - Data

        public int DayExp
        {
            get => SandboxValue.GetValue<int>(kDayExpKey);
            set => SandboxValue.SetValue(kDayExpKey, value);
        }

        public int WeekExp
        {
            get => SandboxValue.GetValue<int>(kWeekExpKey);
            set => SandboxValue.SetValue(kWeekExpKey, value);
        }

        public FObjectiveDaily Daily
        {
            get => SandboxValue.GetValue<FObjectiveDaily>(kObjectiveDailyKey);
            set => SandboxValue.SetValue(kObjectiveDailyKey, value);
        }

        [Obsolete("周任务已弃用")]
        public FObjectiveWeekly Weekly
        {
            get => SandboxValue.GetValue<FObjectiveWeekly>(kObjectiveWeeklyKey);
            set => SandboxValue.SetValue(kObjectiveWeeklyKey, value);
        }

        public FObjectiveCheckIn CheckIn
        {
            get => SandboxValue.GetValue<FObjectiveCheckIn>(kObjectiveCheckInKey);
            set => SandboxValue.SetValue(kObjectiveCheckInKey, value);
        }

        public FObjectiveAccount Account
        {
            get => SandboxValue.GetValue<FObjectiveAccount>(kObjectiveAccountKey);
            set => SandboxValue.SetValue(kObjectiveAccountKey, value);
        }

        /// <summary>
        /// 充值的总金额
        /// </summary>
        public float Recharge
        {
            get => SandboxValue.GetValue<float>(kObjectiveRechargeKey);
            set => SandboxValue.SetValue(kObjectiveRechargeKey, value);
        }

        /// <summary>
        ///
        /// </summary>
        public int PaymentRewardIndex
        {
            get => SandboxValue.GetValue<int>(kObjectivePaymentRewardIndexKey);
            set => SandboxValue.SetValue(kObjectivePaymentRewardIndexKey, value);
        }

        #endregion

        #region API

        /// <summary>
        /// 获取每日任务结束时间戳
        /// </summary>
        /// <returns></returns>
        public int GetDailyFinishTimeStamp()
        {
            return DayExp + QueryTimeStamp;
        }

        /// <summary>
        /// 获取每周任务结束时间戳
        /// </summary>
        /// <returns></returns>
        public int GetWeekFinishTimeStamp()
        {
            return WeekExp + QueryTimeStamp;
        }

        #endregion

        #region Function - SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            if (sandboxCallbacks == null)
            {
                throw new ArgumentNullException(nameof(sandboxCallbacks));
            }

            sandboxCallbacks[kDayExpKey] = (preValue, nowValue) => DayExpChanged?.Invoke((int?)preValue, (int)nowValue);
            sandboxCallbacks[kWeekExpKey] = (preValue, nowValue) => WeekExpChanged?.Invoke((int?)preValue, (int)nowValue);
            sandboxCallbacks[kObjectiveDailyKey] = (preValue, nowValue) => ObjectiveDailyChanged?.Invoke((FObjectiveDaily)preValue, (FObjectiveDaily)nowValue);
            sandboxCallbacks[kObjectiveWeeklyKey] = (preValue, nowValue) => ObjectiveWeeklyChanged?.Invoke((FObjectiveWeekly)preValue, (FObjectiveWeekly)nowValue);
            sandboxCallbacks[kObjectiveCheckInKey] = (preValue, nowValue) => ObjectiveCheckInChanged?.Invoke((FObjectiveCheckIn)preValue, (FObjectiveCheckIn)nowValue);
            sandboxCallbacks[kObjectiveRechargeKey] = (preValue, nowValue) => OnRechargeChanged?.Invoke((float?)preValue, (float)nowValue);
            sandboxCallbacks[kObjectivePaymentRewardIndexKey] = (preValue, nowValue) => OnPaymentRewardIndexChanged?.Invoke((int?)preValue, (int)nowValue);
        }

        protected override void OnInit()
        {
            base.OnInit();
            IsPulledData = false;
            
            GameSessionAPI.ObjectiveAPI.Query();
        }

        #endregion

        #region Function - IMessageReceiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (service == GameSessionAPI.ObjectiveAPI.ServiceName && method == GSObjectiveAPI.MethodQuery)
            {
                QueryTimeStamp = TimeAPI.GetUtcTimeStamp();
                var checkinJson = response.GetAttachmentAsString("check-in");
                var dailyJson = response.GetAttachmentAsString("daily");
                var accountJson = response.GetAttachmentAsString("account");
                
                DayExp = response.GetAttachmentAsInt("day-exp");
                CheckIn = JsonConvert.DeserializeObject<FObjectiveCheckIn>(checkinJson);
                Daily = JsonConvert.DeserializeObject<FObjectiveDaily>(dailyJson);
                Account = JsonConvert.DeserializeObject<FObjectiveAccount>(accountJson);
                Recharge = Daily.dailyCharge;
                PaymentRewardIndex = Daily.dailyChargeRewardIndex;
                IsPulledData = true;
                
                Daily.progress.TryAdd("coin-01", 0);
                Daily.progress.TryAdd("coin-02", 0);
                Daily.progress.TryAdd("coin-03", 0);
                Daily.progress.TryAdd("coin-04", 0);
                Daily.progress.TryAdd("coin-05", 0);
                Daily.progress.TryAdd("dice-01", 0);
                Daily.progress.TryAdd("dice-02", 0);
                Daily.progress.TryAdd("dice-03", 0);
                Daily.progress.TryAdd("dice-04", 0);
                Daily.progress.TryAdd("dice-05", 0);
                
            }
        }

        #endregion
    }
}