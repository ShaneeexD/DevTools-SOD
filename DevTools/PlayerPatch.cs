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
using SOD.Common;

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
                if (!isInputActive && MainMenuController.Instance.mainMenuActive == false && __instance.isInBed == false)
                {
                    CommandUI.OpenDialogInputBox();
                    isInputActive = true;
                }
            }

            if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Mouse3))
            {
                if (InteractionController.Instance.currentLookingAtInteractable != null)
                {
                    if (InteractionController.Instance.currentLookingAtInteractable.interactable.isActor as Human)
                    {
                        CommandManager.storedHuman = InteractionController.Instance.currentLookingAtInteractable.interactable.isActor as Human;

                        if (CommandManager.storedHuman != null)
                        {
                             Lib.GameMessage.ShowPlayerSpeech("Stored citizen: " + CommandManager.storedHuman.GetCitizenName().ToString(), 2, true);

                             StoredHumanInfoProvider.human = CommandManager.storedHuman;
                        }
                    }
                    else
                    {
                        CommandManager.storedItem = InteractionController.Instance.currentLookingAtInteractable.interactable;

                        Lib.GameMessage.ShowPlayerSpeech("Stored item: " + InteractionController.Instance.currentLookingAtInteractable.interactable.name.ToString(), 2, true);
                    }
                }
            }

            if (MainMenuController.Instance.mainMenuActive == true && isInputActive == true)
            {
                CommandUI.CloseDialogInputBox();
            }

            if (isInputActive)
            {        
                Player.Instance.EnablePlayerMouseLook(false, false);
                Player.Instance.EnableCharacterController(false);
                sessionData = SessionData.Instance;
                sessionData.PauseGame(false, false, false);
                InputController.Instance.enabled = false;
                
                if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Return))
                {
                    CommandUI.OnSubmitCommand();
                }
                if (Input.GetKeyInt(BepInEx.Unity.IL2CPP.UnityEngine.KeyCode.Escape))
                {
                    CommandUI.CloseDialogInputBox();
                }
            }

            if(resetNegativeEffects)
            {
                Player.Instance.ResetNegativeStatuses(10f);
            }
        }
    }
}
