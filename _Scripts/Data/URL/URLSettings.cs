using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "URLSettings", menuName = "Scriptable Objects/URLSettings")]
    public class URLSettings : ScriptableObject
    {
        [Header("Setting - Official")]
        [SerializeField] public string websiteURL = "https://www.enkiai.org/enkigo";
        [SerializeField] public string officialURL = "https://www.enkiai.org";
        [SerializeField] public string dAppUrl = "http://dapp.enkigo.games/ekasset";
        [SerializeField] public string email = "enki.ai.go@gmail.com";
        
        [Header("Setting - Account")]
        [SerializeField] public string youtubeURL = "https://www.youtube.com/channel/UCKlw9jgwP5L1o1ExWBZ0U9A";
        [SerializeField] public string twitterURL = "https://x.com/enki_ai_go";
        [SerializeField] public string telegramURL = "https://t.me/Enkiaicoll";
    }
}