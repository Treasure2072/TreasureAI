using DragonLi.Frame;
using UnityEngine;

namespace Game
{
    public abstract class GameMode : MonoBehaviour
    {
        public static readonly string WorldObjectRegisterKey = "GameMode";

        public static T GetGameMode<T>() where T : GameMode
        {
            return GetGameMode<T>(WorldObjectRegisterKey);
        }

        public static T GetGameMode<T>(string key) where T : GameMode
        {
            var gameMode = World.GetRegisteredObject<T>(key);
#if UNITY_EDITOR
            Debug.Assert(gameMode);
#endif
            return gameMode;
        }

        [ContextMenu("Reset Game Mode")]
        public void ResetGameMode()
        {
            var worldObject = GetComponent<WorldObjectRegister>();
            if (!worldObject)
            {
                gameObject.AddComponent<WorldObjectRegister>();
            }

            worldObject.Key = WorldObjectRegisterKey;
        }
    }
}


