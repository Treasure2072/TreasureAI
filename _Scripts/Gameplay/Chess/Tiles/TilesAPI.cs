using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

public static class TilesAPI
{
    /// <summary>
    /// 曼哈顿距离
    /// </summary>
    /// <param name="from">起点</param>
    /// <param name="to">终点</param>
    /// <returns>曼哈顿距离</returns>
    public static float GetManhattanDistance(Vector3 from, Vector3 to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y) + Mathf.Abs(from.z - to.z);
    }
    
    public static float GetManhattanDistance(Vector2 from, Vector2 to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int[] GetHouseLevels(int level)
    {
        var levelArray = new int[4];
        if (level == 0) return levelArray;
        var curLevel = level;
        while (curLevel > 0)
        {
            levelArray[(curLevel - 1) % 4]++;
            curLevel--;
        }
        return levelArray;
    }


    /// <summary>
    /// 升级 land 费用加成
    /// </summary>
    /// <param name="land"></param>
    /// <returns></returns>
    public static float GetFeeMarkupRate(this ChessTileLand land)
    {
        return 0f;
        return land.GetLandType() switch
        {
            ChessTilesType.EChessTileLandType.House => 0f,
            ChessTilesType.EChessTileLandType.Apartment => 0.25f,
            ChessTilesType.EChessTileLandType.Hotel => 0.5f,
            ChessTilesType.EChessTileLandType.Mall => 1f,
        };
    }

    /// <summary>
    /// land 奖励加成
    /// </summary>
    /// <param name="land"></param>
    /// <returns></returns>
    public static float GetRewardBonusRate(this ChessTileLand land)
    {
        return 0f;
        return land.GetLandType() switch
        {
            ChessTilesType.EChessTileLandType.House => 0f,
            ChessTilesType.EChessTileLandType.Apartment => 0.1f,
            ChessTilesType.EChessTileLandType.Hotel => 0.2f,
            ChessTilesType.EChessTileLandType.Mall => 0.3f,
        };
    }
}
