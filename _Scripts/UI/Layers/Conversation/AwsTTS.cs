using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class AwsTTS : MonoBehaviour
{
    [SerializeField] private string access_key = "";
    [SerializeField] private string secret_key = "";
    [SerializeField] private AudioSource audioSource;
    
    public async Task<AudioClip> TTS(string text, long timeStamp)
    {
        var credentials = new BasicAWSCredentials(access_key, secret_key);
        var client = new AmazonPollyClient(credentials, RegionEndpoint.EUCentral1);

        var request = new SynthesizeSpeechRequest
        {
            Text = text,
            Engine = Engine.Neural,
            VoiceId = VoiceId.Zhiyu,
            OutputFormat = OutputFormat.Mp3,
            LanguageCode = LanguageCode.CmnCN
        };
        
        var response = await client.SynthesizeSpeechAsync(request);
        WriteIntoFile(response.AudioStream, timeStamp);

        using (var www = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.persistentDataPath}/{timeStamp}.mp3",
                   AudioType.MPEG))
        {
            var op = www.SendWebRequest();
            while (!op.isDone) await Task.Yield();
            
            var clip = DownloadHandlerAudioClip.GetContent(www);
            return clip;
            // if(audioSource && !audioSource.isPlaying) audioSource.Stop();
            // audioSource.clip = clip;
            // audioSource.Play();
        }
    }

    private void WriteIntoFile(Stream stream, long timeStamp)
    {
        using (var fileStream = new FileStream($"{Application.persistentDataPath}/{timeStamp}.mp3", FileMode.Create))
        {
            byte[] buffer = new byte[8 * 1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
        }
    }
}
