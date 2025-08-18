using System.Collections.Generic;
using System.Linq;
using Game;

namespace Data
{
    public static class PlayerSandboxAPI
    {
        public static FLand GetLandInfoByLevel(this List<FLand> lands, int level)
        {
            foreach (var land in lands.Where(land => land.level == level))
            {
                return land;
            }

            return default;
        }
    }
}