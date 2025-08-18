using System;
using System.Collections.Generic;
using _Scripts.UI.Common.Grids;
using Data.Type;
using DragonLi.Core;
using DragonLi.UI;
using UnityEngine;

namespace Game
{
    public class UIConversationContainer : GridsContainerBase
    {
        public override void SpawnAllGrids(params object[] args)
        {
            base.SpawnAllGrids(args);
            var messages = args[0] as List<AIChatType.TChatMessage>;

            if (messages == null) return;
            for (var i = 0; i < messages.Count; i++)
            {
                SpawnMessage(messages[i], null);
            }
        }

        public void SpawnMessage(AIChatType.TChatMessage message, AudioClip clip)
        {
            var content = SpawnGrid<UIConversation>();
            content.SetGrid(message, clip);

            CoroutineTaskManager.Instance.WaitFrameEnd(LayoutRebuild);
        }
    }
}