using System.Collections.Generic;
using System.Linq;

namespace Data.Type
{
    public static class RankHandlerType
    {
        [System.Serializable]
        public struct FUser
        {
            public string name;
            public long data;

            public override bool Equals(object obj)
            {
                if (obj is FUser other)
                {
                    return name == other.name && data == other.data;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return (name?.GetHashCode() ?? 0) ^ data.GetHashCode();
            }
        }
        
        [System.Serializable]
        public struct FRanks
        {
            public List<FUser> coinRanks;
            public List<FUser> tokenRanks;
            public List<FUser> diceRanks;

            public override bool Equals(object obj)
            {
                if (obj is FRanks other)
                {
                    var areCoinRanksEqual = coinRanks != null && other.coinRanks != null && coinRanks.Count == other.coinRanks.Count && coinRanks.SequenceEqual(other.coinRanks);
                    var areTokenRanksEqual = tokenRanks != null && other.tokenRanks != null && tokenRanks.Count == other.tokenRanks.Count && tokenRanks.SequenceEqual(other.tokenRanks);

                    return areCoinRanksEqual && areTokenRanksEqual;
                }

                return false;
            }
            
            public override int GetHashCode()
            {
                var hashCode = 17;
                if (coinRanks != null)
                    hashCode = hashCode * 31 + coinRanks.GetHashCode();
                if (tokenRanks != null)
                    hashCode = hashCode * 31 + tokenRanks.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        /// 获取 coin 排行榜
        /// </summary>
        /// <param name="ranks"></param>
        /// <returns></returns>
        public static List<FUser> GetRanksOfCoin(this FRanks ranks)
        {
            return ranks.coinRanks;
        }

        /// <summary>
        /// 获取 token 排行榜
        /// </summary>
        /// <param name="ranks"></param>
        /// <returns></returns>
        public static List<FUser> GetRanksOfToken(this FRanks ranks)
        {
            return ranks.tokenRanks;
        }

        public static List<FUser> GetRanksOfDice(this FRanks ranks)
        {
            return ranks.diceRanks;
        }
    }
}