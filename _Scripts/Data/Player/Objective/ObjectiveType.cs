using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Utils;
using Game;
using UnityEngine;

namespace Data
{
    public static class ObjectiveType
    {
        /// <summary>
        /// 获取任务进度
        /// </summary>
        /// <param name="objective">任务列表</param>
        /// <param name="id">任务 id</param>
        /// <returns>进度</returns>
        public static int GetProgressById(this FObjectiveDaily objective, string id)
        {
            return objective.progress.GetValueOrDefault(id, 0);
        }

        public static int GetMaxProgressById(this List<FObjectiveDailyInfo> infos, string id)
        {
            return (from info in infos where info.id == id select info.objective).FirstOrDefault();
        }

        public static FObjectiveDailyInfo GetDailyInfoById(this List<FObjectiveDailyInfo> infos, string id)
        {
            return (from info in infos where info.id == id select info).FirstOrDefault();
        }

        /// <summary>
        /// 任务是否完成
        /// </summary>
        /// <param name="objective">任务列表</param>
        /// <param name="id">任务 id</param>
        /// <returns>完成？</returns>
        public static bool IsCompletedById(this FObjectiveDaily objective, string id)
        {
            return GetProgressById(objective, id) >= PlayerSandbox.Instance.ChessBoardHandler.Objectives.GetMaxProgressById(id);
        }

        /// <summary>
        /// 任务奖励是否被领取
        /// </summary>
        /// <param name="objective">任务列表</param>
        /// <param name="id">任务 id</param>
        /// <returns>领取？</returns>
        public static bool IsCollectedById(this FObjectiveDaily objective, string id)
        {
            return objective.rewarded.Contains(id);
        }

        /// <summary>
        /// 领取每日任务奖励
        /// </summary>
        /// <param name="objective"></param>
        /// <param name="id"></param>
        public static void CompletedById(this FObjectiveDaily objective, string id)
        {
            // 修改本地领取数据
            objective.rewarded = new List<string>(objective.rewarded);
            objective.rewarded.Add(id);
            PlayerSandbox.Instance.ObjectiveHandler.Daily = objective;

            // 修改本地分数
            // var weekly = PlayerSandbox.Instance.ObjectiveHandler.Weekly;
            // weekly.score += MissionInstance.Instance.Settings.GetMissionDaily(id).score;
            // PlayerSandbox.Instance.ObjectiveHandler.Weekly = weekly;
        }

        /// <summary>
        /// 本地数据修改 - 更新每日任务进度
        /// </summary>
        /// <param name="objective"></param>
        /// <param name="id">任务id</param>
        /// <param name="progress">进度</param>
        public static void AddProgressDailyById(this FObjectiveDaily objective, string id, int progress)
        {
            var maxProgress = PlayerSandbox.Instance.ChessBoardHandler.Objectives.GetMaxProgressById(id);
            var curProgress = objective.progress.GetValueOrDefault(id, 0);
            if(curProgress >= maxProgress) return;
            
            objective.progress = new Dictionary<string, int>(objective.progress);
            if (!objective.progress.TryAdd(id, progress))
            {
                objective.progress[id] = Math.Clamp(objective.progress[id] + progress, 0, maxProgress);
            }
            
            PlayerSandbox.Instance.ObjectiveHandler.Daily = objective;
            
        }

        /// <summary>
        /// id 周任务领取是否解锁
        /// </summary>
        /// <param name="objective"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete("周任务已弃用")]
        public static bool IsUnlockById(this FObjectiveWeekly objective, int id)
        {
            return id switch
            {
                1 => objective.score >= 20,
                2 => objective.score >= 40,
                3 => objective.score >= MissionInstance.Instance.Settings.GetMaxScore(),
                _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
            };
        }

