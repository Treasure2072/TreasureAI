using System;
using System.Linq;
using _Scripts.UI.Common;
using Data;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UILandUpgradeLayer : UILayer, IMessageReceiver
    {
        #region Fields

        [Header("References")] 
        [SerializeField] private LandUpgradeContainer container;
        [SerializeField] private TextMeshProUGUI tmpNeedCoin;
        [SerializeField] private TextMeshProUGUI tmpNeedTime;
        
        [Header("Settings")]
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color highLightColor = Color.yellow;

        #endregion

        #region Properties

        private Action<long> FinishCallback { get; set; }
        private bool Selected { get; set; }
        
        private UpgradeLand HighLightedLand { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler  += OnReceiveMessage;
        }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["ButtonOK"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnConfirmClick);
            this["ButtonCancel"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnCancelClick);
        }

        protected override void OnShow()
        {
            base.OnShow();
            // UIStaticsLayer.HideUIStaticsLayer();
            UIActivityLayer.HideUIActivityLayer();
            UIChessboardLayer.HideLayer();
            UIStaticsLayer.ShowUIStaticsLayer();
        }

        protected override void OnHide()
        {
            base.OnHide();
            // UIStaticsLayer.ShowUIStaticsLayer();
            UIActivityLayer.ShowUIActivityLayer();
            UIChessboardLayer.ShowLayer();

            if (HighLightedLand)
            {
                HighLightedLand.SetHouseColor(defaultColor);
                HighLightedLand.transform.DOKill();
                HighLightedLand.transform.localScale = Vector3.one;
            }
        }

        #endregion

        #region Function

        private void ShowLayer(long needCoin, Action<long> callback)
        {
            Selected = false;
            SetNeedCoin(needCoin);
            SetNeedTime();
            FinishCallback = callback;
            container.RecycleAllGrids(null, true);
            container.SpawnAllGrids();
            HighLightedLand = HighLight();

            container.transform.localPosition = (int)(GetNeedUpgradeLevel() / 7) * 230f * Vector3.up;
            
            Show();
        }

        private int GetNeedUpgradeLevel()
        {
            var chessBoard = GetChessBord();
            var standTile = chessBoard.GetTileByIndex(PlayerSandbox.Instance.ChessBoardHandler.StandIndex);
            if (standTile is not ChessTileLand land) return -1;
            return land.GetTileData().level + 1;
        }

        private UpgradeLand HighLight()
        {
            var level = GetNeedUpgradeLevel();
            var upgradeLand = container.GetUpgradeLandByLevel(level);
            upgradeLand.SetHouseColor(highLightColor);
            upgradeLand.transform.DOScale(Vector3.one * 1.1f, 1.0f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
            return upgradeLand;
        }

        private void SetNeedCoin(long needCoin)
        {
            tmpNeedCoin.text = NumberUtils.GetDisplayNumberString(needCoin);
        }

        /// <summary>
        /// 设计当前建造时常长
        /// </summary>
        private void SetNeedTime()
        {
            var level = GetNeedUpgradeLevel();
            foreach (var needTime in from land in PlayerSandbox.Instance.ChessBoardHandler.Lands where land.level == level select land.upgradeDuration)
            {
                tmpNeedTime.text = needTime < 60 
                    ? $"{needTime}{this.GetLocalizedText("minute-acronym")}" 
                    : $"{needTime / 60}{this.GetLocalizedText("hour-acronym")}{needTime % 60}{this.GetLocalizedText("minute-acronym")}";
            }
        }

        private ChessGameBoard GetChessBord()
        {
            return World.GetRegisteredObject<ChessGameBoard>(ChessGameBoard.WorldObjectRegisterKey);
        }

        private ChessGameMode GetChessGameMode()
        {
            return World.GetRegisteredObject<ChessGameMode>(ChessGameMode.WorldObjectRegisterKey);
        }

        #endregion

        #region API

        public static UILandUpgradeLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UILandUpgradeLayer>("UILandUpgradeLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowUILandUpgradeLayer(long needCoin, Action<long> callback = null)
        {
            GetLayer()?.ShowLayer(needCoin, callback);
        }

        #endregion

        #region Callback

        public void OnConfirmClick(UIBasicButton sender)
        {
            if (Selected) return;
            Selected = true;
            GameSessionAPI.ChessBoardAPI.Option(null);
            Hide();
        }

        public void OnCancelClick(UIBasicButton sender)
        {
            if (Selected) return;
            Selected = true;
            Hide();
        }

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess()) return;
            if (service != "chess" || method != "option") return;
            if(response.GetAttachmentAsString("tile") != "land") return;
            var coinNeed = response.GetAttachmentAsInt("coin_need");
            var finishTime = response.GetAttachmentAsLong("finishTime");
            
            // chesstileland中已扣款，这里不必再扣款
            // PlayerSandbox.Instance.CharacterHandler.Coin -= coinNeed;
            var gameMode = GetChessGameMode();
            var player = World.GetPlayer();
            gameMode.ModifyCoin(-coinNeed, player.position);

            FinishCallback(finishTime);
        }

        #endregion
    }

}