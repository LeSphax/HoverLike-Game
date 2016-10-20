using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Settings
{
    private const string RUNTIME_PATH = "Settings";
    private const string EDITOR_PATH = "Assets/Resources/Settings.txt";

    public const string NEXT_VIEW_ID = "NextViewId";

    private static Dictionary<string, string> _data;
    public static Dictionary<string, string> Data
    {
        get
        {
            if (_data == null)
            {
                TextAsset settings = (TextAsset)Resources.Load(RUNTIME_PATH);
                _data = ParseSettings(settings.text);
            }
            return _data;
        }
    }

    public static short GetNextViewId()
    {
        return Int16.Parse(Data["NextViewId"]);
    }

    public static void SaveSettings()
    {
        StringWriter writer = new StringWriter();
        foreach(var pair in Data)
        {
            writer.WriteLine(pair.Key + " : " + pair.Value);
        }
        File.WriteAllText(EDITOR_PATH, writer.ToString());
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
            string value = line.Substring(separatorIndex+1).Trim();
            result.Add(key, value);
        }
        return result;
    }






}
