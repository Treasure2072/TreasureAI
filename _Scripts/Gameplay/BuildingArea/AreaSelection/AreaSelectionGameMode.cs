using System;
using System.Collections;
using Data;
using DragonLi.Core;
using DragonLi.UI;
using Game;
using UnityEngine;

namespace Game
{
    public class AreaSelectionGameMode : GameMode
    {

        private IEnumerator Start()
        {
            yield return null;
            AudioManager.Instance.StopSound(0, 2f);
            AudioManager.Instance.PlaySound(1, AudioInstance.Instance.Settings.building, SystemSandbox.Instance.VolumeHandler.Volume, 2.0f);
            
            UIManager.Instance.GetLayer("UIBlackScreen").Hide();

            yield return CoroutineTaskManager.Waits.OneSecond;
            
            UIManager.Instance.GetLayer("UIBlockSelectionLayer").Show();
        }
    }
}


