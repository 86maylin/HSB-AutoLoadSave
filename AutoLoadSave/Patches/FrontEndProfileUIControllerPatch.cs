using BBI.Unity.Game;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace AutoLoadSave.Patches
{
    [HarmonyPatch(typeof(FrontEndProfileUIController))]
    internal class FrontEndProfileUIControllerPatch
    {
        [HarmonyPatch("SetupProfileButtons")]
        [HarmonyPostfix]
        static void SetupProfileButtons_Postfix(FrontEndProfileUIController __instance)
        {
            AutoLoadSave.LoggerInstance.LogInfo("SetupProfileButtons_Postfix");
            if (AutoLoadSave.HasLoadedSave) return;
            AutoLoadSave.Instance.StartCoroutine(SelectProfileButton(__instance));
        }

        static IEnumerator SelectProfileButton(FrontEndProfileUIController instance)
        {
            AutoLoadSave.LoggerInstance.LogInfo("SetupProfileButtons");
            yield return null;
            string buttonName;
            switch (AutoLoadSave.Config_shiftType.Value)
            {
                case (int)AutoLoadSave.SaveType.OpenShift:
                    buttonName = "Open Shift Button";
                    break;
                case (int)AutoLoadSave.SaveType.Limited:
                    buttonName = "Hard Button";
                    break;
                case (int)AutoLoadSave.SaveType.NoRevival:
                    buttonName = "Extreme Button";
                    break;
                case (int)AutoLoadSave.SaveType.Standard:
                default:
                    buttonName = "Normal Button";
                    break;
            }
            foreach (var button in Object.FindObjectsOfType<FrontEndProfileButton>(true))
            {
                if (button.name == buttonName)
                {
                    if (button.PlayerProfile == null)
                    {
                        AutoLoadSave.LoggerInstance.LogInfo("Playerprofile empty, can't load save.");
                        AutoLoadSave.HasLoadedSave = true;
                        yield break;
                    }
                    instance.CurrentlySelectedProfileButton = button;
                    break;
                }
            }
            yield return new WaitUntil(() => { return !SceneLoader.Instance.IsExecutingTearDownAndLoad; });
            if (AutoLoadSave.Config_shiftType.Value == (int)AutoLoadSave.SaveType.OpenShift)
            {
                instance.GoToNext(AutoLoadSave.Config_drainDisabled.Value);
            }
            else
            {
                instance.GoToNext(false);
            }
        }

        [HarmonyPatch(nameof(FrontEndProfileUIController.GoToNext))]
        [HarmonyPostfix]
        static void GoToNext_Postfix()
        {
            if (AutoLoadSave.HasLoadedSave) return;
            SceneLoader.Instance.TearDownAndLoadLevelWithPlayerProfile(PlayerProfileService.Instance.Profile.ProfileName, true);
            AutoLoadSave.HasLoadedSave = true;
        }
    }
}