        /// <summary>
        /// 判断当前日是否已被领取
        /// </summary>
        /// <param name="objective"></param>
        /// <param name="dts"></param>
        /// <returns></returns>
        [Obsolete("已弃用")]
        public static bool IsCollected(this FObjectiveCheckIn objective, int dts)
        {
            return objective.loginDays.Any(loginDts => loginDts == dts);
        }

        /// <summary>
        /// 是否能被领取
        /// </summary>
        /// <param name="daily"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool CanClaim(this FObjectiveDaily daily, string id)
        {
            return daily.IsCompletedById(id) && !daily.IsCollectedById(id);
        }

        public static int GetCanClaimNumber(this FObjectiveDaily daily)
        {
            return daily.progress.Count(progress => daily.CanClaim(progress.Key));
        }

        public static bool CanSign(this FObjectiveCheckIn checkIn)
        {
            var loginDays = checkIn.loginDays;
            for (var day = 0; day < 7; day++)
            {
                if (loginDays.Count == day && (loginDays.Count == 0 || loginDays[^1] == TimeAPI.GetVietnamDayNumber() - 1))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsCompletedById(this FObjectiveAccount account, EFollowSocialPlatform type)
        {
            if (account.missions.ContainsKey((int)type))
            {
                return account.missions[(int)type];
            }

            return false;
        }
        
        public static bool IsCollectedById(this FObjectiveAccount account, EFollowSocialPlatform type)
        {
            if (account.rewardStatus.ContainsKey((int)type))
            {
                return account.rewardStatus[(int)type];
            }

            return false;
        }
        
        public static bool CanClaim(this FObjectiveAccount account, EFollowSocialPlatform type)
        {
            return account.IsCompletedById(type) && !account.IsCollectedById(type);
        }
        
        public static void CompletedById(this FObjectiveAccount account, EFollowSocialPlatform type)
        {
            // 修改本地领取数据
            account.rewardStatus = new Dictionary<int, bool>(account.rewardStatus);
            account.rewardStatus.TryAdd((int)type, true);
            PlayerSandbox.Instance.ObjectiveHandler.Account = account;
        }

        public static void AddProgressById(this FObjectiveAccount account, EFollowSocialPlatform type)
        {
            account.missions = new Dictionary<int, bool>(account.missions);
            account.missions.TryAdd((int)type, true);
            PlayerSandbox.Instance.ObjectiveHandler.Account = account;
        }

        public static bool CanClaim(this List<FObjectivePaymentInfo> infos, int index)
        {
            //超出奖励档次
            if(infos.Count <= index) return false;
            
            // 当前领取的不是当前档次
            if (PlayerSandbox.Instance.ObjectiveHandler.PaymentRewardIndex != index) return false;
            
            var info = PlayerSandbox.Instance.ChessBoardHandler.PaymentInfos.GetPaymentByIndex(index);
            var recharge = PlayerSandbox.Instance.ObjectiveHandler.Recharge;
            return recharge >= info.sum;
        }
        
        public static FObjectivePaymentInfo GetPaymentLimitByRecharge(this List<FObjectivePaymentInfo> infos, float recharge)
        {
            for (var i = 0; i < infos.Count; i++)
            {
                if(infos[i].sum > recharge) return infos[i];
            }
            
            return infos[^1];
        }

        public static FObjectivePaymentInfo GetPaymentByIndex(this List<FObjectivePaymentInfo> infos, int index)
        {
            return index >= infos.Count ? infos[^1] : infos[index];
        }
    }

    /// <summary>
    /// 每日签到数据配置
    /// id : 1 ~ 7 代表星期一到星期日
    /// </summary>
    [System.Serializable]
    public struct FCheckInInfo
    {
        public int id;
        public int coin;
        public int dice;
        public float token;
    }
    
    [Serializable]
    public struct FObjectiveDailyInfo
    {
        public string id;
        public int objective;
        public int coin;
        public int dice;
        public float token;
    }

    [System.Serializable]
    public struct FObjectivePaymentInfo
    {
        public float sum;
        public long token;
    }
}