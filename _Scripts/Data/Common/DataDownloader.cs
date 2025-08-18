using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Data
{
    public static class DataDownloader
    {
        public static async Task<T> DownloadJsonAsync<T>(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
                // request.SetRequestHeader("Accept", "application/json");
#endif
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
#if UNITY_WEBGL
                if(request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
#else
                if (request.result != UnityWebRequest.Result.Success)
#endif
                {
                    Debug.LogError($"[DataDownloader] 请求失败: {request.error}");
                    return default;
                }

                try
                {
                    string json = request.downloadHandler.text;
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[DataDownloader] JSON 解析错误: {e.Message}");
                    return default;
                }
            }
        }
        
        public static async Task<Texture2D> DownloadTexture2DAsync(string url)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, true))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

#if UNITY_WEBGL
                if(request.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
#else
                if (request.result != UnityWebRequest.Result.Success)
#endif
                {
                    Debug.LogError($"[DataDownloader] 下载图片失败: {request.error}");
                    return null;
                }

                return DownloadHandlerTexture.GetContent(request);
            }
        }
        
        public static Sprite TextureToSprite(Texture2D texture)
        {
            return Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }
    }
}