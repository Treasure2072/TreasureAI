using System;
using System.Collections.Generic;
using DragonLi.Core;
using DragonLi.Network;
using Game;
using Newtonsoft.Json;

namespace Data
{
    public class BuildingAreaHandler : SandboxHandlerBase, IMessageReceiver
    {

        private const string kBuildAreaKey = "build-area";

        #region Properties - Event

        public event Action<BuildAreas, BuildAreas> OnBuildingAreaChanged;

        #endregion
        

        #region Properties - Data

        public BuildAreas BuildAreas
        {
            get => SandboxValue.GetValue<BuildAreas>(kBuildAreaKey);
            set => SandboxValue.SetValue(kBuildAreaKey, value);
        }

        #endregion
        
        #region Function - SandboxHandlerBase

        protected override void OnInitSandboxCallbacks(Dictionary<string, Action<object, object>> sandboxCallbacks)
        {
            base.OnInitSandboxCallbacks(sandboxCallbacks);
            if (sandboxCallbacks == null)
            {
                throw new ArgumentNullException(nameof(sandboxCallbacks));
            }
            sandboxCallbacks[kBuildAreaKey] = (preValue, nowValue) => OnBuildingAreaChanged?.Invoke((BuildAreas)preValue, (BuildAreas)nowValue);
        }

        protected override void OnInit()
        {
            base.OnInit();
            GameSessionAPI.BuildAreaAPI.Query();
        }

        #endregion
        
        #region Function - IMessageReceiver

        public void OnReceiveMessage(HttpResponseProtocol response, string service, string method)
        {
            if (service == GameSessionAPI.BuildAreaAPI.ServiceName && method == GSBuildAreaAPI.MethodQuery)
            {
                var json = response.GetAttachmentAsString("areas");
                BuildAreas = JsonConvert.DeserializeObject<BuildAreas>(json);
            }
        }

        #endregion
    }
}