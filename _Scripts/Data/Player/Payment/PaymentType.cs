using UnityEngine;

namespace Data
{
    public class PaymentType
    {

    }

    public enum ECurrencyType
    {
        USDT = 0,
    }

    public enum ECurrencyChains
    {
        Bsc = 0,
        Ton = 1,
    }

    public enum EPaymentStatus
    {
        /// <summary>
        /// 失败
        /// </summary>
        Failed = -2,
        
        /// <summary>
        /// 取消
        /// </summary>
        Canceled = -1,
        
        /// <summary>
        /// 已创建
        /// </summary>
        Created = 0,
        
        /// <summary>
        /// 已支付
        /// </summary>
        Paid = 1,
    }

    [System.Serializable]
    public struct FPayment
    {
        public long index;
        public string paymentId;
        public string id;
        public string email;
        public string account;
        public string product;
        public int currency;
        public int chain;
        public int platform;
        public string hash;
        public int status;
        public int time;
        public int pay_time;

        public ECurrencyType GetCurrencyType()
        {
            return (ECurrencyType)currency;
        }

        public ECurrencyChains GetCurrencyChain()
        {
            return (ECurrencyChains)chain;
        }

        public EPaymentStatus GetPaymentStatus()
        {
            return (EPaymentStatus)status;
        }
    }
}