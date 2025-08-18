using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.UI.Common;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UIPrisonLayer : UILayer
    {
        #region Fields

        [Header("References")]
        [SerializeField] private TextMeshProUGUI RemainingText;
        
        #endregion

        #region Properties
        
        private DiceController DiceControllerRef { get; set; }
        
        private bool bLoaded { get; set; }
        private bool Finish { get; set; }
        
        private int RollTimes { get; set; }
        
        private int RollPoints { get; set; }

        private int Coin { get; set; }
        private int Dice { get; set; }
        private List<int> Dices { get; set; }

        #endregion

        #region Unity

        private IEnumerator Start()
        {
            bLoaded = false;
            while (!(DiceControllerRef = World.GetRegisteredObject<DiceController>(DiceController.WorldObjectRegisterKey)))
            {
                yield return null;
            }

            bLoaded = true;
        }

        #endregion

        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            this["BtnDice"].As<UIBasicButton>()?.OnClickEvent.AddListener(OnClickDiceCallback);
        }

        protected override void OnShow()
        {
            base.OnShow();
            World.GetRegisteredObject<DiceController>(DiceController.WorldObjectRegisterKey)?.SetDiceType(DiceComponent.EDiceType.Red);
            UIPrisonNormalLayer.GetLayer().Show();
            UIActivityLayer.HideUIActivityLayer();
            UIChessboardLayer.HideLayer();
            UIStaticsLayer.HideUIStaticsLayer();
            Finish = false;
        }

        protected override void OnHide()
        {
            base.OnHide();
            World.GetRegisteredObject<DiceController>(DiceController.WorldObjectRegisterKey)?.SetDiceType(DiceComponent.EDiceType.White);
            
            UIPrisonNormalLayer.GetLayer().Hide();
            UIPrisonSucceedLayer.GetLayer().Hide();
            UIPrisonFailedLayer.GetLayer().Hide();
            
            UIActivityLayer.ShowUIActivityLayer();
            UIChessboardLayer.ShowLayer();
            UIStaticsLayer.ShowUIStaticsLayer();
        }

        #endregion

        #region Function

        private void ShowLayer(List<int> dices, int coin)
        {
            
            Dices = dices;
            Coin = coin;
            RollTimes = 0;
            RollPoints = 0;
            SetRemainingText(Dices.Count / 2 - RollTimes);
            Show();
        }

        private bool IsSucceed()
        {
            return RollPoints >= 24;
        }

        private bool IsFailed()
        {
            return RollTimes * 2 >= Dices.Count && RollPoints < 24;
        }

        private void SetRemainingText(int remain)
        {
            RemainingText.text = string.Format(this.GetLocalizedText("prison-remain-times-fmt"), remain);
        }
        
        #endregion

        #region API

        public static UIPrisonLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIPrisonLayer>("UIPrisonLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void ShowUIPrisonLayer(List<int> dices, int coin)
        {
            GetLayer()?.ShowLayer(dices, coin);
        }

        #endregion

        #region Callback

        private void OnClickDiceCallback(UIBasicButton sender)
        {
            if (!bLoaded) return;
            if(Finish) return;
            if(DiceControllerRef.IsProcessing()) return;
            if (RollTimes * 2 >= Dices.Count) return;
            
            RollPoints += Dices[RollTimes * 2] + Dices[RollTimes * 2 + 1];
            DiceControllerRef.SpawnDices(Dices[RollTimes * 2], Dices[RollTimes * 2 + 1]);
            RollTimes++;
            SetRemainingText(Dices.Count / 2 - RollTimes);

            CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                UIPrisonNormalLayer.GetLayer().SetTMPRollPoints(RollPoints);
            }, 1.3f);
            
            if (IsSucceed())
            {
                Finish = true;
                
                CoroutineTaskManager.Instance.WaitSecondTodo(() =>
                {
                    UIPrisonNormalLayer.GetLayer().Hide();
                    UIPrisonSucceedLayer.GetLayer().Show();
                }, 3f);
                CoroutineTaskManager.Instance.WaitSecondTodo(Hide, 5f);
            }else if (IsFailed())
            {
                Finish = true;
                CoroutineTaskManager.Instance.WaitSecondTodo(() =>
                {
                    UIPrisonNormalLayer.GetLayer().Hide();
                    UIPrisonFailedLayer.GetLayer().SetPunishCoin(Coin);
                    UIPrisonFailedLayer.GetLayer().Show();
                }, 3f);
                CoroutineTaskManager.Instance.WaitSecondTodo(Hide, 5f);
            }
        }

        public IEnumerator AutoRollDice()
        {
            yield return CoroutineTaskManager.Waits.OneSecond;
            
            while (IsShowing && GameInstance.Instance.HostingHandler.Hosting && !IsSucceed() && !IsFailed())
            {
                OnClickDiceCallback(this["BtnDice"].As<UIBasicButton>());
                yield return CoroutineTaskManager.Waits.TwoSeconds;
            }
        }

        #endregion
    }
}