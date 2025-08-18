using System.Collections;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class ChessSelectionGameMode : GameMode
    {
        private IEnumerator Start()
        {
            yield return null;
            
            UIManager.Instance.GetLayer("UIBlackScreen").Hide();

            yield return CoroutineTaskManager.Waits.OneSecond;
            
            UIManager.Instance.GetLayer("UIChessSelectionLayer").Show();
        }
    }
}
