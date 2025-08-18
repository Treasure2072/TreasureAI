using System;
using System.IO;
using DragonLi.Core;
using DragonLi.UI;
using HuggingFace.API;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class RecordButtonHandler : MonoBehaviour
    {
        public static readonly string RecordKey = "Record";
        public static readonly string TextKey = "Text";
        public static readonly string RecordError = "RecordError";
        
        [SerializeField] private int duration = 10;
        
        private AudioClip Clip { get; set; }
        private byte[] Bytes { get; set; }
        
        private ISandbox _Sandbox { get; set; } = new Sandbox();
        
        public ISandbox Sandbox => _Sandbox;

        private bool IsRecording
        {
            get => _Sandbox.GetValue<bool>(RecordKey);
            set => _Sandbox.SetValue(RecordKey, value);
        }

        private string TextContent
        {
            get => _Sandbox.GetValue<string>(TextKey);
            set => _Sandbox.SetValue(TextKey, value);
        }

        public event Action<bool, bool> OnRecodingChanged;
        public event Action<string, string> OnTextChanged; 

        private void Awake()
        {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
            gameObject.SetActive(true);
#else 
            gameObject.SetActive(false);
#endif
            _Sandbox.OnValueChanged += OnSandboxValueChangeCallback;
            IsRecording = false;
        }

        private void OnDestroy()
        {
            _Sandbox.OnValueChanged -= OnSandboxValueChangeCallback;
        }
        
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        private void Update()
        {
            if (IsRecording && Microphone.GetPosition(null) >= Clip.samples)
            {
                StopRecording();
            }
        }
#endif

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        private void StartRecording()
        {
            if (IsRecording) return;
            IsRecording = true;
            this.LogEditorOnly("开始录音。。。");
            Clip = Microphone.Start(null, false, duration, 44100);
        }

        private void StopRecording()
        {
            if(!IsRecording) return;
            IsRecording = false;
            this.LogEditorOnly("结束录音。。。");
            var position = Microphone.GetPosition(null);
            Microphone.End(null);
            var samples = new float[position * Clip.channels];
            Clip.GetData(samples, 0);
            Bytes = EncodeAsWAV(samples, Clip.frequency, Clip.channels);
            SendRecording();
        }
#endif
        
        private void SendRecording() {
            HuggingFaceAPI.AutomaticSpeechRecognition(Bytes, text => {
                TextContent = text;
            }, error => {
                EventDispatcher.TriggerEvent<string>(RecordError, error);
            });
        }
        
        private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
                using (var writer = new BinaryWriter(memoryStream)) {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + samples.Length * 2);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(frequency * channels * 2);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(samples.Length * 2);

                    foreach (var sample in samples) {
                        writer.Write((short)(sample * short.MaxValue));
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private void OnSandboxValueChangeCallback(string key, object preValue, object nowValue)
        {
            // if(key == RecordKey) OnRecodingChanged?.Invoke((bool)preValue, (bool)nowValue);
            if(key == TextKey) OnTextChanged?.Invoke((string)preValue, (string)nowValue);
        }
        
    }
}
