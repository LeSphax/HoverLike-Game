using PlayerManagement;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class UserSettings
{
    public static KeyCode[] MovementKeys
    {
        get
        {
            if (KeyForInputCheck(0) == "a")
            {
                return new KeyCode[] {
                        KeyCode.Z,
                        KeyCode.S,
                        KeyCode.Q,
                        KeyCode.D
                };
            }
            else
            {
                return new KeyCode[] {
                        KeyCode.W,
                        KeyCode.S,
                        KeyCode.A,
                        KeyCode.D
                };
            }
        }
    }

    private static bool? seenTutorial = null;
    public static bool SeenTutorial
    {
        get
        {
            if (seenTutorial == null)
            {
                int value = PlayerPrefs.GetInt("SeenTutorial", 0);
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
            return nickname;
        }
        set
        {
            Debug.Log("Nickname set to " + value);
            nickname = value;
            PlayerPrefs.SetString("Nickname", nickname);
            PlayerPrefs.Save();
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

    private static string keys = null;
    private static string Keys
    {
        get
        {
            return keys;
        }
        set
        {
            keys = value;
            KeysArray = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                KeysArray[i] = keys[i] == ' ' ? "space" : keys[i].ToString();
            }
        }
    }
    private static string[] KeysArray = null;

    public static string KeyForInputCheck(int number)
    {
        InitKeysIfNecessary();
        return KeysArray[number];
    }

    private static void InitKeysIfNecessary()
    {
        if (Keys == null)
        {
            string defaultKeys = GetDefaultKeys();
            string savedKeys = GetDefaultKeys();// PlayerPrefs.GetString("Keys", defaultKeys);
            if (savedKeys.Length != defaultKeys.Length)
            {
                Keys = defaultKeys;
            } else{
                Keys = savedKeys;
            }
        }
    }

    private static string GetDefaultKeys()
    {
        string defaultKeys = "qefr cv";
        if (Application.systemLanguage == SystemLanguage.French)
        {
            Debug.Log("Language is French !");
            defaultKeys = "aefr cv";
        }

        return defaultKeys;
    }

    public static void SetKeys(string newKeys)
    {
        Keys = newKeys;
        PlayerPrefs.SetString("Keys", Keys);
        PlayerPrefs.Save();
    }

    public static string GetKeyForIcon(int number)
    {
        InitKeysIfNecessary();
        return CharToDisplayKey(Keys[number]);
    }

    public static KeyCode GetKeyCode(int number)
    {
        string value = KeyForInputCheck(number).ToString();
        if (value == " ")
        {
            return KeyCode.Space;
        }
        else
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), value);
        }
    }

    public static string CharToDisplayKey(char c)
    {
        return c == ' ' ? "SPC" : c.ToString().ToUpper();
    }

    public static char DisplayKeyToChar(string displayKey)
    {
        Assert.IsTrue(displayKey == "SPC" || displayKey.Length == 1);
        return displayKey == "SPC" ? ' ' : displayKey[0];
    }
}
