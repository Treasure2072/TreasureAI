using System.Text.RegularExpressions;
using UnityEngine;

namespace Game
{
    public static class MathAPI
    {
        /// <summary>
        /// 生成随机uid
        /// </summary>
        /// <returns></returns>
        public static string GenerateUUid()
        {
            return System.Guid.NewGuid().ToString();
        }
        
        /// <summary>
        /// 是否是邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        
        /// <summary>
        /// 删除字符串中的所有空白字符（包括空格、制表符、换行符等）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveAllWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return Regex.Replace(input, @"\s+", "");
        }
        
        /// <summary>
        /// 是否是只包含数字和字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(string input)
        {
            return Regex.IsMatch(input, @"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 是否长度范围内
        /// </summary>
        /// <param name="input"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsLengthInRange(string input, int min, int max)
        {
            return !string.IsNullOrEmpty(input) && input.Length >= min && input.Length <= max;
        }
        
        /// <summary>
        /// 是否固定长度
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool IsFixedLength(string input, int length)
        {
            if (input == null) return false;
            return input.Length == length;
        }
        
        /// <summary>
        /// 是否是数字
        /// 支持：正负号、整数和小数、非科学计数法格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumeric(string input)
        {
            if (input == null) return false;
            return Regex.IsMatch(input, @"^-?\d+(\.\d+)?$");
        }
    }
}