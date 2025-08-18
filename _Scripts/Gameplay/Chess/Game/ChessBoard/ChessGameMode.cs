using System;
using System.Collections;
using Data;
using DG.Tweening;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(WorldObjectRegister))]
    [RequireComponent(typeof(ReceiveMessageHandler))]
    [RequireComponent(typeof(DiceRecoverComponent))]
    [RequireComponent(typeof(RollDiceComponent))]
    [RequireComponent(typeof(PlayerCameraController))]
    public class ChessGameMode : GameMode, IMessageReceiver
    {
        public new static readonly string WorldObjectRegisterKey = "ChessGameMode";

        #region Debug

        [Space]
        
        [Header("Debug")]
        [SerializeField] public bool debugMode;
        
        [Header("Debug - ChanceEvent")]
        [SerializeField][Range(1f, 13f)] public int chanceEventId = 1;

        #endregion
        
        #region Properties

        private ChessGameBoard ChessGameBoard { get; set; }
        private DiceController DiceControllerRef { get; set; }
        
        public DragonLiCamera CameraRef { get; private set; }
        
        public RollDiceComponent RollDiceRef { get; private set; }
        
        public PlayerCameraController PlayerCameraControllerRef { get; private set; }
        
        public ChessGameCharacter CharacterRef { get; private set; }

        #endregion

        #region Unity
        
        private void Awake()
        {
            AudioManager.Instance.StopSound(1, 2f);
            AudioManager.Instance.PlaySound(0, AudioInstance.Instance.Settings.chessboard, SystemSandbox.Instance.VolumeHandler.Volume, 2.0f);
            RollDiceRef = GetComponent<RollDiceComponent>();
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
        }

        private void OnDestroy()
        {
            DisableCamera(null);
            if (!CharacterRef) return;
            CharacterRef.OnCharacterMoveStart -= DisableCamera;
            CharacterRef.OnCharacterMoveEnd -= EnableCamera;
            CharacterRef.OnCharacterJumpStart -= DisableCamera;
            CharacterRef.OnCharacterJumpEnd -= EnableCamera;
        }

        private IEnumerator Start()
        {
            while (!World.GetRegisteredObject<ChessGameMode>(WorldObjectRegisterKey))
            {
                yield return null;
            }
            
            while (!(ChessGameBoard = World.GetRegisteredObject<ChessGameBoard>(ChessGameBoard.WorldObjectRegisterKey)))
            {
                yield return null;
            }
            
            while (!(DiceControllerRef = World.GetRegisteredObject<DiceController>(DiceController.WorldObjectRegisterKey)))
            {
                yield return null;
            }

            while (!(CameraRef = World.GetMainCamera()?.GetComponent<DragonLiCamera>()))
            {
                yield return null;
            }
            
            PlayerCameraControllerRef = GetComponent<PlayerCameraController>();

            this.LogEditorOnly("检测与服务端的连接是否完成...");
            while (!GameSessionConnection.Instance.IsConnected())
            {
                yield return null;
            }

            yield return CoroutineTaskManager.Waits.QuarterSecond;
            
            this.LogEditorOnly("检测与服务端的连接完成");
            ChessGameBoard.InitializeChessBoard(PlayerSandbox.Instance.ChessBoardHandler.StandIndex);

            while (!(CharacterRef = World.GetPlayer<ChessGameCharacter>()))
            {
                yield return null;
            }
            
            if (PlayerSandbox.Instance.ChessBoardHandler.StandIndex == 0)
            {
                UIManager.Instance.GetLayer("UIBlackScreen").Hide();
                CameraRef.enabled = false;
                var cameraStart = World.GetRegisteredObject("Camera-Start");
                Debug.Assert(cameraStart != null, "cameraStart != null");
                CameraRef.transform.DOMove(cameraStart.transform.position, 5.0f).SetEase(Ease.InOutQuad);
                yield return new WaitForSeconds(5.0f);
                CameraRef.enabled = true;
            }
            else
            {
                yield return CoroutineTaskManager.Waits.QuarterSecond;
                UIManager.Instance.GetLayer("UIBlackScreen").Hide();
            }
            
            PlayerCameraControllerRef.SetupController();

            CharacterRef.OnCharacterMoveStart += DisableCamera;
            CharacterRef.OnCharacterMoveEnd += EnableCamera;
            CharacterRef.OnCharacterJumpStart += DisableCamera;
            CharacterRef.OnCharacterJumpEnd += EnableCamera;

            GameInstance.Instance.IsChessBoardScene = true;
            
            yield return CoroutineTaskManager.Waits.OneSecond;

            if (MathAPI.IsValidEmail(PlayerSandbox.Instance.RegisterAndLoginHandler.Email))
            {
                GameObject bank;
                UIWorldElementLayer layer;
                while (!(bank = World.GetRegisteredObject("Bank")))
                {
                    yield return null;
                }
                while (!(layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer")))
                {
                    yield return null;
                }
                layer.SpawnWorldElement<UIWorldElement>(EffectInstance.Instance.Settings.uiBankOpenButton, bank.transform.position + Vector3.down * 3.5f);
            }
            
            if (TutorialHandler.IsTriggerable(TutorialHandler.FirstEnterKey))
            {
                GameInstance.Instance.EnteredMainScene = true;
                UIStartLayer.GetLayer().OnHideOperated.AddListener(() =>
                {
                    var ann = UIManager.Instance.GetLayer("UIAnnLayer");
                    ann.OnHideEventHandler = () =>
                    {
                        if (TutorialHandler.IsTriggerable(TutorialHandler.FirstEnterKey))
                        {
                            var layer = UITutorialLayer.GetLayer("UITutorialLayer-Welcome");
                            layer.OnHidEvent.AddListener(() =>
                            {
                                PopMainUI();
                                UIChessboardLayer.GetLayer().ActiveFingerRoll(true);
                            });
                            layer.Show();
                        }

                        ann.OnHideEventHandler = null;
                    };
                    ann.Show();
                });
                UIStartLayer.GetLayer().Show();
            }
            else if(!GameInstance.Instance.EnteredMainScene)
            {
                GameInstance.Instance.EnteredMainScene = true;
                UIManager.Instance.GetLayer("UIAnnLayer").Show();
                PopMainUI();
            }
            else
            {
                // UIManager.Instance.GetLayer("UIAnnLayer").Show();
                PopMainUI();
            }
        }
        
        #endregion

        #region Functions

        private void PopMainUI()
        {
            UIStaticsLayer.ShowUIStaticsLayer();
            UIActivityLayer.ShowUIActivityLayer();
            UIChessboardLayer.ShowLayer();
        }

        private void StartRollingDice(int diceA, int diceB)
        {
            DiceControllerRef.SpawnDices(diceA, diceB);
            var step = diceA + diceB;
            Go(step);
        }

        private void EnableCamera(GameCharacter sender)
        {
            PlayerCameraControllerRef.SetControllerEnable(true);
        }

        private void DisableCamera(GameCharacter sender)
        {
            PlayerCameraControllerRef.SetControllerEnable(false);
            PlayerCameraControllerRef.SetOverrideTarget(null);
            PlayerCameraControllerRef.SetupCamera();
        }

        #endregion

        #region API
        

        private void Go(int step)
        {
            if (ChessGameBoard.IsProcessing())
            {
                return;
            }

            var tile = ChessGameBoard.GetTileByIndex(PlayerSandbox.Instance.ChessBoardHandler.StandIndex);
            ChessGameBoard.MoveCharacterForwards(step, tile.GetArriveAnimationType());
        }

        public void Jump(int toIndex)
        {
            if (ChessGameBoard.IsMoving())
            {
                return;
            }

            PlayerSandbox.Instance.ChessBoardHandler.StandIndex = toIndex;
            ChessGameBoard.TeleportCharacter(toIndex, ChessTile.EArriveAnimationType.Teleport);
        }

        private void Stay()
        {
            ChessGameBoard.Stay();
        }

        public void ModifyCoin(int coin, Vector3 snapLocation, Transform snapTo = null)
        {
            var layer = UIManager.Instance.GetLayer<UIWorldElementLayer>("UIWorldElementLayer");
            var wsCoin = layer.SpawnWorldElement<UIWSNumber>(EffectInstance.Instance.Settings.uiEffectCoinNumber, snapLocation + Vector3.up * 3.5f, snapTo);
            wsCoin.SetNumber(coin);
        }

        [ContextMenu("QueryCurrency")]
        private void QueryCurrency()
        {
            GameSessionAPI.CharacterAPI.QueryCurrency();
        }

        #endregion

        #region Callbacks

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            this.LogEditorOnly(JsonConvert.SerializeObject(response.body));
            if(service == GameSessionAPI.CharacterAPI.ServiceName && method == GSCharacterAPI.MethodQueryCurrency)
            {
                var coin = response.GetAttachmentAsInt("coin");
                var dice = response.GetAttachmentAsInt("dice");
                var token = response.GetAttachmentAsFloat("token");

                if (PlayerSandbox.Instance.CharacterHandler.Coin != coin)
                {
                    this.LogErrorEditorOnly($"[{PlayerSandbox.Instance.CharacterHandler.Coin}]  [{coin}] Coin Data ERROR!");
                }

                if (PlayerSandbox.Instance.CharacterHandler.Dice != dice)
                {
                    this.LogErrorEditorOnly("Dice Data ERROR!");
                }

                if (!Mathf.Approximately(PlayerSandbox.Instance.CharacterHandler.Token, token))
                {
                    this.LogErrorEditorOnly("Token Data ERROR!");
                }
            }
            
            if (service == GameSessionAPI.ChessBoardAPI.ServiceName && method == GSChessBoardAPI.MethodMove)
            {
                var diceA = response.GetAttachmentAsInt("a");
                var diceB = response.GetAttachmentAsInt("b");
                var finalTileIndex = response.GetAttachmentAsInt("stand");
                var coin = response.GetAttachmentAsInt("coin");
                var rewardType = response.GetAttachmentAsInt("reward");
                if (finalTileIndex == PlayerSandbox.Instance.ChessBoardHandler.StandIndex)
                {
                    Stay();
                }
                else
                {
                    StartRollingDice(diceA, diceB);
                    PlayerSandbox.Instance.ChessBoardHandler.StandIndex = finalTileIndex;
                }
                PlayerSandbox.Instance.CharacterHandler.Coin += coin;
                if ((diceA == 1 && diceB == 1) || (diceA == 6 && diceB == 6))
                {
                    
                }
                else
                {
                    PlayerSandbox.Instance.CharacterHandler.Dice--;
                }
                PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("dice-01", 1);
                PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("dice-02", 1);
                PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("dice-03", 1);
                PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("dice-04", 1);
                PlayerSandbox.Instance.ObjectiveHandler.Daily.AddProgressDailyById("dice-05", 1);
            }
        }


        #endregion
    }
}


