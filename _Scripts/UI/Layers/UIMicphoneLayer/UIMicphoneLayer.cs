using DragonLi.UI;
using TMPro;
using UnityEngine;
using System.IO;
using HuggingFace.API;
using DragonLi.Core;
using UnityEngine.Events;

namespace Game
{
    public class UIMicphoneLayer : UILayer
    {
        [SerializeField] private int duration = 10;
        [SerializeField] private TextMeshProUGUI content;
        
        
        private AudioClip Clip { get; set; }
        private byte[] Bytes { get; set; }
        private bool IsRecording { get; set; }
        
        public UnityEvent<bool, string> RecordResultEvent { get; } = new();
        
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        private void Update()
        {
            if (IsRecording && Microphone.GetPosition(null) >= Clip.samples)
            {
                StopRecording();
            }
        }
#endif
        
        protected override void OnInit()
        {
            base.OnInit();
            this["HoldProgressButton"].As<HoldProgressButton>().Pressed += OnPress;
            this["HoldProgressButton"].As<HoldProgressButton>().Released += OnRelease;
        }

        protected override void OnShow()
        {
            base.OnShow();
            SetContent("");
        }

        protected override void OnHide()
        {
            base.OnHide();
            
        }

        public static UIMicphoneLayer GetLayer()
        {
            var layer = UIManager.Instance.GetLayer<UIMicphoneLayer>("UIMicphoneLayer");
            Debug.Assert(layer != null);
            return layer;
        }

        public static void ShowLayer()
        {
            GetLayer()?.Show();
        }

        public static void HideLayer()
        {
            GetLayer()?.Hide();
        }


        public void SetContent(string text)
        {
            this.content.text = text;
        }
        

        private void StartRecording()
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            if (IsRecording) return;
            IsRecording = true;
            this.LogEditorOnly("开始录音。。。");
            Clip = Microphone.Start(null, false, duration, 44100);
#endif
        }
        
        private void StopRecording()
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            if(!IsRecording) return;
            IsRecording = false;
            this.LogEditorOnly("结束录音。。。");
            var position = Microphone.GetPosition(null);
            Microphone.End(null);
            var samples = new float[position * Clip.channels];
            Clip.GetData(samples, 0);
            Bytes = EncodeAsWAV(samples, Clip.frequency, Clip.channels);
            SendRecording();
#endif
        }

        
        private void SendRecording() {
            HuggingFaceAPI.AutomaticSpeechRecognition(Bytes, text => {
                RecordResultEvent.Invoke(true, text);
                RecordResultEvent?.RemoveAllListeners();
                SetContent(text);
            }, error => {
                RecordResultEvent.Invoke(false, error);
                RecordResultEvent?.RemoveAllListeners();
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

        private void OnPress()
        {
            StartRecording();
        }

        private void OnRelease()
        {
            StopRecording();
        }
    }
}