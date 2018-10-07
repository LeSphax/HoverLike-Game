using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public GUIPart currentPart;

    public const string RETURN = "Return";

    public static bool AnyEnterDown()
    {
        return Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return);
    }

    public static bool AnyShift()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    public bool GetKey(string keyCode, GUIPart part)
    {
        return GetKey(keyCode) && part == currentPart;
    }

    public bool GetKeyDown(string keycode, GUIPart part)
    {
        return GetKeyDown(keycode) && part == currentPart;
    }

    public bool GetKeyUp(string keycode, GUIPart part)
    {
        return GetKeyUp(keycode) && part == currentPart;
    }

    private void Update()
    {
        keysPressed.Clear();
    }

    List<KeyCode> keysPressed = new List<KeyCode>();

    public void SetKey(KeyCode key)
    {
        keysPressed.Add(key);
    }


    public bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    public bool GetKeyDown(string key)
    {
        return Input.GetKeyDown(key);
    }

    public bool GetKey(KeyCode key)
    {
        return Input.GetKey(key) || keysPressed.Contains(key);
    }

    public bool GetKey(string key)
    {
        KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), key);
        return GetKey(keyCode);
    }

    public bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    public bool GetKeyUp(string key)
    {
        return Input.GetKeyUp(key);
    }

    public bool GetMouseButtonDown(int button)
    {
        return Input.GetMouseButtonDown(button);
    }

    public bool GetMouseButton(int button)
    {
        return Input.GetMouseButton(button);
    }

    public bool GetMouseButtonUp(int button)
    {
        return Input.GetMouseButtonUp(button);
    }

    public Vector3 GetInputDirection()
    {
        KeyCode[] movementKeys = UserSettings.MovementKeys;
        return new MovementInputPacket(
                    GetKey(movementKeys[0]),
                    GetKey(movementKeys[1]),
                    GetKey(movementKeys[2]),
                    GetKey(movementKeys[3])
                ).GetDirection();
    }

}
