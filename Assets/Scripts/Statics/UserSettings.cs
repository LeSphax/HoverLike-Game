using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UserSettings
{
    private static bool? isFirstGame = null;
    public static bool IsFirstGame
    {
        get
        {
            if (isFirstGame == null)
            {
                int value = PlayerPrefs.GetInt("IsFirstGame", 1);
                isFirstGame = value == 1;
                PlayerPrefs.SetInt("IsFirstGame", 0);
                PlayerPrefs.Save();
            }
            return isFirstGame.Value;
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
            nickname = value;
            PlayerPrefs.SetString("Nickname", nickname);
            PlayerPrefs.Save();
            if (Players.MyPlayer != null)
            Players.MyPlayer.Nickname = nickname;
        }
    }

    public static string keys = null;

    public static char GetKey(int number)
    {
        if (keys == null)
        {
            string defaultKeys = "QWER ";
            if (Application.systemLanguage == SystemLanguage.French)
            {
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
