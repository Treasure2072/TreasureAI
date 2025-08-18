using System;
using System.Collections.Generic;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public static class EffectsAPI
    {
        public enum EEffectType
        {
            None = 0,
            Coin,
            Dice,
            Token,
            CoinAndToken
        }
        
        public enum EEffectSizeType
        {
            None = 0,
            Small,
            Medium,
            Big,
        }
        
        public static Vector3 GetScreenCenterWorldPosition()
        {
            var screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 10);
            return Camera.main.ScreenToWorldPoint(screenCenter);
        }

        public static IQueueableEvent CreateTip(Func<int> getCoin, Func<int> getDice, Func<float> getToken)
        {
            return new CustomEvent(() =>
            {
                if(getCoin() <= 0 && getDice() <= 0 && getToken() <= 0) return;
                
                var layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
                if (layer == null)
                {
                    Debug.LogError($"The world element layer is null, please add UIWorldElementLayer!");
                    return;
                }
                
                var tip = layer.SpawnWorldElement<UIWSScreenEffectTip>(EffectInstance.Instance.Settings.tip, EffectsAPI.GetScreenCenterWorldPosition());
                tip.Play(getCoin(), getDice(), getToken());
            });
        }
        
        public static IQueueableEvent CreateScreenFullEffect(Func<int> getCoin, Func<int> getDice, Func<float> getToken)
        {
            return new PlayVfxCoinDiceTokenEvent(() => EffectInstance.Instance.Settings.vfxCoinDiceToken.GetGameObject(), getCoin, getDice, getToken);
        }

        public static IQueueableEvent CreateSoundEffect(Func<EEffectType> getType)
        {
            return new CustomEvent(() =>
            {
                if(getType() == EEffectType.None) return;
                
                SoundAPI.PlaySound(getType() switch
                {
                    EEffectType.None => null,
                    EEffectType.Coin => AudioInstance.Instance.Settings.moneyGain,
                    EEffectType.Dice => AudioInstance.Instance.Settings.diceGain,
                    EEffectType.Token => AudioInstance.Instance.Settings.moneyGain,
                    EEffectType.CoinAndToken => AudioInstance.Instance.Settings.moneyGain,
                    _ => throw new ArgumentOutOfRangeException()
                });
            });
        }
    }
}
