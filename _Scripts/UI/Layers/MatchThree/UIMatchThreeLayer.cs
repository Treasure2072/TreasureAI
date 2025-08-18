using System.Collections.Generic;
using DragonLi.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    public class UIMatchThreeLayer : UILayer
    {
        #region Fields

        [Header("Settings")] 
        [SerializeField] private TextMeshProUGUI tmpBox01;
        [SerializeField] private TextMeshProUGUI tmpBox02;
        [SerializeField] private TextMeshProUGUI tmpBox03;
        [SerializeField] private TextMeshProUGUI tmpBox04;

        #endregion

        #region Properties

        private Dictionary<string, TextMeshProUGUI> Matches { get; set; }

        #endregion
        
        #region UILayer

        protected override void OnInit()
        {
            base.OnInit();
            
            Matches = new Dictionary<string, TextMeshProUGUI>
            {
                { "box-01", tmpBox01 },
                { "box-02", tmpBox02 },
                { "box-03", tmpBox03 },
                { "box-04", tmpBox04 },
            };
        }

        #endregion

        #region Functions

        private void SetScoreText(string key, int score)
        {
            Matches.GetValueOrDefault(key).text = $"X {score}";
        }

        private void ShowLayer()
        {
            Show();

            foreach (var match in Matches)
            {
                SetScoreText(match.Key, 0);
            }
        }

        #endregion
        
        #region API

        public static void ShowUIMatchThreeLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIMatchThreeLayer>("UIMatchThreeLayer");
            Assert.IsNotNull(layer);
            layer.ShowLayer();
        }

        public static void HideUIMatchThreeLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIMatchThreeLayer>("UIMatchThreeLayer");
            Assert.IsNotNull(layer);
            layer.Hide();
        }
        
        public static void SetScore(string key, int score)
        {
            var layer = UIManager.Instance.GetLayer<UIMatchThreeLayer>("UIMatchThreeLayer");
            Assert.IsNotNull(layer);
            layer.SetScoreText(key, score);
        }

        #endregion
    }

}