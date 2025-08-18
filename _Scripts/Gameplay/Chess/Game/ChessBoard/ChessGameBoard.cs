using System.Collections.Generic;
using System.Linq;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(WorldObjectRegister))]
    public class ChessGameBoard : MonoBehaviour
    {
        public static readonly string WorldObjectRegisterKey = "ChessGameBoard";

        #region Fields

        [Header("Settings")] 
        [SerializeField] private GameObject characterPrefab;

        #endregion

        #region Proeprties

        private List<ChessTile> Tiles { get; set; }
        private ChessGameCharacter CharacterRef { get; set; }
        
        private int CharacterStandIndex { get; set; }

        #endregion

        #region API

        public static ChessGameBoard GetChessGameBoard()
        {
            var chessGameBoard = World.GetRegisteredObject<ChessGameBoard>(ChessGameBoard.WorldObjectRegisterKey);
            Debug.Assert(chessGameBoard);
            return chessGameBoard;
        }

        public int GetTilesCount()
        {
            return Tiles.Count;
        }

        public List<ChessTile> GetTiles()
        {
            return Tiles;
        }

        public ChessTile GetTileByIndex(int index)
        {
            
            return Tiles[index];
        }

        public void InitializeChessBoard(int characterStandIndex)
        {
            // 设置 Tile 序号
            Tiles = GetComponentsInChildren<ChessTile>().ToList();
            for (var i = 0; i < Tiles.Count; i++) Tiles[i].Initialize(i);
            
            // 初始化道具
            // ...
            // foreach (var (index, value) in PlayerSandbox.Instance.ChessBoardHandler.ChessBoardData.items)
            // {
            //     if(!Tiles.IsValidIndex(index)) continue;
            //     (GetTileByIndex(index) as ChessTileLand)?.InitializedData(value);
            // }
            
            // 初始化格子数据
            // ...
            // foreach (var (index, value) in PlayerSandbox.Instance.ChessBoardHandler.ChessBoardData.lands)
            // {
            //     if(!Tiles.IsValidIndex(index)) continue;
            //     (GetTileByIndex(index) as ChessTileLand)?.InitializedData(value);
            // }
            
            
            if (!Tiles.IsValidIndex(characterStandIndex))
            {
                this.LogErrorEditorOnly("Character standing index is not valid.");
                return;
            }

            CharacterStandIndex = characterStandIndex;
            var standingTile = Tiles[CharacterStandIndex];

            // Create character here
            // ...
            var characterObject = Instantiate(characterPrefab,
                standingTile.GetStandPosition(),
                Quaternion.LookRotation(standingTile.GetForwardDirection(), Vector3.up));

            if (characterObject == null)
            {
                this.LogErrorEditorOnly("Character creation failed.");
                return;
            }

            CharacterRef = characterObject.GetComponent<ChessGameCharacter>();
            // World.SetPlayer(characterObject.GetComponent<Transform>());
        }

        public void MoveCharacterForwards(int step, ChessTile.EArriveAnimationType animationType)
        {
            if (CharacterRef.IsMoving())
            {
                return;
            }
            
            var tilesToMove = new List<ChessTile>();
            var seekIndex = CharacterStandIndex;
            for (var i = 0; i < step; i++)
            {
                seekIndex += 1;
                if (seekIndex >= Tiles.Count) seekIndex = 0;

                tilesToMove.Add(Tiles[seekIndex]);
            }

            // Do move forwards
            // ...
            StartCoroutine(CharacterRef.MoveForward(tilesToMove, animationType,
                OnCharacterPassTile, OnCharacterArriveTile, OnCharacterPauseCheck));
        }

        public void TeleportCharacter(int tileIndex, ChessTile.EArriveAnimationType animationType)
        {
            if (CharacterRef.IsMoving())
            {
                return; 
            }
            
            if (!Tiles.IsValidIndex(tileIndex))
            {
                return;
            }
            
            var tile = Tiles[tileIndex];
            StartCoroutine(CharacterRef.Teleport(tile, animationType, OnCharacterArriveTile));
        }

        public void Stay()
        {
            if (!Tiles.IsValidIndex(CharacterStandIndex))
            {
                return;
            }
            
            var tile = Tiles[CharacterStandIndex];
            OnCharacterArriveTile(tile, ChessTile.EArriveAnimationType.None);
        }
        
        public bool IsProcessing()
        {
            if (!CharacterRef) return false;
            return OnCharacterPauseCheck() || CharacterRef.IsMoving();
        }

        public bool IsMoving()
        {
            return CharacterRef.IsMoving();
        }

        #endregion

        #region Functions
        
        private void OnCharacterArriveTile(ChessTile tile, ChessTile.EArriveAnimationType animationType)
        {
            Tiles.ForEach(t => t.PlayArriveAnimation(animationType, tile.TileIndex));
            CharacterStandIndex = tile.TileIndex;
            ProcessEvents(tile.OnArrive());
        }

        private void OnCharacterPassTile(ChessTile tile)
        {
            CharacterStandIndex = tile.TileIndex;
            ProcessEvents(tile.OnPass());
        }
        
        private static bool OnCharacterPauseCheck()
        {
            return EventQueue.Instance.GetEventCount() > 0; 
        }

        private static void ProcessEvents(List<IQueueableEvent> events)
        {
            EventQueue.Instance.Enqueue(events);
        }

        #endregion
    }
}