using System;
using DragonLi.Core;
using DragonLi.Network;

namespace Game
{
    public class GSLoginAPI
    {
        #region API
        
        public bool SendVisitorLogin(string id, Action<HttpResponseProtocol> onResponse = null)
        {
            this.LogEditorOnly($"尝试游客登录 id = {id}");
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestBody = new HttpRequestProtocol();
            requestBody.AddBodyParams("id", id);
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var loginRequest = new HttpRequest<HttpResponseProtocol>();
            return loginRequest
                .SetBody(requestBody)
                .SetMethod(EHttpRequestMethod.Post)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + "gateway/login-tur")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }
        
        public bool SendLogin(string email, string token, Action<HttpResponseProtocol> onResponse = null)
        {
            this.LogEditorOnly($"尝试登录 email = {email}, token = {token}");
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestBody = new HttpRequestProtocol();
            requestBody.AddBodyParams("email", email);
            requestBody.AddBodyParams("token", token);
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var loginRequest = new HttpRequest<HttpResponseProtocol>();
            return loginRequest
                .SetBody(requestBody)
                .SetMethod(EHttpRequestMethod.Post)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + "gateway/login")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }

        public bool SendRegister(string email, string inviteCode, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestBody = new HttpRequestProtocol();
            requestBody.AddBodyParams("email", email);
            requestBody.AddBodyParams("code", inviteCode);
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var registerRequest = new HttpRequest<HttpResponseProtocol>();
            return registerRequest
                .SetBody(requestBody)
                .SetMethod(EHttpRequestMethod.Post)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + "gateway/register")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }

        public bool SendVerify(string email, string code, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestBody = new HttpRequestProtocol();
            requestBody.AddBodyParams("email", email);
            requestBody.AddBodyParams("code", code);
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var verifyRequest = new HttpRequest<HttpResponseProtocol>();
            return verifyRequest
                .SetBody(requestBody)
                .SetMethod(EHttpRequestMethod.Post)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + "gateway/verify")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }
        
        public bool SendResend(string email, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestBody = new HttpRequestProtocol();
            requestBody.AddBodyParams("email", email);
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var verifyRequest = new HttpRequest<HttpResponseProtocol>();
            return verifyRequest
                .SetBody(requestBody)
                .SetMethod(EHttpRequestMethod.Post)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + "gateway/resend")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }

        public bool ChangeName(string id, string newName, Action<HttpResponseProtocol> onResponse = null)
        {
            var connection = Settings.GetConfiguration().GetConnectionConfiguration();
            var requestBody = new HttpRequestProtocol();
            requestBody.AddBodyParams("id", id);
            requestBody.AddBodyParams("name", newName);
            var timeNow = (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var verifyRequest = new HttpRequest<HttpResponseProtocol>();
            return verifyRequest
                .SetBody(requestBody)
                .SetMethod(EHttpRequestMethod.Post)
                .SetHeader("secret", TextCryptoUtils.GenerateDynamicPassword_Sha256(timeNow))
                .SetUrl(connection.httpServer + "gateway/changeName")
                .AddCallback(onResponse)
                .SendRequestAsync();
        }

        #endregion
    }
}