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
    public struct ConfigData
    {
        private Dictionary<string, object> configValues;

        public ConfigData()
        {
            configValues = new Dictionary<string, object>
            {
                { "delay", 0 },
                { "doRandomizeBorders", false },
                { "coloredOutput", false },
                { "showProgress", false },
                { "isPreloading", false },
                { "measureSpeed", false },
                { "wallSymbol", '#' },
                { "blankSymbol", ' ' },
                { "playerSymbol", 'X' },
                { "outputColors", new ConsoleColor[] { ConsoleColor.Black, ConsoleColor.DarkGray, ConsoleColor.Blue } },
                { "borderRandomizationChance", new int[] { 1, 8 } },
                { "gameMap", new string[21, 39] },
                { "preloadGameMap", new string[5, 5] },
                { "startCoords", new int[] { 1, 1 } }
            };
        }

        public readonly Dictionary<string, object> GetVariables()
        {
            return configValues;
        }

        public object? GetValue(string key)
        {
            if (configValues.ContainsKey(key))
            {
                configValues.TryGetValue(key, out var value);
                return value;
            }
            else
            {
                Console.WriteLine(key + " was not found in the config.");
                return null;
            }
        }

        public readonly void SetValue(string key, object value)
        {
            if (configValues.ContainsKey(key))
            {
                configValues[key] = value;
            }
            else
            {
                Console.WriteLine(key + " was not found in the config.");
            }
        }
    }
}
