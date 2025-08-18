using DragonLi.Core;
using DragonLi.UI;
using TMPro;
using UnityEngine;

namespace Game
{
    public class UIReconnectingLayer : UILayer
    {
        #region Proeprties

        [Header("References")] 
        [SerializeField] private TextMeshProUGUI tmpContent;

        #endregion

        #region API
        
        public static UIReconnectingLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIReconnectingLayer>("UIReconnectingLayer");
            Debug.Assert(layer);
            return layer;
        }

        public static void SetContent(string content)
        {
            var layer = GetLayer();
            layer.tmpContent.text = content;
        }

        public static void SetContent(int reconnectCount, int maxReconnectCount)
        {
            var layer = GetLayer();
            SetContent(string.Format(layer.GetLocalizedText("reconnect-times-fmt"), reconnectCount, maxReconnectCount));
        }

        public static void  ShowLayer(string content)
        {
            var layer = GetLayer();
            SetContent(content);
            layer.Show();
        }

        public static void HideLayer(float delay = 0f)
        {
            if (delay <= 0)
            {
                GetLayer().Hide();
                return;
            }
            CoroutineTaskManager.Instance.WaitSecondTodo(() =>
            {
                GetLayer().Hide();
            }, delay);
        }

        #endregion

    }
}