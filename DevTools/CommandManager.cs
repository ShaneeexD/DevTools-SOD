using System;
using System.Collections.Generic;
using System.Linq;
using SOD.Common;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static Il2CppMono.Security.X509.X520;

namespace DevTools
{
    public static class CommandManager
    {
        private static Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();
        private static bool godmode = false;

        public static MurderController murderController;
        public static Player player;
        public static PlayerInfoProvider playerInfoProvider;
        public static VictimInfoHelper victimInfoHelper;
        public static MurdererInfoProvider murdererInfoProvider;
        public static GameplayController controller;

        static CommandManager()
        {
            Initialize(); // Automatically initialize commands on load
        }

        public static void Initialize()
        {
            RegisterCommand("/help", HelpCommand);
            RegisterCommand("/hello", HelloCommand);
            RegisterCommand("/goodbye", GoodbyeCommand);
            RegisterCommand("/say", SayCommand);
            RegisterCommand("/poison", PoisonEntityCommand);
            RegisterCommand("/kill", KillEntityCommand);
            RegisterCommand("/killall", KillAllCommand);
            RegisterCommand("/destroyMe", DeleteMeCommand);
            RegisterCommand("/tp", TeleportCommand);
            RegisterCommand("/tpe", TeleportEntityCommand);
            RegisterCommand("/mName", GetMurdererFullName);
            RegisterCommand("/vName", GetVictimFullName);
            RegisterCommand("/god", GodCommand);
            RegisterCommand("/resetHealth", ResetHealthCommand);
            RegisterCommand("/giveMoney", GiveMoneyCommand);
            RegisterCommand("/currentNode", CurrentNodeCommand);
            RegisterCommand("/pos", PosCommand);
            RegisterCommand("/passcode", GetEntityPasscodeCommand);
            RegisterCommand("/setPasscode", SetPasscodeCommand);
            RegisterCommand("/allowedEverywhere", allowedEverywhereCommand);
            RegisterCommand("/disableBadEffects", disableBadEffectsCommand);
        }

        public static void RegisterCommand(string commandName, Action<string[]> commandAction)
        {
            commands[commandName.ToLower()] = commandAction;
        }

        public static void ExecuteCommand(string command)
        {
            playerInfoProvider = new PlayerInfoProvider();
            victimInfoHelper = new VictimInfoHelper();
            murdererInfoProvider = new MurdererInfoProvider();
            player = Player.Instance;
            murderController = MurderController.Instance;

            if (command.StartsWith("/"))
            {
                string[] splitCommand = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (splitCommand.Length > 0)
                {
                    string commandName = splitCommand[0].ToLower(); //case-insensitive
                    string[] args = splitCommand.Skip(1).ToArray(); // Get arguments after the command name

                    // Check if the command exists and invoke it
                    if (commands.TryGetValue(commandName, out Action<string[]> commandAction))
                    {
                        commandAction(args);
                    }
                    else
                    {
                        Lib.GameMessage.ShowPlayerSpeech($"Unknown command: {commandName}", 2, true);
                    }
                }
            }
            else
            {
                SendChatMessage(command);
            }
        }

        private static void SendChatMessage(string message)
        {
            Lib.GameMessage.ShowPlayerSpeech(message, 5, true);
            DevTools.Logger.LogInfo($"Chat Message Sent: {message}");
        }

        private static void HelloCommand(string[] args)
        {
            DevTools.Logger.LogInfo("Hello, World!");
            Lib.GameMessage.ShowPlayerSpeech("Hello, Player!", 2, true);
        }

        private static void GoodbyeCommand(string[] args)
        {
            DevTools.Logger.LogInfo("Goodbye, World!");
            Lib.GameMessage.ShowPlayerSpeech("Goodbye, Player!", 2, true);
        }

        private static void SayCommand(string[] args)
        {
            if (args.Length < 3) // Ensure there are at least 3 arguments: message, float, and bool
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /say <string> <float> <bool>", 2, true);
                return;
            }

            // Join the first part of the args into a message
            string message = string.Join(" ", args.Take(args.Length - 2)); // Join all except last two for message
            string floatString = args[args.Length - 2]; // Second to last argument (float)
            string boolString = args[args.Length - 1]; // Last argument (bool)

            // Parse the float value
            if (!float.TryParse(floatString, out float someFloatValue))
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid float value. Please provide a valid number.", 2, true);
                return;
            }

