using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SOD.Common;
using System.Text;

namespace DevTools
{
    public class ConfigManager
    {
        // Configuration model
        public class DevToolsConfig
        {
            public bool God { get; set; } = false;
            public bool DisableBadEffects { get; set; } = false;
            public bool AllowEverywhere { get; set; } = false;
            public List<string> CustomCommands { get; set; } = new List<string>();
        }

        // Get the plugin directory path
        public static string GetPluginDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        // Get the full path to a config file
        public static string GetConfigFilePath(string configName)
        {
            // Ensure the config name doesn't contain path traversal
            if (configName.Contains("..") || configName.Contains("/") || configName.Contains("\\"))
            {
                throw new ArgumentException("Invalid config name");
            }

            // Sanitize the config name to prevent directory traversal
            configName = Path.GetFileNameWithoutExtension(configName);
            
            // Add the .dt.txt extension
            return Path.Combine(GetPluginDirectory(), $"{configName}.dt.txt");
        }

        // Load a configuration file
        public static DevToolsConfig LoadConfig(string configName)
        {
            string configPath = GetConfigFilePath(configName);
            
            if (!File.Exists(configPath))
            {
                DevTools.Logger.LogWarning($"Config file not found: {configPath}");
                return null;
            }

            try
            {
                string[] lines = File.ReadAllLines(configPath);
                DevToolsConfig config = new DevToolsConfig();
                
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    
                    // Skip comments and empty lines
                    if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
                        continue;
                        
                    // Parse key-value pairs
                    if (trimmedLine.Contains("="))
                    {
                        string[] parts = trimmedLine.Split(new char[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                             
                            // Parse the different config options
                            if (key.Equals("God", StringComparison.OrdinalIgnoreCase))
                            {
                                bool godValue;
                                if (bool.TryParse(value, out godValue))
                                    config.God = godValue;
                            }
                            else if (key.Equals("DisableBadEffects", StringComparison.OrdinalIgnoreCase))
                            {
                                bool effectsValue;
                                if (bool.TryParse(value, out effectsValue))
                                    config.DisableBadEffects = effectsValue;
                            }
                            else if (key.Equals("AllowEverywhere", StringComparison.OrdinalIgnoreCase))
                            {
                                bool everywhereValue;
                                if (bool.TryParse(value, out everywhereValue))
                                    config.AllowEverywhere = everywhereValue;
                            }
                            else if (key.Equals("Command", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    config.CustomCommands.Add(value);
                                }
                            }
                        }
                    }
                }
                
                DevTools.Logger.LogInfo($"Loaded config from {configPath}");
                return config;
            }
            catch (Exception ex)
            {
                DevTools.Logger.LogError($"Failed to load config: {ex.Message}");
                return null;
            }
        }

        // Save a configuration file
        public static bool SaveConfig(string configName, DevToolsConfig config)
        {
            string configPath = GetConfigFilePath(configName);
            
            try
            {
                StringBuilder sb = new StringBuilder();
                
                // Add a header comment
                sb.AppendLine("// DevTools configuration file");
                sb.AppendLine("// Format: key = value");
                sb.AppendLine();
                
                // Add the boolean settings
                sb.AppendLine($"God = {config.God}");
                sb.AppendLine($"DisableBadEffects = {config.DisableBadEffects}");
                sb.AppendLine($"AllowEverywhere = {config.AllowEverywhere}");
                sb.AppendLine();
                
                // Add custom commands
                sb.AppendLine("// Custom commands to execute");
                foreach (string command in config.CustomCommands)
                {
                    sb.AppendLine($"Command = {command}");
                }
                
                File.WriteAllText(configPath, sb.ToString());
                DevTools.Logger.LogInfo($"Saved config to {configPath}");
                return true;
            }
            catch (Exception ex)
            {
                DevTools.Logger.LogError($"Failed to save config: {ex.Message}");
                return false;
            }
        }

        // Execute a configuration
        public static void ExecuteConfig(string configName)
        {
            DevToolsConfig config = LoadConfig(configName);
            if (config == null)
            {
                SOD.Common.Lib.GameMessage.ShowPlayerSpeech($"Config '{configName}' not found or invalid", 3, true);
                return;
            }

            // Execute built-in toggles
            if (config.God)
            {
                CommandManager.ExecuteCommand("/god");
            }
            
            if (config.DisableBadEffects)
            {
                CommandManager.ExecuteCommand("/disableBadEffects");
            }
            
            if (config.AllowEverywhere)
            {
                CommandManager.ExecuteCommand("/allowEverywhere");
            }

            // Execute custom commands
            foreach (string command in config.CustomCommands)
            {
                if (!string.IsNullOrEmpty(command))
                {
                    CommandManager.ExecuteCommand(command);
                }
            }

            SOD.Common.Lib.GameMessage.ShowPlayerSpeech($"Executed config '{configName}'", 3, true);
        }

        // Create a default configuration file if it doesn't exist
        public static void CreateDefaultConfig()
        {
            string configPath = GetConfigFilePath("config");
            
            if (!File.Exists(configPath))
            {
                DevToolsConfig defaultConfig = new DevToolsConfig
                {
                    God = false,
                    DisableBadEffects = true,
                    AllowEverywhere = false,
                    CustomCommands = new List<string>
                    {
                        "/noclip",
                        "/speed 2"
                    }
                };

                SaveConfig("config", defaultConfig);
                DevTools.Logger.LogInfo("Created default config file");
            }
        }
    }
}
