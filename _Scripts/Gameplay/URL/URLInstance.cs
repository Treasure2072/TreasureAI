using Data;
using DragonLi.Core;

namespace Game
{
    public class URLInstance : Singleton<URLInstance>
    {
        private URLSettings _urlSettings;
        public URLSettings URLSettings
        {
            get
            {
                if (_urlSettings == null)
                {
                    _urlSettings = UnityEngine.Resources.Load<URLSettings>(nameof(Data.URLSettings));
                }
                return _urlSettings;
            }
            set
            {
                if(_urlSettings) return;
                _urlSettings = value;
            }
        } 
    }
}