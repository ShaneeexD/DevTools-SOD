using HarmonyLib;
using BepInEx.Unity.IL2CPP.UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using static CaseComponent;
using static InteractableController;
using System.Collections;
using System.Reflection;
using System;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.EventSystems;
using Rewired.Demos;

namespace DevTools
{
    [HarmonyPatch(typeof(Player), "Update")]
    public class PlayerPatch
    {
        public static bool isUICreated = false;
        public static bool isInputActive = false;
        public static bool resetNegativeEffects = false;

        public static GameObject inputFieldObject;
        public static InputField inputField;
        public static GameObject panelObject;
        public static Canvas canvas;
        public static SessionData sessionData;
        public static InputController inputController;
        public static Game game = new Game();
        public static Player player = Player.Instance;

        [HarmonyPrefix]
        public static void Prefix(Player __instance)
        {
            if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Slash))
            {
                if (!isInputActive)
                {
                    CommandUI.OpenDialogInputBox();
                    isInputActive = true;
                }
            }

            if (isInputActive)
            {        
                Player.Instance.EnablePlayerMouseLook(false, false);
                Player.Instance.EnableCharacterController(false);
                inputController = new InputController();
                inputController.enableInput = false;
                sessionData = SessionData.Instance;
                sessionData.PauseGame(false, false, false);

                if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Return))
                {
                    CommandUI.OnSubmitCommand();
                }
                if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Escape))
                {
                    CommandUI.CloseDialogInputBox();
                }
            }

            if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.V) && !isInputActive)
            {
                game.SetFreeCamMode(true);
            }
            else
            {
                game.SetFreeCamMode(false);
            }

            if(resetNegativeEffects)
            {
                Player.Instance.ResetNegativeStatuses(10f);
            }
        }
    }
}
