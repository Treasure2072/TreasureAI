using UnityEngine;

namespace Data
{
    public class TutorialHandler
    {
        public static string FirstEnterKey => $"{PlayerSandbox.Instance.RegisterAndLoginHandler.Email}_first-enter-key";
        public static string FirstStandLandKey => $"{PlayerSandbox.Instance.RegisterAndLoginHandler.Email}_first-stand-land-key";
        public static string FirstStandShortKey => $"{PlayerSandbox.Instance.RegisterAndLoginHandler.Email}_first-stand-short-key";
        public static string FirstStandInvestKey => $"{PlayerSandbox.Instance.RegisterAndLoginHandler.Email}_first-stand-invest-key";
        public static string FirstUpgradeCharacterKey => $"{PlayerSandbox.Instance.RegisterAndLoginHandler.Email}_first-upgrade-character-key";
        public static string FirstLackDiceKey => $"{PlayerSandbox.Instance.RegisterAndLoginHandler.Email}_first-lack-dice-key";
        public static string FirstScratchKey => $"{PlayerSandbox.Instance.RegisterAndLoginHandler.Email}_first-scratch-key";
        
        #region API

        public static bool IsTriggerable(string eventKey)
        {
            // return true;
            return !PlayerPrefs.HasKey(eventKey) || PlayerPrefs.GetInt(eventKey) == 0;
        }

        public static void Triggered(string eventKey)
        {
            PlayerPrefs.SetInt(eventKey, 1);
            PlayerPrefs.Save();
        }

        public static void ClearCache()
        {
            PlayerPrefs.DeleteKey(FirstEnterKey);
            PlayerPrefs.DeleteKey(FirstStandLandKey);
            PlayerPrefs.DeleteKey(FirstStandShortKey);
            PlayerPrefs.DeleteKey(FirstStandInvestKey);
            PlayerPrefs.DeleteKey(FirstUpgradeCharacterKey);
            PlayerPrefs.DeleteKey(FirstLackDiceKey);
            PlayerPrefs.DeleteKey(FirstScratchKey);
        }

        #endregion
    }
}