using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SOD.Common;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static Il2CppMono.Security.X509.X520;
using System.IO;
using SOD.Common.Extensions;
using Il2CppSystem.Linq.Expressions.Interpreter;
using UnityEngine.UIElements;

namespace DevTools
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        public string CommandName { get; }
        public CommandAttribute(string commandName)
        {
            CommandName = commandName;
        }
    }

    public static class CommandManager
    {
        private static Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();
        private static Dictionary<string, Vector3> teleportPoints = new Dictionary<string, Vector3>();

        private static bool godmode = false;
        private static bool noclip = false;
        public static Human storedHuman;
        public static Interactable storedItem;

        public static MurderController murderController;
        public static Player player;
        public static PlayerInfoProvider playerInfoProvider;
        public static VictimInfoHelper victimInfoHelper;
        public static MurdererInfoProvider murdererInfoProvider;
        public static StoredHumanInfoProvider storedHumanInfoProvider;
        public static PurpInfoProvider purpInfoProvider;
        public static PosterInfoProvider posterInfoProvider;
        public static GameplayController controller;
        public static SideJob sideJob;

        static CommandManager()
        {
            Initialize(); // Automatically initialize commands on load
        }

        public static void Initialize()
        {
            var methods = typeof(CommandManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(m => m.GetCustomAttribute<CommandAttribute>() != null);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttribute<CommandAttribute>();
                RegisterCommands(attribute.CommandName, (Action<string[]>)Delegate.CreateDelegate(typeof(Action<string[]>), method));
            }
        }

        public static void RegisterCommands(string commandName, Action<string[]> commandAction)
        {
            commands[commandName.ToLower()] = commandAction;
        }

        public static void ExecuteCommand(string command)
        {
            playerInfoProvider = new PlayerInfoProvider();
            victimInfoHelper = new VictimInfoHelper();
            purpInfoProvider = new PurpInfoProvider();
            posterInfoProvider = new PosterInfoProvider();
            murdererInfoProvider = new MurdererInfoProvider();
            storedHumanInfoProvider = new StoredHumanInfoProvider();
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
            ExtModLog.LogInfo($"Chat Message Sent: {message}");
        }

        [Command("/say")]
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

        [Command("/cloneMe")]
        private static void CloneMeCommand(string[] args)
        {
            FirstPersonController fpsController = GameObject.FindObjectOfType<FirstPersonController>();

            if (fpsController != null)
            {
                GameObject newFpsController = UnityEngine.Object.Instantiate(fpsController.gameObject, new Vector3(1, 1, 1), fpsController.transform.rotation);

                newFpsController.name = fpsController.name + "player_clone";

                Lib.GameMessage.ShowPlayerSpeech("Cloning the player...", 2, true);
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech("Player not found!", 2, true);
            }
        }

        [Command("/tp")]
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

        [Command("/tpe")]
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

                case "poster":
                    newLocation = CasePanelController.Instance.activeCase.job.poster.transform.position;
                    break;

                case "purp":
                    newLocation = CasePanelController.Instance.activeCase.job.purp.transform.position;
                    break;

                case "stored":
                    if (storedHuman != null)
                    {
                        newLocation = storedHuman.transform.position;
                        break;
                    }
                    else
                    {
                        Lib.GameMessage.ShowPlayerSpeech("No current stored citizen!", 2, true);
                        return;
                    }

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid destination. Use 'player', 'murderer', 'victim', 'poster' 'purp', or 'stored'.", 2, true);
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

                case "poster":
                    CasePanelController.Instance.activeCase.job.poster.transform.position = newLocation;
                    break;

                case "purp":
                    CasePanelController.Instance.activeCase.job.purp.transform.position = newLocation;
                    break;

                case "stored":
                    if (storedHuman != null)
                    {
                        storedHuman.transform.position = newLocation;
                    }
                    else
                    {
                        Lib.GameMessage.ShowPlayerSpeech("No current stored citizen!", 2, true);
                    }
                    break;

                case "all":
                    foreach (Citizen citizen in CityData.Instance.citizenDirectory)
                    {
                        citizen.transform.position = newLocation;
                    }
                    Lib.GameMessage.ShowPlayerSpeech("Teleported all citizens to new location.", 2, true);
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid source. Use 'player' or 'murderer', 'poster', 'purp', 'all' or 'stored'.", 2, true);
                    break;
            }
        }

        [Command("/poison")]
        private static void PoisonEntityCommand(string[] args)
        {
            if (args.Length != 2)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /poison <entity> <amount>", 2, true);
                return;
            }

            string sourceEntity = args[0].ToLower();
            if (!float.TryParse(args[1], out float amount))
            {
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

                case "stored":
                    if (storedHuman != null)
                    {
                        storedHuman.AddPoisoned(amount, Player.Instance);
                    }
                    else
                    {
                        Lib.GameMessage.ShowPlayerSpeech("No current stored citizen!", 2, true);
                    }
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

        [Command("/kill")]
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

                case "poster":
                    CasePanelController.Instance.activeCase.job.poster.RecieveDamage(99999f, murderController.currentMurderer, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, true, true, 1f);
                    break;

                case "purp":
                    CasePanelController.Instance.activeCase.job.purp.RecieveDamage(99999f, murderController.currentMurderer, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, true, true, 1f);
                    break;

                case "stored":
                    if (storedHuman != null)
                    {
                        storedHuman.RecieveDamage(99999f, Player.Instance, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
                        storedHuman.isDead = true;
                    }
                    else
                    {
                        Lib.GameMessage.ShowPlayerSpeech("No current stored citizen!", 2, true);
                    }
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid entity. Use 'player', 'murderer', 'victim', 'poster or 'purp'.", 2, true);
                    return;
            }
        }

        [Command("/ko")]
        private static void KOEntityCommand(string[] args)
        {
            if (args.Length != 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /ko <entity>", 2, true);
                return;
            }

            string sourceEntity = args[0].ToLower();

            switch (sourceEntity)
            {
                case "player":
                    player.TriggerPlayerKO(Vector3.zero, 0, false);
                    break;

                case "murderer":
                    murdererInfoProvider.KOMurderer();
                    break;

                case "victim":
                    victimInfoHelper.KOVictim();
                    break;

                case "poster":
                    CasePanelController.Instance.activeCase.job.poster.RecieveDamage(99999f, murderController.currentMurderer, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
                    break;

                case "purp":
                    CasePanelController.Instance.activeCase.job.purp.RecieveDamage(99999f, murderController.currentMurderer, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
                    break;

                case "stored":
                    if (storedHuman != null)
                    {
                        storedHuman.RecieveDamage(99999f, murderController.currentMurderer, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
                    }
                    else
                    {
                        Lib.GameMessage.ShowPlayerSpeech("No current stored citizen!", 2, true);
                    }
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid entity. Use 'player', 'murderer', 'victim', 'poster or 'purp'", 2, true);
                    return;
            }
        }

        [Command("/mName")]
        private static void GetMurdererFullName(string[] args)
        {
            String name = murdererInfoProvider.GetMurdererFullName().ToString();
            Lib.GameMessage.ShowPlayerSpeech("Full name of murderer: " + name, 2, true);
        }

        [Command("/vName")]
        private static void GetVictimFullName(string[] args)
        {
            String name = victimInfoHelper.GetVictimFullName().ToString();
            Lib.GameMessage.ShowPlayerSpeech("Full name of victim: " + name, 2, true);
        }

        [Command("/sName")]
        private static void GetStoredFullName(string[] args)
        {
            String name = storedHumanInfoProvider.GetStoredFullName().ToString();
            Lib.GameMessage.ShowPlayerSpeech("Full name of citizen: " + name, 2, true);
        }

        [Command("/god")]
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

        [Command("/resetHealth")]
        private static void ResetHealthCommand(string[] args)
        {
            Player.Instance.ResetHealthToMaximum();
            Lib.GameMessage.ShowPlayerSpeech("Health reset.", 2, true);
        }

        [Command("/giveMoney")]
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

        [Command("/currentNode")]
        private static void CurrentNodeCommand(string[] args)
        {
            Lib.GameMessage.ShowPlayerSpeech($"Current Node: {playerInfoProvider.GetPlayerNode()}", 2, true);
        }

        [Command("/currentNodeCoord")]
        private static void CurrentNodeCoordCommand(string[] args)
        {
            Lib.GameMessage.ShowPlayerSpeech($"Current Node: {playerInfoProvider.GetPlayerNodeCoord()}", 2, true);
        }

        [Command("/pos")]
        private static void PosCommand(string[] args)
        {
            Vector3 pos = playerInfoProvider.GetPlayerLocation();
            Lib.GameMessage.ShowPlayerSpeech($"Current Position: {pos}", 2, true);
        }

        [Command("/killAll")]
        private static void KillAllCommand(string[] args)
        {
            foreach (Citizen citizen in CityData.Instance.citizenDirectory)
            {
                citizen.RecieveDamage(99999f, Player.Instance, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
                citizen.isDead = true;
            }
            Lib.GameMessage.ShowPlayerSpeech($"Killed everyone.", 2, true);
        }

        [Command("/koAll")]
        private static void KOAllCommand(string[] args)
        {
            foreach (Citizen citizen in CityData.Instance.citizenDirectory)
            {
                citizen.RecieveDamage(99999f, Player.Instance, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
            }
            Lib.GameMessage.ShowPlayerSpeech($"KO'd everyone.", 2, true);
        }

        [Command("/passcode")]
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

                case "poster":
                    Lib.GameMessage.ShowPlayerSpeech("Poster passcode is: " + purpInfoProvider.GetPassword().ToString(), 2, true);
                    break;

                case "purp":
                    Lib.GameMessage.ShowPlayerSpeech("Purp passcode is: " + purpInfoProvider.GetPassword().ToString(), 2, true);
                    break;

                case "stored":
                    if (storedHuman != null)
                    {
                        Lib.GameMessage.ShowPlayerSpeech("Stored citizen passcode is: " + storedHumanInfoProvider.GetPassword().ToString(), 2, true);
                    }
                    else
                    {
                        Lib.GameMessage.ShowPlayerSpeech("No current stored citizen!", 2, true);
                    }
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid entity. Use 'player', 'murderer', 'victim', 'poster' or 'purp'.", 2, true);
                    return;
            }
        }

        [Command("/setPasscode")]
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

        [Command("/allowEverywhere")]
        private static void AllowedEverywhereCommand(string[] args)
        {
            CityData cityData = CityData.Instance;
            foreach (var newLoc in cityData.gameLocationDirectory)
            {
                Player.Instance.AddLocationOfAuthorty(newLoc);
            }
            Lib.GameMessage.ShowPlayerSpeech("Authorising player everywhere.", 2, true);
        }

        [Command("/room")]
        private static void GetCurrentRoomNameAndPreset(string[] args)
        {
            NewRoom newRoom = Player.Instance.currentRoom;
            string roomName = newRoom.name != null ? newRoom.name : "unnamed";
            string roomPreset = newRoom.preset != null ? newRoom.preset.name : "no preset";
            string buildingName = newRoom.building != null ? newRoom.building.name : "unknown building";
            string floorName = newRoom.floor != null ? newRoom.floor.name : "unknown floor";
            // Get the actual building name from GameLocation if possible
            string actualBuildingName = "unknown";
            string initialBuildingName = "unknown";
            if (newRoom.building != null)
            {
                // Try to find the GameLocation that corresponds to this building
                foreach (var location in CityData.Instance.gameLocationDirectory)
                {
                    if (location != null && location.thisAsAddress != null && 
                        location.thisAsAddress.building.buildingID == newRoom.building.buildingID)
                    {                
                        actualBuildingName = location.thisAsAddress.name;
                        initialBuildingName = location.name;
                        break;
                    }
                }
            }
    
            Lib.GameMessage.ShowPlayerSpeech($"Room: {roomName}\nPreset: {roomPreset}\nFloor: {floorName}", 5, true);
        }

        [Command("/furni")]
        private static void ListFurnitureInRoom(string[] args)
        {
            NewRoom currentRoom = Player.Instance.currentRoom;
            if (currentRoom == null)
            {
                Lib.GameMessage.ShowPlayerSpeech("Could not determine current room.", 3, true);
                return;
            }

            if (currentRoom.individualFurniture == null || currentRoom.individualFurniture.Count == 0)
            {
                Lib.GameMessage.ShowPlayerSpeech("No furniture found in this room.", 3, true);
                return;
            }

            // Collect unique furniture preset names
            HashSet<string> uniqueFurniturePresets = new HashSet<string>();
            foreach (var furniture in currentRoom.individualFurniture)
            {
                if (furniture != null && furniture.furniture != null)
                {
                    uniqueFurniturePresets.Add(furniture.furniture.name);
                }
            }

            // Create a message with all furniture preset names
            string furnitureList = string.Join("\n", uniqueFurniturePresets.OrderBy(name => name));

            // Create a JSON-formatted version for clipboard
            string jsonFormatted = string.Join("\", \"", uniqueFurniturePresets.OrderBy(name => name));
            if (!string.IsNullOrEmpty(jsonFormatted))
            {
                jsonFormatted = "[\"" + jsonFormatted + "\"]";
            }
            else
            {
                jsonFormatted = "[]";
            }

            // Save to a file that can be easily accessed
            try
            {
                string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string filePath = Path.Combine(directoryPath, "furniture_list.json");
                File.WriteAllText(filePath, jsonFormatted);
                Lib.GameMessage.ShowPlayerSpeech($"Furniture in {currentRoom.name} saved to: {filePath}", 5, true);
            }
            catch (Exception ex)
            {
                // If file write fails, just show the list
                Lib.GameMessage.ShowPlayerSpeech($"Furniture in {currentRoom.name} failed to save JSON: {ex.Message}", 5, true);
            }
        }

        [Command("/allfloors")]
        private static void ListAllFloors(string[] args)
        {
            // Get all floor presets from the game
            HashSet<string> allFloorPresets = new HashSet<string>();
            
            // Access the floor directory from CityData
            if (CityData.Instance != null && CityData.Instance.floorDirectory != null)
            {
                foreach (var floor in CityData.Instance.floorDirectory)
                {
                    if (floor != null && !string.IsNullOrEmpty(floor.name))
                    {
                        allFloorPresets.Add(floor.name);
                    }
                }
            }

            // Create a JSON-formatted version
            string jsonFormatted = string.Join("\", \"", allFloorPresets.OrderBy(name => name));
            if (!string.IsNullOrEmpty(jsonFormatted))
            {
                jsonFormatted = "[\"" + jsonFormatted + "\"]";
            }
            else
            {
                jsonFormatted = "[]";
            }

            // Save to a file that can be easily accessed
            try
            {
                string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string filePath = Path.Combine(directoryPath, "all_floors_list.json");
                File.WriteAllText(filePath, jsonFormatted);
                Lib.GameMessage.ShowPlayerSpeech($"All floor presets ({allFloorPresets.Count}) saved to: {filePath}", 5, true);
            }
            catch (Exception ex)
            {
                // If file write fails, just show a message
                Lib.GameMessage.ShowPlayerSpeech($"Failed to save floor presets to JSON: {ex.Message}", 5, true);
            }
        }

        [Command("/disableBadEffects")]
        private static void DisableBadEffectsCommand(string[] args)
        {
            PlayerPatch.resetNegativeEffects = true;
            Lib.GameMessage.ShowPlayerSpeech("Negative effects disabled.", 2, true);
        }

        [Command("/help")]
        private static void HelpCommand(string[] args)
        {
            Application.OpenURL("https://github.com/ShaneeexD/DevTools-SOD/wiki");
        }

        [Command("/noclip")]
        private static void NoClipCommand(string[] args)
        {
            if (!noclip)
            {
                noclip = true;
                Game.Instance.SetFreeCamMode(true);
                Lib.GameMessage.ShowPlayerSpeech($"Noclip enabled.", 2, true);

            }
            else
            {
                noclip = false;
                Game.Instance.SetFreeCamMode(false);
                Lib.GameMessage.ShowPlayerSpeech($"Noclip disabled.", 2, true);
            }
        }

        [Command("/compSideJob")]
        private static void CompleteSideJobCommand(string[] args)
        {
            CasePanelController.Instance.activeCase.job.Complete();
        }

        [Command("/giveSocialCredit")]
        private static void GiveSocialCreditCommand(string[] args)
        {
            if (args.Length != 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /giveSocialCredit <type>", 2, true);
                return;
            }

            string type = args[0].ToLower();


            switch (type)
            {
                case "murdercase":
                    GameplayController.Instance.AddSocialCredit(GameplayControls.Instance.socialCreditForMurders, true, "cheat");
                    break;

                case "sidejob":
                    GameplayController.Instance.AddSocialCredit(GameplayControls.Instance.socialCreditForSideJobs, true, "cheat");

                    break;

                case "lostandfound":
                    GameplayController.Instance.AddSocialCredit(GameplayControls.Instance.socialCreditForLostAndFound, true, "cheat");
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Unknown social credit type. Use: murdercase, sidejob, lostandfound", 2, true);
                    return;
            }

            Lib.GameMessage.ShowPlayerSpeech($"Given social credit for {type}.", 2, true);
        }

        [Command("/sideJobDetails")]
        private static void SideJobDetailsCommand(string[] args)
        {
            if (CasePanelController.Instance.activeCase != null)
            {
                string caseId = CasePanelController.Instance.activeCase.id.ToString();
                string purp = CasePanelController.Instance.activeCase.job.purp.name.ToString();
                string poster = CasePanelController.Instance.activeCase.job.poster.name.ToString();

                if (caseId != null)
                {
                    Lib.GameMessage.ShowPlayerSpeech("Case ID is: " + caseId, 2, true);
                }
                if (poster != null)
                {
                    Lib.GameMessage.ShowPlayerSpeech("Poster is: " + poster, 2, true);
                }
                if (purp != null)
                {
                    Lib.GameMessage.ShowPlayerSpeech("Purp is: " + purp, 2, true);
                }
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech("No active side job!", 2, true);
            }
        }

        [Command("/removeAttackers")]
        private static void RemoveAttackers(string[] args)
        {
            foreach (Citizen citizen in CityData.Instance.citizenDirectory)
            {
                citizen.ai.EndAttack();
                citizen.ai.attackActive = false;
                citizen.ai.CancelCombat();
                Player.Instance.RemovePersuedBy(citizen);
            }
        }

        [Command("/itemTest")]
        private static void ItemTestCommand(string[] args)
        {
            if (storedItem != null)
            {
                player = Player.Instance;
                storedItem.IsSafeToDelete(true);
                //storedItem.furnitureParent.Delete(true, FurnitureClusterLocation.RemoveInteractablesOption.remove);
                GameObject furniture = storedItem.furnitureParent.spawnedObject.gameObject; //Parent
                GameObject furnitureItem = storedItem.spawnedObject.gameObject; //Item
                SpawnFurnitureAtLookPosition(furniture);
                SpawnFurnitureAtLookPosition(furnitureItem);
                //Lib.GameMessage.ShowPlayerSpeech("Deleting Object!", 2, true);
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech("No item stored!", 2, true);
            }
        }

        private static void SpawnFurnitureAtLookPosition(GameObject furniturePrefab)
        {
            RaycastHit raycastHit = InteractionController.Instance.playerCurrentRaycastHit;

            if (raycastHit.collider != null)
            {
                Vector3 hitPoint = raycastHit.point;

                // Instantiate the furniture at the hit point
                GameObject.Instantiate(furniturePrefab, hitPoint, Quaternion.identity);

                // Optionally, align the furniture with the surface normal
                // Quaternion rotation = Quaternion.LookRotation(raycastHit.normal);
                // Instantiate(furniturePrefab, hitPoint, rotation);
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech("No valid raycast hit.", 2, true);
            }
        }

        [Command("/job")]
        private static void GetEntityJobCommand(string[] args)
        {
            if (args.Length != 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /job <entity>", 2, true);
                return;
            }

            string entity = args[0].ToLower();

            switch (entity)
            {
                case "murderer":
                    murdererInfoProvider.GetJob();
                    Lib.GameMessage.ShowPlayerSpeech(murdererInfoProvider.GetJob(), 2, true);

                    break;

                case "victim":
                    victimInfoHelper.GetJob();
                    Lib.GameMessage.ShowPlayerSpeech(victimInfoHelper.GetJob(), 2, true);

                    break;

                case "poster":
                    posterInfoProvider.GetJob();
                    Lib.GameMessage.ShowPlayerSpeech(posterInfoProvider.GetJob(), 2, true);

                    break;

                case "purp":
                    purpInfoProvider.GetJob();
                    Lib.GameMessage.ShowPlayerSpeech(purpInfoProvider.GetJob(), 2, true);
                    break;

                case "stored":
                    if (storedHuman != null)
                    {
                        storedHumanInfoProvider.GetJob();
                        Lib.GameMessage.ShowPlayerSpeech(storedHumanInfoProvider.GetJob(), 2, true);
                    }
                    else
                    {
                        Lib.GameMessage.ShowPlayerSpeech("No current stored citizen!", 2, true);
                    }
                    break;


                default:
                    Lib.GameMessage.ShowPlayerSpeech("Invalid entity. Use 'murderer', 'victim', 'Poster', 'Purp' or 'Stored'.", 2, true);
                    return;
            }
        }

        [Command("/testStoredScale")]
        private static void TestStored(string[] args)
        {
            if (float.TryParse(args[0], out float scale))
            {
                storedHuman.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        [Command("/testStoredSpawn")]
        private static void TestStoredClone(string[] args)
        {
            if (storedHuman != null)
            {
                GameObject oldHuman = storedHuman.gameObject;

                GameObject.Instantiate(oldHuman, new Vector3(1, 1, 1), Quaternion.identity);

                Lib.GameMessage.ShowPlayerSpeech("Spawned Citizen!", 2, true);
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech("No current stored citizen!", 2, true);
            }
        }

        [Command("/createTP")]
        private static void SetTeleport(string[] args)
        {
            if (args.Length < 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Please provide a name for the teleport point.", 2, true);
                return;
            }

            string teleportName = args[0];
            Vector3 pos = playerInfoProvider.GetPlayerLocation();

            if (teleportPoints.ContainsKey(teleportName))
            {
                Lib.GameMessage.ShowPlayerSpeech($"Teleport point '{teleportName}' updated to new position: {pos}", 2, true);
                teleportPoints[teleportName] = pos;
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech($"Teleport point '{teleportName}' created at position: {pos}", 2, true);
                teleportPoints.Add(teleportName, pos);
            }
        }

        [Command("/tpc")]
        private static void TeleportToPoint(string[] args)
        {
            if (args.Length < 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Please provide the name of the teleport point.", 2, true);
                return;
            }

            string teleportName = args[0];

            if (teleportPoints.TryGetValue(teleportName, out Vector3 targetPosition))
            {
                playerInfoProvider.SetPlayerLocation(targetPosition);
                Lib.GameMessage.ShowPlayerSpeech($"Teleported to '{teleportName}' at position: {targetPosition}", 2, true);
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech($"Teleport point '{teleportName}' does not exist.", 2, true);
            }
        }

        [Command("/showTPList")]
        private static void ShowTeleportPoints(string[] args)
        {
            if (teleportPoints.Count == 0)
            {
                Lib.GameMessage.ShowPlayerSpeech("No teleport points have been set.", 2, true);
                return;
            }

            System.Text.StringBuilder pointsList = new System.Text.StringBuilder("Teleport Points:\n");
            foreach (var point in teleportPoints)
            {
                pointsList.AppendLine($"{point.Key}: {point.Value}");
            }
            Lib.GameMessage.ShowPlayerSpeech(pointsList.ToString(), 5, true);
        }

        [Command("/deleteTP")]
        private static void DeleteTeleportPoint(string[] args)
        {
            if (args.Length < 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Please provide the name of the teleport point to delete.", 2, true);
                return;
            }

            string teleportName = args[0];

            if (teleportPoints.Remove(teleportName))
            {
                Lib.GameMessage.ShowPlayerSpeech($"Teleport point '{teleportName}' has been deleted.", 2, true);
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech($"Teleport point '{teleportName}' does not exist.", 2, true);
            }
        }

        [Command("/tpp")]
        private static void TeleportPresetCommand(string[] args)
        {
            if (args.Length < 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /tpp <presetname>", 2, true);
                return;
            }

            NewNode newLocation = null;

            switch (args[0].ToLower())
            {
                case "home":
                    if (player.home != null) newLocation = player.home.GetDestinationNode();
                    break;

                case "mwork":
                    if (murderController.currentMurderer?.workPosition?.node != null)
                        newLocation = murderController.currentMurderer.workPosition.node;
                    break;

                case "mhome":
                    if (murderController.currentMurderer?.home != null)
                        newLocation = murderController.currentMurderer.home.GetDestinationNode();
                    break;

                case "vwork":
                    if (murderController.currentVictim?.workPosition?.node != null)
                        newLocation = murderController.currentVictim.workPosition.node;
                    break;

                case "vhome":
                    if (murderController.currentVictim?.home != null)
                        newLocation = murderController.currentVictim.home.GetDestinationNode();
                    break;

                case "posterwork":
                    if (CasePanelController.Instance.activeCase?.job?.poster?.workPosition?.node != null)
                        newLocation = CasePanelController.Instance.activeCase.job.poster.workPosition.node;
                    break;

                case "posterhome":
                    if (CasePanelController.Instance.activeCase?.job?.poster?.home != null)
                        newLocation = CasePanelController.Instance.activeCase.job.poster.home.GetDestinationNode();
                    break;

                case "purpwork":
                    if (CasePanelController.Instance.activeCase?.job?.purp?.workPosition?.node != null)
                        newLocation = CasePanelController.Instance.activeCase.job.purp.workPosition.node;
                    break;

                case "purphome":
                    if (CasePanelController.Instance.activeCase?.job?.purp?.home != null)
                        newLocation = CasePanelController.Instance.activeCase.job.purp.home.GetDestinationNode();
                    break;

                case "swork":
                    if (storedHuman?.workPosition?.node != null)
                        newLocation = storedHuman.workPosition.node;
                    break;

                case "shome":
                    if (storedHuman?.home != null)
                        newLocation = storedHuman.home.GetDestinationNode();
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech($"Unknown preset '{args[0]}'. Available presets: home, mwork, mhome, vwork, vhome, posterwork, posterhome, purpwork, purphome, swork, shome.", 2, true);
                    return;
            }

            if (newLocation != null)
            {
                player.Teleport(newLocation, null);
                Lib.GameMessage.ShowPlayerSpeech($"Teleported to {args[0]}", 2, true);
            }
            else
            {
                Lib.GameMessage.ShowPlayerSpeech($"Failed to teleport. The preset '{args[0]}' has no set destination.", 2, true);
            }
        }

        [Command("/tpn")]
        private static void TeleportNodeCommand(string[] args)
        {
            if (args.Length < 3)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /tpn <x> <y> <z>", 2, true);
                return;
            }

            if (!int.TryParse(args[0], out int x) ||
                !int.TryParse(args[1], out int y) ||
                !int.TryParse(args[2], out int z))
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid coordinates. Please provide valid numbers.", 2, true);
                return;
            }

            Vector3Int newLocation = new Vector3Int(x, y, z);

            playerInfoProvider.SetPlayerNode(newLocation);

            Lib.GameMessage.ShowPlayerSpeech($"Teleported to {newLocation}", 2, true);
        }

        [Command("/output")]
        private static void ExportCommand(string[] args)
        {
            if (args.Length < 1)
            {
                Lib.GameMessage.ShowPlayerSpeech("Invalid usage. Use: /export <murderer|victim|poster|purp|stored>", 2, true);
                return;
            }

            string characterType = args[0].ToLower();
            string output = string.Empty;

            switch (characterType)
            {
                case "murderer":
                    if (murderController.currentMurderer != null)
                        output = GetCitizenInfo(murderController.currentMurderer);
                    else
                        Lib.GameMessage.ShowPlayerSpeech("No murderer data found.", 2, true);
                    break;

                case "victim":
                    if (murderController.currentVictim != null)
                        output = GetCitizenInfo(murderController.currentVictim);
                    else
                        Lib.GameMessage.ShowPlayerSpeech("No victim data found.", 2, true);
                    break;

                case "poster":
                    if (CasePanelController.Instance.activeCase?.job?.poster != null)
                        output = GetCitizenInfo(CasePanelController.Instance.activeCase.job.poster);
                    else
                        Lib.GameMessage.ShowPlayerSpeech("No poster data found.", 2, true);
                    break;

                case "purp":
                    if (CasePanelController.Instance.activeCase?.job?.purp != null)
                        output = GetCitizenInfo(CasePanelController.Instance.activeCase.job.purp);
                    else
                        Lib.GameMessage.ShowPlayerSpeech("No purp data found.", 2, true);
                    break;

                case "stored":
                    if (storedHuman != null)
                        output = GetCitizenInfo(storedHuman);
                    else
                        Lib.GameMessage.ShowPlayerSpeech("No stored data found.", 2, true);
                    break;

                default:
                    Lib.GameMessage.ShowPlayerSpeech("Unknown character type. Use one of: murderer, victim, poster, purp, stored.", 2, true);
                    return;
            }

            if (!string.IsNullOrEmpty(output))
            {
                string directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string filePath = Path.Combine(directoryPath, $"{characterType}_info.txt");

                try
                {
                    File.WriteAllText(filePath, output);
                    Lib.GameMessage.ShowPlayerSpeech($"Exported {characterType} info to {filePath}", 2, true);
                }
                catch (Exception ex)
                {
                    Lib.GameMessage.ShowPlayerSpeech($"Error exporting {characterType} info: {ex.Message}", 2, true);
                }
            }
        }

        private static string GetCitizenInfo(Human human)
        {
            string name = human.name?.ToString() ?? "null";
            string age = human.GetAge().ToString() ?? "null";
            string employer = human.job?.employer?.name?.ToString() ?? "null";
            string occupation = human.job?.name?.ToString() ?? "null";
            string address = human.home?.thisAsAddress?.name?.ToString() ?? "null";

            string passcode = human.passcode?.digits != null
                ? string.Join("", human.passcode.digits.Select(digit => digit.ToString() ?? "null"))
                : "null";

            string inventoryItems = human.inventory != null
                ? string.Join(", ", human.inventory.Select(item => item?.GetName()?.ToString() ?? "null"))
                : "null";

            return $"Name: {name}\n" +
                   $"Age: {age}\n" +
                   $"Employer: {employer}\n" +
                   $"Occupation: {occupation}\n" +
                   $"Address: {address}\n" +
                   $"Passcode: {passcode}\n" +
                   $"Inventory: {inventoryItems}\n";
        }
    }
}

