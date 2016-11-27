using System;
using System.Collections.Generic;
using UnityEngine;

public class Inputs
{
    public static string[] Keys = new string[5] { "Q", "W", "E", "R", " " };

    public static string GetKeyForIcon(int number)
    {
        string value = Keys[number];
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
        string value = Keys[number];
        if (value == " ")
        {
            return KeyCode.Space;
        }
        else
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), Keys[number]);
        }
    }
}
