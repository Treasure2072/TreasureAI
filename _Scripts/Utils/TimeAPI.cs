using System;
using System.Collections.Generic;
using DragonLi.WebGL;

namespace _Scripts.Utils
{
    public static class TimeAPI
    {
        public static int GetUtcTimeStamp()
        {
            return (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        
        /// <summary>
        /// 越南时间戳
        /// </summary>
        /// <returns></returns>
        public static int GetVietnamTimeStamp()
        {
            return GetUtcTimeStamp() + 7 * 3600;
#if UNITY_WEBGL && !UNITY_EDITOR
            return WebGLExternalAPI.GetVietnamTimeStamp();
#else
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var vietnamTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, vietnamTimeZone);
            return (int)vietnamTime.ToUnixTimeSeconds();
#endif
        }

        /// <summary>
        /// 越南
        /// </summary>
        /// <returns></returns>
        public static int GetVietnamDayNumber()
        {
            return GetVietnamTimeStamp() / 86400;
        }
        
        public static DayOfWeek GetWeekdayFromDayNumber(int dts)
        {
            var date = new DateTime(1970, 1, 1).AddDays(dts).ToLocalTime();
            return date.DayOfWeek;
        }

        public static DateTime ConvertVietnamTimestampToLocalTime(long timestamp, bool isMilliseconds = false)
        {
            DateTimeOffset dateTimeOffset = isMilliseconds
                ? DateTimeOffset.FromUnixTimeMilliseconds(timestamp)
                : DateTimeOffset.FromUnixTimeSeconds(timestamp);
            
            // 越南时间是 UTC+7
            TimeSpan vietnamOffset = TimeSpan.FromHours(7);
            return dateTimeOffset.ToOffset(vietnamOffset).DateTime;
        }

        public static List<int> GetCurrentWeekUtcDays()
        {
            var today = DateTime.UtcNow.Date;
            var todayDts = (int)(today - new DateTime(1970, 1, 1)).TotalDays;
            var dayOfWeek = (int)today.DayOfWeek;
            
            // 将 Sunday=0 调整为 7
            if (dayOfWeek == 0) dayOfWeek = 7;
            var mondayDts = todayDts - (dayOfWeek - 1);

            var result = new List<int>();
            for (var i = 0; i < 7; i++)
            {
                result.Add(mondayDts + i);
            }

            return result;
        }
    }
}
