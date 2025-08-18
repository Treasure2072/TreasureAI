using System;
using Data;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class UISettingVolumeComponent : UIComponent
    {
        #region Properties

        [Header("Settings")]
        [SerializeField] private Color colorVolume;
        [SerializeField] private Sprite iconVolume;
        [SerializeField] private Color colorMute;
        [SerializeField] private Sprite iconMute;
        
        [Header("References")]
        [SerializeField] private Button btnVolume;
        [SerializeField] private Image imgVolume;
        [SerializeField] private Slider volumeSlider;
        
        private bool IsMuted { get; set; }

        #endregion

        #region Unity

        private void Awake()
        {
            SystemSandbox.Instance.VolumeHandler.OnVolumeChanged += OnVolumeChanged;
        }

        private void OnDestroy()
        {
            SystemSandbox.Instance.VolumeHandler.OnVolumeChanged -= OnVolumeChanged;
        }

        #endregion

        #region UIComponent

        protected override void OnInit()
        {
            base.OnInit();
            btnVolume.onClick.RemoveAllListeners();
            btnVolume.onClick.AddListener(OnSwitcherVolumeStatusClickCallback);
            
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
            volumeSlider.value = SystemSandbox.Instance.VolumeHandler.Volume;
        }

        #endregion

        #region Function

        /// <summary>
        /// 主动设置系统静音
        /// </summary>
        private void MuteVolume()
        {
            volumeSlider.interactable = false;
            UpdateVolumeIcon(true);
            SetSystemVolume(0f);
        }

        /// <summary>
        /// 解除静音设置
        /// </summary>
        private void UnmuteVolume()
        {
            volumeSlider.interactable = true;
            UpdateVolumeIcon(false);
            SetSystemVolume(volumeSlider.value);
        }

        /// <summary>
        /// 更新音量图标
        /// </summary>
        /// <param name="mute"></param>
        private void UpdateVolumeIcon(bool mute)
        {
            imgVolume.sprite = mute ? iconMute : iconVolume;
            imgVolume.color = mute ? colorMute : colorVolume;
        }

        /// <summary>
        /// 设置系统音量
        /// </summary>
        /// <param name="volume"></param>
        private void SetSystemVolume(float volume)
        {
            // TODO: 设置系统音量
            // ...
            SystemSandbox.Instance.VolumeHandler.Volume = volume;
        }

        #endregion

        #region Callback

        private void OnSwitcherVolumeStatusClickCallback()
        {
            IsMuted = !IsMuted;
            if (IsMuted)
            {
                MuteVolume();
            }
            else
            {
                UnmuteVolume();
            }
        }

        private void OnVolumeSliderValueChanged(float value)
        {
            SystemSandbox.Instance.VolumeHandler.Volume = value;
            UpdateVolumeIcon(value <= 0f);
        }

        private void OnVolumeChanged(float preValue, float newValue)
        {
            foreach (var audioSource in AudioManager.Instance.PlayingAudioSources)
            {
                audioSource.volume = newValue;
            }

            if (newValue - preValue > 0.01f)
            {
                this.SaveVolume(newValue);
            }
        }

        #endregion
    }
}