using System.Linq;
using Data;
using UnityEngine;

namespace Game
{
    public static class CharacterSelectionAPI
    {
        
        /// <summary>
        /// 获取角色售卖信息
        /// 注意：角色 id 是从 1 开始
        /// </summary>
        /// <param name="level">角色等级</param>
        /// <param name="characterId">角色等级</param>
        /// <returns>角色售卖信息</returns>
        public static FCharacterShopInfo GetCharacterShopInfoByLevel(int level, int characterId)
        {

            foreach (var characterInfo in PlayerSandbox.Instance.ChessBoardHandler.CharactersShopInfos.Where(characterInfo => characterInfo.level == level))
            {
                return characterInfo;
            }
            
            var maxLevel = PlayerSandbox.Instance.ChessBoardHandler.GetMaxLevel(characterId);
            return GetCharacterShopInfoByLevel(maxLevel, characterId);
        }
    }

}