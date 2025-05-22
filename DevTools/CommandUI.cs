using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.EventSystems;
using Rewired.Demos;

namespace DevTools
{
    public class CommandUI
    {
        public static void OpenDialogInputBox()
        {
            if (PlayerPatch.isUICreated) return;

            PlayerPatch.canvas = new GameObject("CommandCanvas").AddComponent<Canvas>();
            PlayerPatch.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            PlayerPatch.canvas.gameObject.AddComponent<CanvasScaler>();
            PlayerPatch.canvas.gameObject.AddComponent<GraphicRaycaster>();

            PlayerPatch.panelObject = new GameObject("InputPanel");
            RectTransform panelRect = PlayerPatch.panelObject.AddComponent<RectTransform>();
            panelRect.SetParent(PlayerPatch.canvas.transform);
            panelRect.sizeDelta = new Vector2(400, 40); 
            panelRect.anchoredPosition = new Vector2(-750, -487); 

            Image panelImage = PlayerPatch.panelObject.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f); 
            panelImage.color = new Color(1, 0, 0, 0.7f);

            Shadow shadow = PlayerPatch.panelObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.7f); 
            shadow.effectDistance = new Vector2(2, -2);

            PlayerPatch.inputFieldObject = new GameObject("CommandInputField");
            PlayerPatch.inputFieldObject.transform.SetParent(PlayerPatch.panelObject.transform);

            RectTransform inputRect = PlayerPatch.inputFieldObject.AddComponent<RectTransform>();
            inputRect.sizeDelta = new Vector2(380, 40); 
            inputRect.anchoredPosition = new Vector2(0, 0);

            PlayerPatch.inputField = PlayerPatch.inputFieldObject.AddComponent<InputField>();

            Text inputText = new GameObject("InputText").AddComponent<Text>();
            inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); 
            inputText.color = Color.white;
            inputText.alignment = TextAnchor.MiddleLeft; 
            inputText.rectTransform.sizeDelta = new Vector2(380, 40);
            inputText.transform.SetParent(PlayerPatch.inputFieldObject.transform, false); 
            PlayerPatch.inputField.textComponent = inputText;

            Text placeholder = new GameObject("Placeholder").AddComponent<Text>();
            placeholder.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); 
            placeholder.color = new Color(1f, 1f, 1f, 0.5f); 
            placeholder.text = "Enter command...";
            placeholder.rectTransform.sizeDelta = new Vector2(380, 40);
            placeholder.alignment = TextAnchor.MiddleLeft; 
            placeholder.transform.SetParent(PlayerPatch.inputFieldObject.transform, false); 
            PlayerPatch.inputField.placeholder = placeholder; 

            PlayerPatch.inputField.interactable = true;
            PlayerPatch.inputFieldObject.SetActive(true);
            PlayerPatch.inputField.ActivateInputField();

            DevTools.Logger.LogInfo("Command Input Field created.");

            PlayerPatch.isUICreated = true;
        }

        public static Text CreateTextComponent(string name, Color color)
        {
            GameObject textObject = new GameObject(name);
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = color;

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            if (rectTransform == null) rectTransform = textObject.AddComponent<RectTransform>();
            return text;
        }

        public static void OnSubmitCommand()
        {
            string command = PlayerPatch.inputField.text.Trim();
            if (!string.IsNullOrEmpty(command))
            {
                CommandManager.ExecuteCommand(command);
                DevTools.Logger.LogInfo($"Command Executed: {command}");
            }
            CloseDialogInputBox();
        }

        public static void CloseDialogInputBox()
        {
            if (PlayerPatch.panelObject != null)
            {
                Player.Instance.EnableCharacterController(true);
                Player.Instance.EnablePlayerMouseLook(true, true);
                InputController.Instance.enabled = true;
                UnityEngine.Object.Destroy(PlayerPatch.panelObject);
                PlayerPatch.sessionData.ResumeGame();
                DevTools.Logger.LogInfo("Command Input Field destroyed.");
            }
            PlayerPatch.isUICreated = false;
            PlayerPatch.isInputActive = false;
        }
    }
}
