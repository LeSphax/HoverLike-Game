using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public static class Settings
{
    private const string SETTINGS_PATH = "Settings/";
    private const string EDITOR_PATH = "Assets/Resources/";

    public const string NETWORK_SETTINGS = "NetworkSettings";
    public const string NEXT_VIEW_ID = "NextViewId";

    private static Dictionary<string, Dictionary<string, string>> _data = new Dictionary<string, Dictionary<string, string>>();
    public static Dictionary<string, string> GetSettings(string fileName)
    {
        Dictionary<string, string> fileSettings;
        if (!_data.TryGetValue(fileName, out fileSettings))
        {
            TextAsset settings = Resources.Load<TextAsset>(SETTINGS_PATH + fileName);
            Assert.IsTrue(settings != null, "This settings file doesn't exist " + fileName);
            fileSettings = ParseSettings(settings.text);
            _data.Add(fileName, fileSettings);
        }
        return fileSettings;

    }

    public static short GetNextViewId()
    {
        return short.Parse(GetSettings(NETWORK_SETTINGS)["NextViewId"]);
    }

    public static void SaveSettings(string fileName, Dictionary<string, string> settings)
    {
        StringWriter writer = new StringWriter();
        foreach (var __pair in settings)
        {
            writer.WriteLine(__pair.Key + " : " + __pair.Value);
        }
        Debug.Log("Save Settings " + EDITOR_PATH + SETTINGS_PATH + fileName + ".txt");
        File.WriteAllText(EDITOR_PATH + SETTINGS_PATH + fileName + ".txt", writer.ToString());

    }


    private static Dictionary<string, string> ParseSettings(string settings)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        StringReader reader = new StringReader(settings);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            int separatorIndex = line.IndexOf(':');
            string key = line.Substring(0, separatorIndex).Trim();
            string value = line.Substring(separatorIndex + 1).Trim();
            result.Add(key, value);
        }
        return result;
    }

    internal static string[] ParseTable(string v)
    {
        string[] elements = v.Split('|');
        return elements;
    }
}
