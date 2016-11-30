using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Language
{
    public Dictionary<string, string> texts;

    private Language()
    {
        string path;
        if (Application.systemLanguage == SystemLanguage.French)
            path = Paths.FRENCH_LOC;
        else
            path = Paths.ENGLISH_LOC;

        TextAsset textAsset = Resources.Load<TextAsset>(path);

        ParseLocalisationFile(textAsset);
    }


    private static Language instance;
    public static Language Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Language();
            }
            return instance;
        }
    }

    private void ParseLocalisationFile(TextAsset text)
    {
        texts = new Dictionary<string, string>();

        string[] KeysAndValues = text.text.Split('"');

        //The last element of KeysAndValues is an empty string
        for (int i = 0; i < KeysAndValues.Length-1; i += 2)
        {
            string key = KeysAndValues[i].TrimEnd().TrimStart();
            string value = KeysAndValues[i+1].Trim();
            texts.Add(key, value);
        }
    }
}


