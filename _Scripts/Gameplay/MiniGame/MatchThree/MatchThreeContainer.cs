using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.UI.Common.Grids;
using DG.Tweening;
using DragonLi.Frame;
using Game;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Scripts.Gameplay.MiniGame.MatchThree
{
    public class MatchThreeContainer : GridsContainerBase
    {
        #region Fields
        [Header("Settings")]
        [SerializeField] private float gridSize = 1.4f;
        // [SerializeField] private float gridSpacing = 0.1f;

        [Header("References")]
        [SerializeField] private MatchThreeGameMode gameMode;

        #endregion
        #region Properties
        
        public Action<MatchThreeElement> SelectAction { get; set; }
        
        private WaitForSeconds OneSecond { get; set; }
        private WaitForSeconds HalfSecond { get; set; }
        private WaitForSeconds OnePointSecond { get; set; }

        #endregion
        
        #region GridsContainerBase

        protected override void OnInitialized()
        {
            base.OnInitialized();
            OneSecond = new WaitForSeconds(1f);
            HalfSecond = new WaitForSeconds(0.5f);
            OnePointSecond = new WaitForSeconds(0.1f);
        }

        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);

            if (!gameMode)
            {
                throw new NullReferenceException("GameMode cannot be null!"); 
            }
            
            if (args[0] is not List<List<string>> map)
            {
                throw new NullReferenceException("[MatchThreeContainer] Map cannot be null!");
            }

            for (var i = 0; i < map.Count; i++)
            {
                for (var j = 0; j < map[j].Count; j++)
                {
                    var grid = SpawnGrid<MatchThreeGrid>();
                    SetGridTransform(grid.transform, Vector2Int.right * j + Vector2Int.up * i);
                    grid.SetGrid(map[i][j], SelectAction);
                }
            }
        }

        private void SetGridTransform(Transform grid, Vector2Int cord)
        {
            var gridRoot = GetRoot();
            var xDirection = -gridRoot.transform.right;
            var yDirection = -gridRoot.transform.up;

            var obj = grid.gameObject;
            obj.transform.localPosition = xDirection * (gridSize * cord.x) + yDirection * (gridSize * cord.y);
            obj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj.transform.localScale = Vector3.one;
        }

        #endregion

        #region API

        public GridBase GetGrid(int row, int col, int maxColumn)
        {
            // 检查输入是否合法
            if (row < 0 || col < 0)
            {
                throw new ArgumentException("Row and column indices must be non-negative.");
            }

            int index = row * maxColumn + col;

            // 检查是否越界
            if (index < 0 || index >= Grids.Count)
            {
                throw new IndexOutOfRangeException($"Invalid grid position: ({row}, {col}).");
            }

            return Grids[index];
        }


        public Vector2Int GetGridPosition(GridBase grid, int maxRow, int maxColumn)
        {
            var index = Grids.IndexOf(grid);
            if (index == -1)
            {
                throw new ArgumentException("Grid not found in Grids list.");
            }
            if (maxRow <= 0 || maxColumn <= 0)
            {
                throw new ArgumentException("maxRow and maxColumn must be greater than 0.");
            }
            int row = index / maxColumn;
            int column = index % maxColumn;
            if (row >= maxRow || column >= maxColumn)
            {
                throw new IndexOutOfRangeException("Grid position is out of range.");
            }

            return new Vector2Int(row, column);
        }

        public IEnumerator SwapGrid(GridBase from, GridBase to)
        {
            var fromArgs = from.GetAllValues();
            var toArgs = to.GetAllValues();

            from.GetElement().transform.DOKill();
            to.GetElement().transform.DOKill();
            from.GetElement().transform.DOMove(to.transform.position, 0.25f);
            to.GetElement().transform.DOMove(from.transform.position, 0.25f);

            yield return HalfSecond;
            
            from.RecycleElement();
            from.SetGrid(toArgs);
            to.RecycleElement();
            to.SetGrid(fromArgs);
        }

        public IEnumerator<List<GridBase>> ProcessMatch(GridBase from, GridBase to, int maxRow, int maxColumn)
        {
            // 检查第一个网格的水平和垂直匹配
            var matchesFromHorizontal = CheckHorizontalMatch(from, maxRow, maxColumn);
            var matchesFromVertical = CheckVerticalMatch(from, maxRow, maxColumn);

            // 检查第二个网格的水平和垂直匹配
            var matchesToHorizontal = CheckHorizontalMatch(to, maxRow, maxColumn);
            var matchesToVertical = CheckVerticalMatch(to, maxRow, maxColumn);

            // 合并所有匹配
            var allMatches = matchesFromHorizontal
                .Concat(matchesFromVertical)
                .Concat(matchesToHorizontal)
                .Concat(matchesToVertical)
                .Distinct()
                .ToList();

            // 如果没有匹配，恢复交换
            if (allMatches.Count == 0)
            {
                // SwapGrid(from, to); // 恢复网格状态
                yield return null;
                yield break;
            }

            yield return allMatches;
        }

        // 检查是否有连锁反应
        public IEnumerator<List<GridBase>> CheckForChains(int maxRow, int maxColumn)
        {
            var allMatches = new List<GridBase>();

            // 遍历所有网格，检查新的匹配
            foreach (var grid in Grids)
            {
                if (grid.IsEmpty()) continue;

                var horizontalMatches = CheckHorizontalMatch(grid, maxRow, maxColumn);
                var verticalMatches = CheckVerticalMatch(grid, maxRow, maxColumn);

                allMatches.AddRange(horizontalMatches);
                allMatches.AddRange(verticalMatches);
            }

            // 消除新匹配
            yield return allMatches.Distinct().ToList();
            // if (allMatches.Count > 0)
            // {
            //     foreach (var grid in allMatches)
            //     {
            //         grid.RecycleElement();
            //     }
            //     
            //     yield return FallDown(allMatches, maxRow, maxColumn);
            //
            //     // 递归检查新的连锁反应
            //     yield return CheckForChains(maxRow, maxColumn);
            // }
        }

        /// <summary>
        /// 掉落方块到指定位置
        /// </summary>
        /// <param name="grids">当前消除的方块</param>
        /// <param name="maxRow"></param>
        /// <param name="maxColumn"></param>
        public IEnumerator FallDown(List<GridBase> grids, int maxRow, int maxColumn)
        {
            // 找到所有受影响的列
            var affectedColumns = grids
                .Select(grid => GetGridPosition(grid, maxRow, maxColumn).y) // 获取列索引
                .Distinct()
                .ToList();

            var moves = new List<GridBase>();
            foreach (var column in affectedColumns)
            {
                // 按列处理，从下往上检查
                for (var row = maxRow - 1; row >= 0; row--)
                {
                    var currentGrid = Grids[row * maxColumn + column];

                    if (!currentGrid.IsEmpty()) continue; // 如果当前网格为空
                    // 查找上方最近的非空网格
                    for (var aboveRow = row - 1; aboveRow >= 0; aboveRow--)
                    {
                        var aboveGrid = Grids[aboveRow * maxColumn + column];

                        if (aboveGrid.IsEmpty()) continue; // 找到非空网格
                        // 将上方网格内容移到当前网格
                        currentGrid.SetGrid(aboveGrid.GetAllValues());
                        aboveGrid.RecycleElement(); // 清空上方网格
                                
                        // 位置设置为原来网格
                        currentGrid.GetElement().transform.position = aboveGrid.transform.position;
                        currentGrid.GetElement().transform.DOMove(currentGrid.transform.position, 0.25f).SetEase(Ease.InQuad);
                        break;
                    }
                }

                var offset = 0f;
                // 为该列顶部生成新的方块
                for (var row = maxRow - 1; row >= 0; row--)
                {
                    var grid = Grids[row * maxColumn + column];
                    if (!grid.IsEmpty()) continue;
                    if (offset <= 0f)
                    {
                        offset = 0.215f * (row + 1) + 0.5f;
                    }
                    // 根据逻辑生成新方块（随机或按规则）
                    grid.SetGrid(GenerateRandomElement(), SelectAction);
                    grid.GetElement().transform.position += offset * Vector3.up;
                    grid.GetElement().transform.DOMove(grid.transform.position, 0.25f);
                }
            }

            yield return null;
        }

        private string GenerateRandomElement()
        {
            var designMatches = MatchThreeInstance.Instance.GetMatchesType();
            var router = designMatches.ToList();
            var element = router[Random.Range(0, router.Count)];
            return element;
        }
        
        #endregion

        #region Functions

        private List<GridBase> CheckHorizontalMatch(GridBase target, int maxRow, int maxColumn)
        {
            List<GridBase> matches = new List<GridBase>();

            // 获取目标网格的位置和类型
            var targetPos = GetGridPosition(target, maxRow, maxColumn);
            var targetName = (target as MatchThreeGrid)?.GetName();

            // 向左检查
            for (int col = targetPos.y - 1; col >= 0; col--)
            {
                var grid = Grids[targetPos.x * maxColumn + col];
                if ((grid as MatchThreeGrid)?.GetName() == targetName)
                {
                    matches.Add(grid);
                }
                else
                {
                    break; // 一旦不匹配就停止检查
                }
            }

            // 向右检查
            for (int col = targetPos.y + 1; col < maxColumn; col++)
            {
                var grid = Grids[targetPos.x * maxColumn + col];
                if ((grid as MatchThreeGrid)?.GetName() == targetName)
                {
                    matches.Add(grid);
                }
                else
                {
                    break; // 一旦不匹配就停止检查
                }
            }

            // 添加目标自身
            matches.Add(target);

            // 如果匹配数量小于 3，则不算有效匹配
            if (matches.Count < 3)
            {
                matches.Clear();
            }

            return matches;
        }
        
        public List<GridBase> CheckVerticalMatch(GridBase target, int maxRow, int maxColumn)
        {
            List<GridBase> matches = new List<GridBase>();

            // 获取目标网格的位置和类型
            var targetPos = GetGridPosition(target, maxRow, maxColumn);
            var targetName = (target as MatchThreeGrid)?.GetName();

            // 向上检查
            for (int row = targetPos.x - 1; row >= 0; row--)
            {
                var grid = Grids[row * maxColumn + targetPos.y];
                if ((grid as MatchThreeGrid)?.GetName() == targetName)
                {
                    matches.Add(grid);
                }
                else
                {
                    break; // 一旦不匹配就停止检查
                }
            }

            // 向下检查
            for (int row = targetPos.x + 1; row < maxRow; row++)
            {
                var grid = Grids[row * maxColumn + targetPos.y];
                if ((grid as MatchThreeGrid)?.GetName() == targetName)
                {
                    matches.Add(grid);
                }
                else
                {
                    break; // 一旦不匹配就停止检查
                }
            }

            // 添加目标自身
            matches.Add(target);

            // 如果匹配数量小于 3，则不算有效匹配
            if (matches.Count < 3)
            {
                matches.Clear();
            }

            return matches;
        }

        public List<List<string>> GetMap(int maxRow, int maxColumn)
        {
            if (Grids.Count != maxRow * maxColumn)
            {
                throw new ArgumentException("数组长度与二维尺寸不匹配");
            }
            
            var map = new List<List<string>>(maxRow);
            for (var i = 0; i < maxRow; i++)
            {
                var mapRow = new List<string>(maxColumn);
                for (var j = 0; j < maxColumn; j++)
                {
                    mapRow.Add((Grids[i * maxColumn + j] as MatchThreeGrid)?.GetName());
                }
                
                map.Add(mapRow);
            }
            
            return map;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="map">地图</param>
        /// <param name="minMatchesRowOrCol">在行或列上至少匹配 minMatchesRowOrCol 个相同的元素 (包含自身)</param>
        /// <returns></returns>
        public bool CheckMatch(Vector2Int pos, List<List<string>> map, int minMatchesRowOrCol = 3)
        {
            var target = map[pos.x][pos.y];

            var hor = 1;
            // Horizontal
            for (var col = pos.y - 1; col >= 0; col--)
            {
                if (target == map[pos.x][col])
                {
                    hor++;
                }
                else
                {
                    break;
                }
            }

            for (var col = pos.y + 1; col < map[pos.x].Count; col++)
            {
                if (target == map[pos.x][col])
                {
                    hor++;
                }
                else
                {
                    break;
                }
            }
            
            if(hor >= minMatchesRowOrCol) return true;

            var ver = 1;
            for (var row = pos.x - 1; row >= 0; row--)
            {
                if (target == map[row][pos.y])
                {
                    ver++;
                }
                else
                {
                    break;
                }
            }

            for (var row = pos.x + 1; row < map.Count; row++)
            {
                if (target == map[row][pos.y])
                {
                    ver++;
                }
                else
                {
                    break;
                }
            }
            
            if(ver >= minMatchesRowOrCol) return true;
            
            return false;
        }

        public List<(Vector2Int, Vector2Int)> GetSwaps(List<List<string>> map)
        {
            var res = new List<(Vector2Int, Vector2Int)>();
            for (var i = 0; i < map.Count; i++)
            {
                for (var j = 0; j < map[i].Count; j++)
                {
                    // 向右交换
                    if (j < map[i].Count - 1)
                    {
                        var pos1 = new Vector2Int(i, j);
                        var pos2 = new Vector2Int(i, j + 1);
                        map = Swap(map, pos1, pos2);

                        if (CheckMatch(pos1, map) || CheckMatch(pos2, map))
                        {
                            res.Add((pos1, pos2));
                        }
                        
                        map = Swap(map, pos1, pos2);
                    }

                    // 向下交换
                    if (i < map.Count - 1)
                    {
                        var pos1 = new Vector2Int(i, j);
                        var pos2 = new Vector2Int(i + 1, j);
                        map = Swap(map, pos1, pos2);

                        if (CheckMatch(pos1, map) || CheckMatch(pos2, map))
                        {
                            res.Add((pos1, pos2));
                        }
                        
                        map = Swap(map, pos1, pos2);
                    }
                }
            }
            
            return res;
        }

        private List<List<string>> Swap(List<List<string>> map, Vector2Int pos1, Vector2Int pos2)
        {
            (map[pos1.x][pos1.y], map[pos2.x][pos2.y]) = (map[pos2.x][pos2.y], map[pos1.x][pos1.y]);
            return map;
        }
        
        #endregion
    }

}