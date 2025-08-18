using Game;

namespace Data
{
    public static class CharacterAPI
    {
        public static bool CanUpgrade(int characterId)
        {
            var maxLevel = PlayerSandbox.Instance.ChessBoardHandler.GetMaxLevel(characterId);
            var level = PlayerSandbox.Instance.CharacterHandler.GetLevelById(characterId);

            if (level >= maxLevel) return false;
            
            // coin不足
            var characterInfo = CharacterSelectionAPI.GetCharacterShopInfoByLevel(level + 1, characterId);
            return PlayerSandbox.Instance.CharacterHandler.Coin >= characterInfo.coinNeeded;
        }
    }
}