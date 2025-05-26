using Maze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable CA1861 // Avoid constant arrays as arguments
#pragma warning disable IDE1006 // Naming Styles

namespace Maze
{
    /// <summary>
    /// Class for each entry in the config dictionary
    /// </summary>
    public class ConfigEntry
    {
        public object value { get; set; }

        public bool read_only { get; }
        public string description { get; }

        public ConfigEntry(object val, bool read_only = false, string desc = "")
        {
            this.value = val;
            this.read_only = read_only;
            this.description = desc;
        }
    }

    public static class ConfigData
    {
        private static Dictionary<string, ConfigEntry> configValues;

        /// <summary>
        /// Static constructor where the default variable and their values are saved
        /// </summary>
        static ConfigData()
        {
            configValues = new Dictionary<string, ConfigEntry>
            {
                { "delay", new ConfigEntry(0) },
                { "doRandomizeBorders", new ConfigEntry(false) },
                { "coloredOutput", new ConfigEntry(false) },
                { "showProgress", new ConfigEntry(false) },
                { "measureSpeed", new ConfigEntry(true) },
                { "wallSymbol", new ConfigEntry('#') },
                { "blankSymbol", new ConfigEntry(' ') },
                { "playerSymbol", new ConfigEntry('X') },
                { "outputColors", new ConfigEntry(new ConsoleColor[] { ConsoleColor.Black, ConsoleColor.DarkGray, ConsoleColor.Blue }) },
                { "borderRandomizationChance", new ConfigEntry(new int[] { 1, 8 }) },
                { "gameMap", new ConfigEntry(new string[115, 115], true) }, // Safe limit is 115 x 115, larger sizes might cause stack overflow
                { "preloadGameMap", new ConfigEntry(new string[5, 5], true) },
                { "startCoords", new ConfigEntry(new int[] {1, 1}) }
            };
        }

        /// <summary>
        /// Gets all the variables and their values from the config
        /// </summary>
        public static Dictionary<string, ConfigEntry> GetVariables()
        {
            return configValues;
        }

        /// <summary>
        /// Method to retrieve a value in the config from a specific key
        /// </summary>
        public static TValue GetValue<TValue>(string key)
        {
            if (configValues.ContainsKey(key))
            {
                var entry = configValues[key];
                if (entry.value is TValue tValue)
                {
                    return tValue;
                }
                else
                {
                    throw new InvalidCastException($"Value for {key} is not of type {typeof(TValue).Name}");
                }
            }
            else
            {
                throw new ArgumentNullException($"{key} was not found in config");
            }
        }

        /// <summary>
        /// Method to set a value for a key in the config
        /// </summary>
        public static void SetValue(string key, object value)
        {
            if (configValues.ContainsKey(key))
            {
                Console.WriteLine(value.ToString());
                var entry = configValues[key];
                if (!entry.read_only)
                {
                    entry.value = value;
                }
                else
                {
                    Console.WriteLine($"{key}' is read-only, cannot be modified");
                }
            }
            else
            {
                Console.WriteLine(key + " was not found in the config.");
            }
        }
    }
}
