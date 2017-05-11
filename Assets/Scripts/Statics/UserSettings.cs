using PlayerManagement;
using System;
using UnityEngine;

public class UserSettings
{
    private static bool? seenTutorial = null;
    public static bool SeenTutorial
    {
        get
        {
            if (seenTutorial == null)
            {
                int value = PlayerPrefs.GetInt("SeenTutorial", 1);
                seenTutorial = value == 1;
                PlayerPrefs.Save();
            }
            return seenTutorial.Value;
        }
        set
        {
            seenTutorial = value;
            PlayerPrefs.SetInt("SeenTutorial", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private static string nickname = null;
    public static string Nickname
    {
        get
        {
            if (nickname == null)
            {
                nickname = PlayerPrefs.GetString("Nickname", "");
            }
            Debug.Log("Nickname is " + nickname);
            return nickname;
        }
        set
        {
            Debug.Log("Nickname set to " + value);
            nickname = value;
            PlayerPrefs.SetString("Nickname", nickname);
            PlayerPrefs.Save();
            if (Players.MyPlayer != null)
                Players.MyPlayer.Nickname = nickname;
        }
    }

    private static bool volumeSet = false;
    public static float Volume
    {
        get
        {
            if (!volumeSet)
            {
                volumeSet = true;
                AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1);
            }

            return AudioListener.volume;
        }
        set
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat("Volume", AudioListener.volume);
            PlayerPrefs.Save();
        }
    }

    public static string keys = null;

    public static char GetKey(int number)
    {
        if (keys == null)
        {
            string defaultKeys = "QWER ";
            Debug.Log("---------LANGUAGE " + Application.systemLanguage);
            if (Application.systemLanguage == SystemLanguage.French)
            {
                Debug.Log("Language is French !");
                defaultKeys = "AZER ";
            }
            keys = PlayerPrefs.GetString("Keys", defaultKeys);
        }
        return keys[number];
    }

    public static void SetKeys(string newKeys)
    {
        keys = newKeys;
        PlayerPrefs.SetString("Keys", keys);
        PlayerPrefs.Save();
    }

    public static string GetKeyForIcon(int number)
    {
        string value = GetKey(number).ToString();
        if (value == " ")
        {
            return "SPC";
        }
        else
        {
            return value;
        }
    }

    public static KeyCode GetKeyCode(int number)
    {
        string value = GetKey(number).ToString();
        if (value == " ")
        {
            return KeyCode.Space;
        }
        else
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), value);
        }
    }
}
