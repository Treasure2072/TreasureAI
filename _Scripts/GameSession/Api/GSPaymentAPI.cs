using System;
using _Scripts.Utils;
using DragonLi.Core;
using DragonLi.Network;
using UnityEngine;

namespace Game
{
    public class GSPaymentAPI
    {
        /// <summary>
        /// 创建订单
        /// 目前有3个状态：PAID, FAILED, CANCEL
        /// </summary>
        /// <param name="product">产品，传入shop里面产品的id，如dice-pack-01</param>
        /// <param name="account">用户的区块链付款账户，通过连接钱包获得</param>
        /// <param name="email">用户的Email</param>
        /// <param name="platform">平台，传入钱包的种类（如果可以获取的话）</param>
        /// <param name="onResponse"></param>
        /// <returns>返回 payment-id, amount（应付金额）, success（是否成功）</returns>
        public bool CreatePayment(string product, string account, string email, string platform, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestProtocol = new HttpRequestProtocol();
            requestProtocol.AddBodyParams("product", product);
            requestProtocol.AddBodyParams("account", account);
            requestProtocol.AddBodyParams("email", email);
            requestProtocol.AddBodyParams("platform", platform);
            return new HttpRequest<HttpResponseProtocol>()
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(TimeAPI.GetUtcTimeStamp()))
                .SetMethod(EHttpRequestMethod.Post)
                .SetBody(requestProtocol)
                .SetUrl(connection.httpServer + "payment/create")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }

        /// <summary>
        /// 更新支付状态, 再次尝试支付
        /// </summary>
        /// <param name="id">订单号</param>
        /// <param name="hash">交易号</param>
        /// <param name="onResponse"></param>
        /// <returns>返回 无参数</returns>
        public bool UpdatePayment(string id, string hash, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestProtocol = new HttpRequestProtocol();
            requestProtocol.AddBodyParams("id", id);
            // requestProtocol.AddBodyParams("hash", hash);
            return new HttpRequest<HttpResponseProtocol>()
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(TimeAPI.GetUtcTimeStamp()))
                .SetMethod(EHttpRequestMethod.Post)
                .SetBody(requestProtocol)
                .SetUrl(connection.httpServer + "payment/pay")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }

        /// <summary>
        /// 查询订单状态 
        /// </summary>
        /// <param name="id">订单号</param>
        /// <param name="onResponse"></param>
        /// <returns>FPayment</returns>
        public bool QueryPaymentById(string id, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestProtocol = new HttpRequestProtocol();
            return new HttpRequest<HttpResponseProtocol>()
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(TimeAPI.GetUtcTimeStamp()))
                .SetMethod(EHttpRequestMethod.Get)
                .SetBody(requestProtocol)
                .SetUrl(connection.httpServer + $"payment/query-status?id={id}")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }

        /// <summary>
        ///  查询用户订单
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="onResponse"></param>
        /// <returns>FPayment 列表</returns>
        public bool QueryPaymentByUserId(string id, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestProtocol = new HttpRequestProtocol();
            return new HttpRequest<HttpResponseProtocol>()
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(TimeAPI.GetUtcTimeStamp()))
                .SetMethod(EHttpRequestMethod.Get)
                .SetBody(requestProtocol)
                .SetUrl(connection.httpServer + $"payment/query?id={id}")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id">订单号</param>
        /// <param name="onResponse"></param>
        /// <returns></returns>
        public bool CancelPayment(string id, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestProtocol = new HttpRequestProtocol();
            requestProtocol.AddBodyParams("id", id);
            return new HttpRequest<HttpResponseProtocol>()
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(TimeAPI.GetUtcTimeStamp()))
                .SetMethod(EHttpRequestMethod.Post)
                .SetBody(requestProtocol)
                .SetUrl(connection.httpServer + "payment/cancel")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }
    }
}