            // Parse the boolean value
            if (!bool.TryParse(boolString, out bool someBoolValue))
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid boolean value. Please provide 'true' or 'false'.", 2, true);
                return;
            }

            // Output the message with parsed values
            Lib.GameMessage.ShowPlayerSpeech(message, someFloatValue, someBoolValue);
            DevTools.Logger.LogInfo($"Command Executed: {message}, Float: {someFloatValue}, Bool: {someBoolValue}");
        }

        private static void DeleteMeCommand(string[] args)
        {
            FirstPersonController fpsController = GameObject.FindObjectOfType<FirstPersonController>();
            UnityEngine.Object.Destroy(fpsController);
            Lib.GameMessage.ShowPlayerSpeech("Deleting the player, wow!", 2, true);
        }

        private static void TeleportCommand(string[] args)
        {
            if (args.Length < 3) 
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /tp <x> <y> <z>", 2, true);
                return;
            }

            if (!float.TryParse(args[0], out float x) ||
                !float.TryParse(args[1], out float y) ||
                !float.TryParse(args[2], out float z))
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid coordinates. Please provide valid numbers.", 2, true);
                return;
            }

            Vector3 newLocation = new Vector3(x, y, z);
            
            playerInfoProvider.SetPlayerLocation(newLocation);

            Lib.GameMessage.ShowPlayerSpeech($"Teleported to {newLocation}", 2, true);
        }

        private static void TeleportEntityCommand(string[] args)
        {
            if (args.Length != 2)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /tpe <source> <destination>", 5, true);
                return;
            }

            string sourceEntity = args[0].ToLower();
            string destinationEntity = args[1].ToLower();

            Vector3 newLocation;

            switch (destinationEntity)
            {
                case "player":
                    newLocation = playerInfoProvider.GetPlayerLocation(); 
                    break;

                case "murderer":
                    newLocation = murdererInfoProvider.GetMurdererLocation(); 
                    break;

                case "victim":
                    newLocation = victimInfoHelper.GetVictimLocation(); 
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid destination. Use 'player', 'murderer', 'victim'.", 2, true);
                    return;
            }

            // Teleport the source entity to the new location
            switch (sourceEntity)
            {
                case "player":
                    playerInfoProvider.SetPlayerLocation(newLocation);
                    Lib.GameMessage.ShowPlayerSpeech("Teleported player to new location.", 2, true);
                    break;

                case "murderer":
                    murdererInfoProvider.SetMurdererLocation(newLocation);
                    Lib.GameMessage.ShowPlayerSpeech("Teleported murderer to new location.", 2, true);
                    break;

                case "victim":
                    victimInfoHelper.SetVictimLocation(newLocation);
                    Lib.GameMessage.ShowPlayerSpeech("Teleported victim to new location.", 2, true);
                    break;

                case "all":
                    foreach (Citizen citizen in CityData.Instance.citizenDirectory)
                    {
                        citizen.transform.position = newLocation;
                    }
                    Lib.GameMessage.ShowPlayerSpeech("Teleported all citizens to new location.", 2, true);
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid source. Use 'player' or 'murderer'  or 'all'.", 2, true);
                    break;
            }
        }

        private static void PoisonEntityCommand(string[] args)
        {
            if (args.Length != 2)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /poison <entity> <amount>", 2, true);
                return;
            }

            string sourceEntity = args[0].ToLower();
            if (!float.TryParse(args[1], out float amount)) {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /poison <entity> <amount>.", 2, true);

            }

            switch (sourceEntity)
            {
                case "player":
                    playerInfoProvider.AddPoisoned(amount, Player.Instance);
                    break;

                case "murderer":
                    murdererInfoProvider.AddPoisoned(amount, Player.Instance);
                    break;

                case "victim":
                    victimInfoHelper.AddPoisoned(amount, Player.Instance);
                    break;

                case "all":
                    foreach (Citizen citizen in CityData.Instance.citizenDirectory)
                    {
                        citizen.AddPoisoned(amount, murderController.currentMurderer);
                    }
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid entity. Use 'player', 'murderer', 'victim' or 'all'.", 2, true);
                    return;
            }
        }

        private static void KillEntityCommand(string[] args)
        {
            if (args.Length != 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /kill <entity>", 2, true);
                return;
            }

            string sourceEntity = args[0].ToLower();

            switch (sourceEntity)
            {
                case "player":
                    player.TriggerPlayerKO(Vector3.zero, 0, false);
                    break;

                case "murderer":
                    murdererInfoProvider.KillMurderer();
                    break;

                case "victim":
                    victimInfoHelper.KillVictim();
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid entity. Use 'player', 'murderer' or 'victim'.", 2, true);
                    return;
            }
        }

        private static void GetMurdererFullName(string[] args)
        {
            String name = murdererInfoProvider.GetMurdererFullName().ToString();
            Lib.GameMessage.ShowPlayerSpeech("Full name of murderer: " + name, 2, true);
        }
        private static void GetVictimFullName(string[] args)
        {
            String name = victimInfoHelper.GetVictimFullName().ToString();
            Lib.GameMessage.ShowPlayerSpeech("Full name of victim: " + name, 2, true);
        }
        private static void GodCommand(string[] args)
        {
            if (!godmode)
            {
                player.SetMaxHealth(99999999);
                player.SetHealth(99999999);
                Lib.GameMessage.ShowPlayerSpeech("Godmode enabled.", 2, true);
                godmode = true;
            }
            else
            {
                player.SetMaxHealth(100);
                player.SetHealth(100);
                Lib.GameMessage.ShowPlayerSpeech("Godmode disabled.", 2, true);
                godmode = false;
            }
        }
        private static void GiveMoneyCommand(string[] args)
        {
            if (args.Length != 1) 
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /givemoney <amount>", 2, true);
                return;
            }

            if (!int.TryParse(args[0], out int amount))
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid amount. Please provide a valid number.", 2, true);
                return;
            }

            controller = GameplayController.Instance;
            controller.AddMoney(amount, true, "Dev Action");

            Lib.GameMessage.ShowPlayerSpeech($"Given {amount} crows.", 2, true);
        }

        private static void CurrentNodeCommand(string[] args)
        {
            Lib.GameMessage.ShowPlayerSpeech($"Current Node: {playerInfoProvider.GetPlayerNode()}", 2, true);
        }

        private static void PosCommand(string[] args)
        {
            Vector3 pos = playerInfoProvider.GetPlayerLocation();
            Lib.GameMessage.ShowPlayerSpeech($"Current Position: {pos}", 2, true);
        }
        private static void KillAllCommand(string[] args)
        {
            foreach (Citizen citizen in CityData.Instance.citizenDirectory)
            {
                citizen.RecieveDamage(99999f, Player.Instance, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
                citizen.isDead = true;
            }
            Lib.GameMessage.ShowPlayerSpeech($"Killed everyone.", 2, true);
        }

        private static void GetEntityPasscodeCommand(string[] args)
        {
            if (args.Length != 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /passcode <entity>", 2, true);
                return;
            }

            string sourceEntity = args[0].ToLower();

            switch (sourceEntity)
            {
                case "player":
                    Lib.GameMessage.ShowPlayerSpeech("Your passcode is: " + playerInfoProvider.GetPassword().ToString(), 2, true);
                    break;

                case "murderer":
                    Lib.GameMessage.ShowPlayerSpeech("Murderer passcode is: " + murdererInfoProvider.GetPassword().ToString(), 2, true);
                    break;

                case "victim":
                    Lib.GameMessage.ShowPlayerSpeech("Victim passcode is: " + victimInfoHelper.GetPassword().ToString(), 2, true);
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid entity. Use 'player', 'murderer' or 'victim'.", 2, true);
                    return;
            }
        }
        private static void SetPasscodeCommand(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrEmpty(args[0]))
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /setpasscode <passcode>", 2, true);
                return;
            }

            string passcode = args[0];

            if (!passcode.All(char.IsDigit))
            {
                Lib.GameMessage.ShowPlayerSpeech("Passcode should only contain digits.", 2, true);
                return;
            }

            playerInfoProvider.SetPassword(passcode);

            Lib.GameMessage.ShowPlayerSpeech($"Passcode set to {passcode}.", 2, true);
        }
        private static void allowedEverywhereCommand(string[] args)
        {
            CityData cityData = CityData.Instance;
            foreach (var newLoc in cityData.gameLocationDirectory)
            {
                Player.Instance.AddLocationOfAuthorty(newLoc);
            }
            Lib.GameMessage.ShowPlayerSpeech("Authorising player everywhere.", 2, true);
        }
        private static void disableBadEffectsCommand(string[] args)
        {
            PlayerPatch.resetNegativeEffects = true;
        }
        private static void ResetHealthCommand(string[] args)
        {
            Player.Instance.ResetHealthToMaximum();
        }
        private static void HelpCommand(string[] args)
        {
            Application.OpenURL("https://www.Google.com");

        }
    }
}
