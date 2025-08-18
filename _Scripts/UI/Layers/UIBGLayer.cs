using System;
using System.Collections;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class UIBGLayer : UILayer
    {
        [Header("References")]
        [SerializeField] private RectTransform rect;
        
        [Header("Settings")]
        [SerializeField] private Texture2D textureBg;
        
        private float Width => textureBg.width;
        private float Height => textureBg.height;

        private Vector2 ViewSize { get; set; } = Vector2.zero;

        private void Awake()
        {
            // ViewSize = new Vector2(Screen.width, Screen.height);
            // StartCoroutine(MatchViewport(Screen.width, Screen.height));
            // EventDispatcher.AddEventListener<float, float>(WebGLInputReceiver.WEBGL_VIEWPORT_CHANGED, OnViewportChange);
        }

        private void OnDestroy()
        {
            // EventDispatcher.RemoveEventListener<float, float>(WebGLInputReceiver.WEBGL_VIEWPORT_CHANGED, OnViewportChange);
        }

        private void FixedUpdate()
        {
            if (!Mathf.Approximately(ViewSize.x, Screen.width) || !Mathf.Approximately(ViewSize.y, Screen.height))
            {
                ViewSize = new Vector2(Screen.width, Screen.height);
                StartCoroutine(MatchViewport(Screen.width, Screen.height));
            }
        }

        private IEnumerator MatchViewport(float width, float height)
        {
            if (rect == null || textureBg == null)
            {
                Debug.LogWarning("RectTransform 或 Texture2D 为空，无法匹配视口");
                yield break;
            }
    
            // 计算视口的宽高比
            var viewportRatio = width / height;
            
            // 计算纹理的宽高比
            var textureRatio = Width / Height;

            if (viewportRatio > textureRatio)
            {
                // 视口更宽，宽度两侧对齐
                
                // 1. 先设置左右拉伸锚点，控件宽度拉满父容器宽度（sizeDelta.x=0表示无边距）
                rect.anchorMin = new Vector2(0f, 0.5f);
                rect.anchorMax = new Vector2(1f, 0.5f);
                rect.sizeDelta = new Vector2(0, rect.sizeDelta.y);
                
                yield return null;
                // 2. 获取此时控件的宽度（拉伸后的实际宽度）
                var stretchedWidth = rect.rect.width;
                
                // 3. 改锚点为中心点，设置宽度为刚才获取的宽度，实现固定宽度且居中
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(stretchedWidth, stretchedWidth * Height / Width);
            }
            else
            {
                // 视口更高，按高度撑满，宽度按比例缩放

                // 1. 设置上下拉伸锚点，拉满高度
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);
                
                yield return null;
                
                // 2. 获取拉伸后实际高度
                float stretchedHeight = rect.rect.height;
                
                // 3. 切换锚点为中心，固定高度，宽度按比例缩放
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(stretchedHeight * Width / Height, stretchedHeight);
            }
        }

        private void OnViewportChange(float width, float height)
        {
            StartCoroutine(MatchViewport(width, height));
        }
    }
}