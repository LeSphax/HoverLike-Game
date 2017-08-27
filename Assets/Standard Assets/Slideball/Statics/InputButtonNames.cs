using System;
using UnityEngine;

public static class SlideBallInputs
{
    public static GUIPart currentPart;

    public enum GUIPart
    {
        ABILITY,
        CHAT,
        MENU,
    }

    public const string RETURN = "Return";

    public static bool AnyEnterDown()
    {
        return Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return);
    }

    public static bool GetKey(KeyCode keyCode, GUIPart part)
    {
        return Input.GetKey(keyCode) && part == currentPart;
    }

    public static bool GetKeyDown(KeyCode keycode,GUIPart part)
    {
        return Input.GetKeyDown(keycode) && part == currentPart;
    }

    public static bool GetKeyUp(KeyCode keycode, GUIPart part)
    {
        return Input.GetKeyUp(keycode) && part == currentPart;
    }
}
