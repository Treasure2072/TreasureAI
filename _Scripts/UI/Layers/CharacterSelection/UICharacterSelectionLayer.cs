using System.Collections.Generic;
using _Scripts.UI.Common;
using Data;
using DragonLi.UI;
using DragonLi.Core;
using DragonLi.Network;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class UICharacterSelectionLayer : UILayer, IMessageReceiver
    {
        #region Fields

        [Header("References")]
        [SerializeField] private List<GameObject> characters = new();
        [SerializeField] private TextMeshProUGUI tmpNameText;
        [SerializeField] private TextMeshProUGUI tmpDiceText;
        [SerializeField] private TextMeshProUGUI tmpCoinText;
        [SerializeField] private TextMeshProUGUI tmpPriceText;
        [SerializeField] private TextMeshProUGUI tmpDescriptionText;
        
        [Header("References - Arrow")]
        [SerializeField] private Image imgPre;
        [SerializeField] private Image imgNext;
        
        [Header("Settings -Arrow")]
        [SerializeField] [Range(0, 255)] private int validAlpha = 255; 
        [SerializeField] [Range(0, 255)] private int invalidAlpha = 80;
        
        [Space(10)]
        [Header("References")]
        [SerializeField] private Image imgUpgrade;
        [Header("Settings")]
        [SerializeField] private Sprite lockedSprite;
        [SerializeField] private Color lockedColor;
        [SerializeField] private Sprite unlockedSprite;
        [SerializeField] private Color unlockedColor;
        
        #endregion

        #region Properties
        
        private int _selectedCharacter;
        private int SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (_selectedCharacter == value) return;
                _selectedCharacter = value;
                OnSelectedChange(value);
            }
        }
        
        // private int CharacterId => _selectedCharacter;

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
            this["BtnNext"].As<UIBasicButton>().OnClickEvent?.AddListener(OnNextButtonPressed);
            this["BtnPrev"].As<UIBasicButton>().OnClickEvent?.AddListener(OnPrevButtonPressed);
            this["BtnUnlock"].As<UIBasicButton>().OnClickEvent?.AddListener(OnUnlockButtonPressed);
            this["BtnUpgrade"].As<UIBasicButton>().OnClickEvent?.AddListener(OnUpgradeButtonPressed);
            this["BtnGet"].As<UIBasicButton>().OnClickEvent?.AddListener(OnGetButtonPressed);
            this["BtnBack"].As<UIBasicButton>().OnClickEvent?.AddListener(OnBackPressed);
        }

        protected override void OnShow()
        {
            base.OnShow();
            SetUp();
            UIStaticsLayer.ShowUIStaticsLayer();
        }

        protected override void OnHide()
        {
            base.OnHide();
            UIStaticsLayer.HideUIStaticsLayer();
        }

        #endregion

        #region API

        public int GetMaxCharacters()
        {
            return characters?.Count ?? 0;
        }

        #endregion

        #region Functiuons

        private void SetName(string characterName)
        {
            tmpNameText.text = characterName;
        }

        private void SetDice(int dice)
        {
            
            tmpDiceText.text = string.Format(this.GetLocalizedText("more-dice-fmt"), dice);
        }

        private void SetCoin(float coin)
        {
            tmpCoinText.text = string.Format(this.GetLocalizedText("more-coin-fmt"), $"{(int)(coin * 100f)}%");
        }

        private void SetPrice(long price)
        {
            tmpPriceText.text = NumberUtils.GetDisplayNumberString(price);
        }

        private void SetMax()
        {
            tmpPriceText.SetText(this.GetLocalizedText("max-level"));
        }

        private void SetDescription(bool unlocked)
        {
            tmpDescriptionText.text = this.GetLocalizedText(!unlocked ? "unlock-character-des" : "get-character-des");
        }

        private void SetUpgradeButton(bool canUpgrade)
        {
            this["BtnUpgrade"].As<UIBasicButton>().ButtonRef.interactable = canUpgrade;
            if (IsCharacterUnlocked(SelectedCharacter))
            {
                // 当前角色已解锁
                imgUpgrade.sprite = unlockedSprite;
                var col = unlockedColor;
                col.a = canUpgrade ? 1 : 0.5f;
                imgUpgrade.color = col;
            }
            else
            {
                this["BtnUpgrade"].As<UIBasicButton>().ButtonRef.interactable = true;
                //当前角色未解锁
                imgUpgrade.sprite = lockedSprite;
                imgUpgrade.color = lockedColor;
            }
        }

        private void SetUp()
        {
            SwitchCharacter(PlayerSandbox.Instance.CharacterHandler.CharacterId);
            SelectedCharacter = PlayerSandbox.Instance.CharacterHandler.CharacterId;
        }

        private void SwitchCharacter(int character)
        {
            for (var i = 0; i < GetMaxCharacters(); i++)
            {
                characters[i].SetActive(i == character);
            }

            var characterInfo = CharacterSelectionAPI.GetCharacterShopInfoByLevel(PlayerSandbox.Instance.CharacterHandler.GetLevel() + 1, character);
            var characterUnlock = IsCharacterUnlocked(SelectedCharacter);
            SetName(this.GetLocalizedText($"character-{SelectedCharacter}"));
            SetDice(characterInfo.diceAdded);
            SetCoin(characterInfo.coinMultiplier);
            SetDescription(characterUnlock);
            
            SetUpgradeButton(CharacterAPI.CanUpgrade(character));
            SetPrice(characterInfo.coinNeeded);
            
            this["BtnUnlock"].gameObject.SetActive(!characterUnlock);
            this["BtnGet"].gameObject.SetActive(characterUnlock);
            CoroutineTaskManager.Instance.WaitFrameEnd(() =>
            {
                LayoutRebuilder.MarkLayoutForRebuild(this["BtnUpgrade"].transform as RectTransform);
            });
        }

        private void SetImageAlphaValid(Image image, bool valid)
        {
            var preColor = image.color;
            preColor.a = (valid ? validAlpha : invalidAlpha) / 255f;
            image.color = preColor;
        }

        /// <summary>
        /// 当前角色是否解锁
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        private bool IsCharacterUnlocked(int characterId)
        {
            return characterId == 0;
            return PlayerSandbox.Instance.CharacterHandler.Characters.Contains(characterId);
        }
        #endregion

        #region Callbacks

        private void OnSelectedChange(int character)
        {
            SetImageAlphaValid(imgPre, character != 0);
            SetImageAlphaValid(imgNext, character != GetMaxCharacters() - 1);
            SwitchCharacter(character);
        }

        private void OnNextButtonPressed(UIBasicButton sender)
        {
            if (SelectedCharacter >= GetMaxCharacters() - 1) return;
            SelectedCharacter++;
        }

        private void OnPrevButtonPressed(UIBasicButton sender)
        {
            if(SelectedCharacter <= 0) return;
            SelectedCharacter--;
        }

        private void OnUnlockButtonPressed(UIBasicButton sender)
        {
            // TODO: 购买角色相关逻辑处理
            // ...
            
            UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                this.GetLocalizedText("function-not-available"));
        }

        private void OnUpgradeButtonPressed(UIBasicButton sender)
        {
            if (!IsCharacterUnlocked(SelectedCharacter))
            {
                UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                    this.GetLocalizedText("function-not-available"));
                return;
            }
            
            UIConfirmLayer.DisplayConfirmation(this.GetLocalizedText("weather-upgrade-character"),
                res =>
                {
                    if(!res) return;
                    GameSessionAPI.CharacterAPI.UpgradeCharacter(SelectedCharacter);
                });
        }

        private void OnGetButtonPressed(UIBasicButton sender)
        {
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            GameSessionAPI.CharacterAPI.SetCharacter(SelectedCharacter);
            PlayerSandbox.Instance.CharacterHandler.CharacterId = SelectedCharacter;
            SceneManager.Instance.AddSceneToLoadQueueByName(ChessBoardAPI.GetCurrentChessBoard(), 2, true);
            SceneManager.Instance.StartLoad();
            Hide();
        }

        private void OnBackPressed(UIBasicButton sender)
        {
            UIManager.Instance.GetLayer("UIBlackScreen").Show();
            SceneManager.Instance.AddSceneToLoadQueueByName(ChessBoardAPI.GetCurrentChessBoard(), 2, true);
            SceneManager.Instance.StartLoad();
            Hide();
        }
        
        #endregion

        #region Callback - Socket Receiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess())
            {
                UITipLayer.DisplayTip(
                    this.GetLocalizedText("notice"), 
                    this.GetLocalizedText(response.GetError()));
                this.LogErrorEditorOnly(response.error);
                return;
            }
            
            // TODO: 升级角色服务器响应
            // ...
            if (service == GameSessionAPI.CharacterAPI.ServiceName && method == GSCharacterAPI.MethodCharacterUpgrade)
            {
                UITipLayer.DisplayTip(this.GetLocalizedText("notice"),
                    this.GetLocalizedText("upgrade-succeed"));
                this.LogEditorOnly(response.ToJson());
                var level = response.GetAttachmentAsInt("level");
                var coin = response.GetAttachmentAsInt("coin");
                
                PlayerSandbox.Instance.CharacterHandler.UpdateLocalDataOfUpgradeCharacter(SelectedCharacter, level);
                PlayerSandbox.Instance.CharacterHandler.Coin -= UnityEngine.Mathf.Abs(coin);
                
                // 刷新
                SwitchCharacter(SelectedCharacter);
            }
            
            // TODO: 购买角色服务器响应
            // ...
        }

        #endregion
    }

}