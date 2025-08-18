using System.Collections.Generic;
using _Scripts.UI.Common;
using Data;
using DragonLi.Core;
using DragonLi.Network;
using DragonLi.UI;
using DragonLi.WebGL;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class TaskMainComponent : UIComponent, IMessageReceiver
    {
        #region Property

        [Header("Settings")]
        [SerializeField] private string discord = "https://discord.gg";
        
        [SerializeField] private TaskAccountObjective[] objectives = new TaskAccountObjective[3];
        
        #endregion

        #region UIComponent

        public override void OnShow()
        {
            base.OnShow();
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;

            RefreshCollection();
        }

        public override void OnHide()
        {
            base.OnHide();
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler -= OnReceiveMessage;
        }

        #endregion

        #region Function

        private void ToLink(EFollowSocialPlatform platform)
        {
            var url = platform switch
            {
                EFollowSocialPlatform.Twitter => URLInstance.Instance.URLSettings.twitterURL,
                EFollowSocialPlatform.Discord => discord,
                EFollowSocialPlatform.Telegram => URLInstance.Instance.URLSettings.telegramURL,
                EFollowSocialPlatform.Youtube => URLInstance.Instance.URLSettings.youtubeURL,
            };
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLExternalAPI.OpenExternalLink(url);
#else 
            Application.OpenURL(url);
#endif
        }

        private void RefreshCollection()
        {
            foreach (var objective in objectives)
            {
                objective.RefreshCollection();
            }
        }

        #endregion

        #region API

        public void OnTwitterClick(UIBasicButton sender)
        {
            ToLink(EFollowSocialPlatform.Twitter);
            GameSessionAPI.ObjectiveAPI.CompleteAccount((int)EFollowSocialPlatform.Twitter);
        }

        public void OnDiscordClick(UIBasicButton sender)
        {
            ToLink(EFollowSocialPlatform.Discord);
            GameSessionAPI.ObjectiveAPI.CompleteAccount((int)EFollowSocialPlatform.Discord);
        }

        public void OnTelegramClick(UIBasicButton sender)
        {
            ToLink(EFollowSocialPlatform.Telegram);
            GameSessionAPI.ObjectiveAPI.CompleteAccount((int)EFollowSocialPlatform.Telegram);
        }

        public void OnYoutubeClick(UIBasicButton sender)
        {
            ToLink(EFollowSocialPlatform.Youtube);
            GameSessionAPI.ObjectiveAPI.CompleteAccount((int)EFollowSocialPlatform.Youtube);
        }

        #endregion

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (service != GameSessionAPI.ObjectiveAPI.ServiceName) return;
            if (!response.IsSuccess()) return;
            if (method == GSObjectiveAPI.MethodCompleteAccount)
            {
                var id = response.GetAttachmentAsInt("id");
                PlayerSandbox.Instance.ObjectiveHandler.Account.AddProgressById((EFollowSocialPlatform)id);
                RefreshCollection();
            }

            if (method == GSObjectiveAPI.MethodRewardAccount)
            {
                var id = response.GetAttachmentAsInt("id");
                var progress = response.GetAttachmentAsInt("progress");
                var coin = response.GetAttachmentAsInt("coin");
                var dice = response.GetAttachmentAsInt("dice");
                var token = response.GetAttachmentAsFloat("token");
                
                PlayerSandbox.Instance.ObjectiveHandler.Account.CompletedById((EFollowSocialPlatform)id);
                RefreshCollection();
                var tasks = new List<IQueueableEvent>
                {
                    EffectsAPI.CreateTip(() => coin, () => dice, () => token),
                    EffectsAPI.CreateSoundEffect(() => EffectsAPI.EEffectType.Coin),
                    EffectsAPI.CreateScreenFullEffect(() => coin, () => dice, () => token),
                    new CustomEvent(() =>
                    {
                        PlayerSandbox.Instance.CharacterHandler.Coin += coin;
                        PlayerSandbox.Instance.CharacterHandler.Dice += dice; 
                        PlayerSandbox.Instance.CharacterHandler.Token += Mathf.Approximately(token, -1) ? 0 : token;
                    })
                };
                EventQueue.Instance.Enqueue(tasks);
            }
        }
    }
}