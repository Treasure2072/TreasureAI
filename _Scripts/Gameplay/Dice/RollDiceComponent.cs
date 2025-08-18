using System;
using System.Collections;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class RollDiceComponent : MonoBehaviour, IMessageReceiver
    {
        #region Define

        private enum ECharacterMoveType
        {
            Move,
            MoveDev,
            Stay
        }

        #endregion
        
        #region Properties
        [Header("Debug")]
        [SerializeField] private ECharacterMoveType moveType = ECharacterMoveType.Move;
        [SerializeField] private int diceA;
        [SerializeField] private int diceB;
        private ChessGameBoard ChessBoardRef { get; set; }
        
        private DiceController DiceControllerRef { get; set; }
        
        private Coroutine AutoRollDiceCoroutine { get; set; }

        private bool CanGo { get; set; } = false;

        #endregion
        
        #region Unity

        private void Awake()
        {
            GameInstance.Instance.HostingHandler.HostingChanged += OnHostingChanged;
        }
        
        private void OnDestroy()
        {
            GameInstance.Instance.HostingHandler.HostingChanged -= OnHostingChanged;
        }

        private IEnumerator Start()
        {
            while (!(ChessBoardRef = World.GetRegisteredObject<ChessGameBoard>(ChessGameBoard.WorldObjectRegisterKey)))
            {
                this.LogEditorOnly("load ChessBoardRef");
                yield return null;
            }
            
            while (!(DiceControllerRef = World.GetRegisteredObject<DiceController>(DiceController.WorldObjectRegisterKey)))
            {
                this.LogEditorOnly("load DiceControllerRef");
                yield return null;
            }
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
            CanGo = true;
            if (GameInstance.Instance.HostingHandler.Hosting)
            {
                StartAutoRoll();
            }
        }

        #endregion

        #region Fucntion

        public bool CanRollDice()
        {
            return CanGo
                   && !ChessBoardRef.IsProcessing()
                   && !DiceControllerRef.IsProcessing()
                   && PlayerSandbox.Instance.CharacterHandler.Dice > 0
                   && GameSessionConnection.Instance.IsConnected()
                   && EventQueue.Instance.GetEventCount() <= 0
                   && GameInstance.Instance.IsChessBoardScene;
        }

        private void ShortDice()
        {
            GameInstance.Instance.HostingHandler.HostingChanged -= OnHostingChanged;
            UITipLayer.DisplayTip(this.GetLocalizedText("notice"), this.GetLocalizedText("dice-shot-des"));
        }

        private void StartAutoRoll()
        {
            if (AutoRollDiceCoroutine != null)
            {
                StopCoroutine(AutoRollDiceCoroutine);
            }
            AutoRollDiceCoroutine = StartCoroutine(AutoRollDiceIEnumerator());
        }

        private void StopAutoRoll()
        {
            if(AutoRollDiceCoroutine == null) return;
            StopCoroutine(AutoRollDiceCoroutine);
            AutoRollDiceCoroutine = null;
        }

        #endregion

        #region API

        public bool Move()
        {
            if (PlayerSandbox.Instance.CharacterHandler.Dice <= 0)
            {
                ShortDice();
                return false;
            }
            
            if(!CanRollDice())
            {
                this.LogEditorOnly($"\n CanGo: {CanGo} " +
                                   $"\n ChessBoardRef.IsProcessing: {ChessBoardRef.IsProcessing()}" +
                                   $"\n DiceControllerRef.IsProcessing: {DiceControllerRef.IsProcessing()}" +
                                   $"\n GameSessionConnection: {GameSessionConnection.Instance.IsConnected()}" +
                                   $"\n Event count = {EventQueue.Instance.GetEventCount()}" +
                                   $"\n IsChessBoardScene = {GameInstance.Instance.IsChessBoardScene}"
                                   );
                return false;
            }
            CanGo = false;
            switch (moveType)
            {
                case ECharacterMoveType.Move:
                    GameSessionAPI.ChessBoardAPI.Move();
                    break;
                case ECharacterMoveType.MoveDev:
                    GameSessionAPI.ChessBoardAPI.MoveDev(diceA, diceB);
                    break;
                case ECharacterMoveType.Stay:
                    GameSessionAPI.ChessBoardAPI.Stay();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        #endregion

        #region Callback - Socket Receiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (service == GameSessionAPI.ChessBoardAPI.ServiceName && method == "arrive")
            {
                CanGo = true;
            }
        }

        #endregion

        #region Calback

        private void OnHostingChanged(bool preVal, bool newVal)
        {
            if (newVal)
            {
                StartAutoRoll();
            }
            else
            {
                StopAutoRoll();
            }
        }

        private IEnumerator AutoRollDiceIEnumerator()
        {
            yield return null;
            while (true)
            {
                if (CanRollDice())
                {
                    yield return CoroutineTaskManager.Waits.OneSecond;
                    Move();
                }
                else
                {
                    // this.LogEditorOnly($"\n CanGo: {CanGo} " +
                    //                    $"\n ChessBoardRef.IsProcessing: {ChessBoardRef.IsProcessing()}" +
                    //                    $"\n DiceControllerRef.IsProcessing: {DiceControllerRef.IsProcessing()}" +
                    //                    $"\n GameSessionConnection: {GameSessionConnection.Instance.IsConnected()}" +
                    //                    $"\n Event count = {EventQueue.Instance.GetEventCount()}");
                }
                
                yield return CoroutineTaskManager.Waits.OneSecond;
            }
        }

        #endregion
    }
}