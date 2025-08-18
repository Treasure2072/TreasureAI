using System;
using UnityEngine;

namespace Data
{
    public static class SystemSandboxAPI
    {
        public static void SaveLanguage(this object call, string language)
        {
            PlayerPrefs.SetString(LanguageHandler.kLanguageKey, language);
        }

        public static void SaveVolume(this object call, float volume)
        {
            PlayerPrefs.SetFloat(VolumeHandler.kVolumeKey, volume);
        }
    }
}