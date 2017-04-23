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

    internal static bool GetKey(KeyCode keyCode, GUIPart part)
    {
        return Input.GetKey(keyCode) && part == currentPart;
    }

    internal static bool GetKeyDown(KeyCode keycode,GUIPart part)
    {
        return Input.GetKeyDown(keycode) && part == currentPart;
    }

    public static bool AnyShiftDown()
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }
    public static bool AnyShiftUp()
    {
        return Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift);
    }

    public static bool AnyShift()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
}
