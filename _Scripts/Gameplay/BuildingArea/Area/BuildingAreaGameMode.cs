using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Utils;
using Data;
using DragonLi.Core;
using DragonLi.Frame;
using DragonLi.Network;
using DragonLi.UI;
using Newtonsoft.Json;
using UnityEngine;
using IQueueableEvent = DragonLi.Core.IQueueableEvent;

namespace Game
{
    [RequireComponent(typeof(ReceiveMessageHandler))]
    public class BuildingAreaGameMode : GameMode, IMessageReceiver
    {
        #region Fields

        [Header("References")]
        [SerializeField] private BuildingArea[] buildingAreas;

        [Header("Settings")]
        [SerializeField] private int areaIndex;

        #endregion
        
        #region Unity

        private void Awake()
        {
            GetComponent<ReceiveMessageHandler>().OnReceiveMessageHandler += OnReceiveMessage;
            AudioManager.Instance.StopSound(0, 2f, 1f);
            AudioManager.Instance.PlaySound(1, AudioInstance.Instance.Settings.building, SystemSandbox.Instance.VolumeHandler.Volume, 2.0f);
        }

        private IEnumerator Start()
        {
            while (!World.GetRegisteredObject<BuildingAreaGameMode>(WorldObjectRegisterKey))
            {
                yield return null;
            }

            Initialize();

            UILayer screenLayer;
            while (!(screenLayer = UIManager.Instance.GetLayer("UIBlackScreen")))
            {
                yield return null;
            }
            screenLayer.Hide();

            yield return CoroutineTaskManager.Waits.OneSecond;
            
            UIBuildingAreaSelectionLayer.ShowUIBuildingAreaSelectionLayer(BuildingAreaAPI.GetBuildingSlots(areaIndex), GetBuildingsCount());
        }
        
        #endregion

        #region Function

        private void Initialize()
        {
            var slotsData = BuildingAreaAPI.GetBuildingSlots(areaIndex);
            for (var i = 0; i < buildingAreas.Length; i++)
            {
                var id = i + 1;
                var buildingArea = GetBuildingArea(i);
                buildingArea.SetUILockState(false);
                var succeed = slotsData.TryGetValue(id, out var data);
                if (!succeed)
                {
                    buildingArea.SetLevel(0);
                    buildingArea.SetUILockState(true);
                    return;
                }
                buildingArea.SetLevel(data.level);
                if (data.IsUpgrading())
                {
                    buildingArea.Upgrading();
                }
            }
        }

        private int GetBuildingsCount()
        {
            return buildingAreas.Length;
        }
        
        #endregion

        #region ApI

        public int GetBuildingAreaIndex()
        {
            return areaIndex;
        }

        public BuildingArea GetBuildingArea(int buildingIndex)
        {
            return buildingAreas[buildingIndex];
        }

        #endregion

        #region Callback

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (!response.IsSuccess()) return;
            if (service != "build") return;
            this.LogEditorOnly(JsonConvert.SerializeObject(response));
            
            if (method == "unlock")
            {
                var slotId = response.GetAttachmentAsInt("slot");
                var index = BuildingAreaAPI.GetIndexBySlotId(slotId).Item2;
                if (index < 0)
                {
                    this.LogEditorOnly($"slot {slotId} not found");
                    return;
                }
                var buildArea = GetBuildingArea(index);
                buildArea.SetUILockState(false);
                
                BuildingAreaAPI.UnlockBySlotId(areaIndex, index + 1);
                var container = UIBuildingAreaSelectionLayer.GetContainer();
                container.SetCardTypeByIndex(index, BuildingAreaType.EBuildAreaType.NotBuilt);
                return;
            }

            if (method == "upgrade")
            {
                var slotId = response.GetAttachmentAsInt("slot");
                var finishTime = response.GetAttachmentAsInt("finish-time");
                var coin = response.GetAttachmentAsInt("coin");
                var token = response.GetAttachmentAsFloat("token");
                PlayerSandbox.Instance.CharacterHandler.Coin -= coin;
                PlayerSandbox.Instance.CharacterHandler.Token -= token;

                
                var index = BuildingAreaAPI.GetIndexBySlotId(slotId).Item2;
                BuildingAreaAPI.UpgradeBySlotId(areaIndex, index + 1);
                
                var container = UIBuildingAreaSelectionLayer.GetContainer();
                container.SetCardTypeByIndex(index, BuildingAreaType.EBuildAreaType.Upgrading, (TimeAPI.GetUtcTimeStamp(), finishTime));
                
                var buildArea = GetBuildingArea(index);
                buildArea.Upgrading();
                return;
            }

            if (method == "withdraw")
            {
                var token = response.GetAttachmentAsFloat("token");
                PlayerSandbox.Instance.CharacterHandler.Token += Mathf.Approximately(token, -1) ? 0 : token;

                var task = new List<IQueueableEvent>
                {
                    EffectsAPI.CreateTip(() => 0, () => 0, () => token),
                    EffectsAPI.CreateSoundEffect(() => EffectsAPI.EEffectType.Token),
                    EffectsAPI.CreateScreenFullEffect(() => 0, () => 0, () => token)
                };
                EventQueue.Instance.Enqueue(task);
            }
        }

        #endregion

    }

}