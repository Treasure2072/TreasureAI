using UnityEngine;

namespace Game
{
    public static class Settings
    {
        private static GameSettingConfiguration _gameSettingConfiguration;
        public static void LoadSettings()
        {
            _gameSettingConfiguration = Resources.Load<GameSettingConfiguration>("Setting/GameSetting");
        }
        
        public static GameSettingConfiguration GetConfiguration()
        {
            if (_gameSettingConfiguration == null)
            {
                _gameSettingConfiguration = Resources.Load<GameSettingConfiguration>("Setting/GameSetting");
            }
            return _gameSettingConfiguration;
        }
        
        public static GameConnectionConfiguration GetActiveConnectionConfiguration()
        {
            if (_gameSettingConfiguration == null)
            {
                _gameSettingConfiguration = Resources.Load<GameSettingConfiguration>("Setting/GameSetting");
            }
            return _gameSettingConfiguration.GetConnectionConfiguration();
        }
    }
}


