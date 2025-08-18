using System;
using System.IO;
using System.Collections;
using DragonLi.Core;
using HuggingFace.API;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class HuggingFaceTTS : MonoBehaviour
    {
        [Header("Hugging Face")]
        [SerializeField] private string url = "https://api-inference.huggingface.co/models/suno/bark";

        private static IAPIConfig _config;
        private static IAPIConfig config {
            get {
                if (_config == null) {
                    _config = Resources.Load<APIConfig>("HuggingFaceAPIConfig");
                    if (_config == null) {
                        Debug.LogError("HuggingFaceAPIConfig asset not found.");
                    }
                }
                return _config;
            }
        }
        
        private AudioSource Audio { get; set; }
        
        public IEnumerator SendTTSRequest(string text)
        {
            Debug.Log($"SendTTSRequest: {text}");
            // 构造 JSON body
            var json = $"{{\"inputs\": \"{text}\"}}";
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

            var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Authorization", $"Bearer {config.apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("TTS 请求失败: " + request.error);
                yield break;
            }

            var audioBytes = request.downloadHandler.data;
            File.WriteAllBytes(Application.streamingAssetsPath, audioBytes);
            
            LoadClipFromBytes(audioBytes);
        }

        private void LoadClipFromBytes(byte[] audioData)
        {
            var clip = WavUtility.ToAudioClip(audioData);
            if(Audio && Audio.isPlaying) Audio.Stop();
            Audio = SoundAPI.PlaySound(clip);
        }
    }
}