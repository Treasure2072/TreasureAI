using System;
using Data;
using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class TaskAccountObjective : UIComponent
    {
        #region Fields

        [Header("Settings")]
        [SerializeField] public EFollowSocialPlatform type;
        
        [Header("References")]
        [SerializeField] private Button btnCollect;
        [SerializeField] private GameObject objCollected;
        [SerializeField] private GameObject objStatus;

        [Header("Reference - Claim")]
        [SerializeField] private GameObject objCoin;
        [SerializeField] private TextMeshProUGUI tmpCoin;
        [SerializeField] private GameObject objDice;
        [SerializeField] private TextMeshProUGUI tmpDice;
        [SerializeField] private GameObject objToken;
        [SerializeField] private TextMeshProUGUI tmpToken;
        
        #endregion

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            btnCollect.onClick.AddListener(() =>
            {
                GameSessionAPI.ObjectiveAPI.RewardAccount((int)type);
            });

            SetClaimNums();
        }
        
        public override void OnShow()
        {
            base.OnShow();

            RefreshCollection();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        #endregion

        #region Function
        
        private void SetClaimNums()
        {
            var dailyRouter = MissionInstance.Instance.Settings.GetTaskMainRouteById(type);
            objCoin.gameObject.SetActive(dailyRouter.coin > 0);
            objDice.gameObject.SetActive(dailyRouter.dice > 0);
            objToken.gameObject.SetActive(dailyRouter.token > 0);
            
            tmpCoin.SetText(NumberUtils.GetDisplayNumberString(dailyRouter.coin));
            tmpDice.SetText(NumberUtils.GetDisplayNumberString(dailyRouter.dice));
            tmpToken.SetText(NumberUtils.GetDisplayNumberStringAsCurrency(dailyRouter.token, 1));
        }

        public void RefreshCollection()
        {
            btnCollect.gameObject.SetActive(PlayerSandbox.Instance.ObjectiveHandler.Account.CanClaim(type));
            objCollected.SetActive(PlayerSandbox.Instance.ObjectiveHandler.Account.IsCollectedById(type));
            objStatus.SetActive(PlayerSandbox.Instance.ObjectiveHandler.Account.IsCompletedById(type));
        }

        #endregion
    }
}

