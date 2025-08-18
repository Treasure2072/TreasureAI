using System;
using System.Collections.Generic;

namespace Game
{
    public class GSObjectiveAPI : GameSessionAPIImpl
    {
        public static readonly string MethodQuery = "query";
        public static readonly string MethodRewardDaily = "reward_daily";
        public static readonly string MethodRewardWeekly = "reward_weekly";
        public static readonly string MethodRewardAccount = "reward_account";
        public static readonly string MethodRewardCharge = "reward_charge";
        
        /// <summary>
        /// 仅用于开发调试
        /// </summary>
        public static readonly string MethodComplete = "complete";
        
        /// <summary>
        /// 仅用于开发调试
        /// </summary>
        public static readonly string MethodCompleteWeek = "complete_week";
        
        /// <summary>
        /// 完成个人任务
        /// </summary>
        public static readonly string MethodCompleteAccount = "complete_account";
        
        /// <summary>
        /// 每日签到
        /// </summary>
        public static readonly string MethodCheckIn = "check_in";
        
        /// <summary>
        /// 查询任务详情，必须先调用这个方法
        /// day-exp: 每日任务结束倒计时
        /// daily: 每日任务数据
        /// account: 账号任务数据
        /// </summary>
        public void Query()
        {
            SendMessage(CreateRequest(MethodQuery));
        }

        /// <summary>
        /// 增加每日任务进度，仅用于开发调试
        /// </summary>
        public void CompleteDay(string id, int count)
        {
            var request = CreateRequest(MethodComplete);
            request.AddBodyParams("objective", id);
            request.AddBodyParams("count", count);
            SendMessage(request);
        }
        
        /// <summary>
        /// 增加每周任务进度，仅用于开发调试
        /// </summary>
        public void CompleteWeek(int score)
        { 
            var request = CreateRequest(MethodCompleteWeek);
            request.AddBodyParams("score", score);
            SendMessage(request);
        }
        
        /// <summary>
        /// 完成账号任务
        /// </summary>
        /// <param name="missionId"></param>
        public void CompleteAccount(int missionId)
        { 
            var request = CreateRequest(MethodCompleteAccount);
            request.AddBodyParams("id", missionId);
            SendMessage(request);
        }
        
        /// <summary>
        /// 领取每日奖励
        /// </summary>
        /// <param name="id"></param>
        public void RewardDaily(string id)
        {
            var request = CreateRequest(MethodRewardDaily);
            request.AddBodyParams("objective", id);
            SendMessage(request);
        }
        
        /// <summary>
        /// 领取每周进度奖励
        /// </summary>
        /// <param name="rank"></param>
        [Obsolete("周任务已弃用")]
        public void RewardWeekly(string rank)
        {
            var request = CreateRequest(MethodRewardWeekly);
            request.AddBodyParams("rank", rank);
            SendMessage(request);
        }
        
        /// <summary>
        /// 领取账户任务奖励
        /// </summary>
        /// <param name="missionId"></param>
        public void RewardAccount(int missionId)
        {
            var request = CreateRequest(MethodRewardAccount);
            request.AddBodyParams("id", missionId);
            SendMessage(request);
        }

        /// <summary>
        /// 领取每日签到奖励
        /// </summary>
        public void CompleteCheckIn()
        {
            SendMessage(CreateRequest(MethodCheckIn));
        }

        /// <summary>
        /// 领取充值奖励
        /// return:
        /// int - "level"
        /// float - "token"
        /// </summary>
        public void RewardCharge()
        {
            SendMessage(CreateRequest(MethodRewardCharge));
        }

        public void DevCompleteCheckIn(int dts)
        {
            var request = CreateRequest(MethodCheckIn);
            request.AddBodyParams("debug-day", dts);
            SendMessage(request);
        }
        
        protected override string GetServiceName()
        {
            return "objective";
        }
    }
    
    
    [Serializable]
    public struct FObjectiveDaily
    {
        public Dictionary<string, int> progress;
        public List<string> rewarded;
        public float dailyCharge;
        public int dailyChargeRewardIndex;
    }

    [Serializable]
    public struct FObjectiveCheckIn
    {
        // public int weekStartDTS;
        public List<int> loginDays;
    }
    
    [Serializable]
    public struct FObjectiveAccount
    {
        public Dictionary<int, bool> missions;
        public Dictionary<int, bool> rewardStatus;
    }
    
    [Serializable]
    public struct FObjectiveWeekly
    {
        /// <summary>
        /// 0 - 70 （20 - 40 - 70）
        /// </summary>
        public int score;
        
        /// <summary>
        /// 1，2，3
        /// </summary>
        public List<int> rewarded;
    }

    [System.Serializable]
    public enum EFollowSocialPlatform
    {
        Twitter = 0,
        Discord = 1,
        Telegram = 2,
        Youtube = 3,
    }
}