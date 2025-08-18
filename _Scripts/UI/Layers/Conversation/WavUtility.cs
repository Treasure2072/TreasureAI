using UnityEngine;
using System;
using System.IO;

public static class WavUtility
{
    public static AudioClip ToAudioClip(byte[] flacFile, string clipName = "TTS_Audio")
    {
        try
        {
            // 临时保存 FLAC 数据到文件
            string tempFlacFilePath = Path.Combine(Application.persistentDataPath, "temp.flac");

            // 将 FLAC 数据保存为文件
            File.WriteAllBytes(tempFlacFilePath, flacFile);

            // 确保文件已保存
            if (!File.Exists(tempFlacFilePath))
            {
                Debug.LogError("FLAC 文件保存失败！");
                return null;
            }

            // 输出调试信息
            Debug.Log("FLAC 文件保存成功，开始解码...");

            // 使用 NAudio.Flac 解码 FLAC 文件为 PCM 数据
            float[] pcmData = DecodeFlacToPcm(tempFlacFilePath);

            // 如果 PCM 数据为空或长度为零，输出调试信息
            if (pcmData == null || pcmData.Length == 0)
            {
                Debug.LogError("解码 FLAC 文件时，PCM 数据为空！");
                return null;
            }

            // 输出调试信息
            Debug.Log($"解码完成，PCM 数据长度：{pcmData.Length}");

            // 将 PCM 数据加载为 AudioClip
            return CreateAudioClipFromPcmData(pcmData, clipName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"发生异常: {ex.Message}");
            return null;
        }
    }

    private static float[] DecodeFlacToPcm(string flacFilePath)
    {
        return null;
        // 读取 FLAC 文件并解码为 PCM 数据
        // try
        // {
        //     using (var reader = new FlacReader(flacFilePath))
        //     {
        //         int sampleCount = (int)reader.Length;  // 获取样本数量
        //         float[] pcmData = new float[sampleCount];
        //         reader.Read(pcmData, 0, sampleCount);  // 解码 FLAC 文件为 PCM 数据
        //         return pcmData;
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogError($"解码 FLAC 文件时出错: {ex.Message}");
        //     return null;
        // }
    }

    private static AudioClip CreateAudioClipFromPcmData(float[] pcmData, string clipName)
    {
        // 创建 AudioClip 并加载 PCM 数据
        try
        {
            int sampleCount = pcmData.Length;
            AudioClip audioClip = AudioClip.Create(clipName, sampleCount, 1, 44100, false);
            audioClip.SetData(pcmData, 0);  // 将 PCM 数据加载到 AudioClip
            return audioClip;
        }
        catch (Exception ex)
        {
            Debug.LogError($"创建 AudioClip 时出错: {ex.Message}");
            return null;
        }
    }
}
