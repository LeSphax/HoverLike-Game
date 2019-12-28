using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SlideBall.MonoBehaviour
{
    List<string> keysUp = new List<string>();
    List<string> keysDown = new List<string>();
    List<KeyCode> keysPressed = new List<KeyCode>();
    readonly List<int> mouseButtonsDown = new List<int>();
    readonly List<int> mouseButtonsUp = new List<int>();
    Vector3? mouseLocalPosition = null;

    public static GUIPart currentPart;

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

    private void FixedUpdate()
    {
        keysUp.Clear();
        keysDown.Clear();
        keysPressed.Clear();
        mouseButtonsDown.Clear();
        mouseButtonsUp.Clear();
    }

    public void SetKey(KeyCode key)
    {
        keysPressed.Add(key);
    }

    public void SetKeyDown(string key)
    {
        keysDown.Add(key);
    }

    
    public void SetKeyUp(string key)
    {
        keysUp.Add(key);
    }


    public bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    public bool GetKeyDown(string key)
    {
        return Input.GetKeyDown(key) || keysDown.Contains(key);
    }

    public bool GetKey(KeyCode key)
    {
        return Input.GetKey(key) || keysPressed.Contains(key);
    }

    public bool GetKey(string key)
    {
        KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), key.ToUpper());
        return GetKey(keyCode);
    }

    public bool GetKeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }

    public bool GetKeyUp(string key)
    {
        return Input.GetKeyUp(key) || keysUp.Contains(key);
    }

    public void SetMouseButtonDown(int button)
    {
        mouseButtonsDown.Add(button);
    }

    public bool GetMouseButtonDown(int button)
    {
        return mouseButtonsDown.Contains(button) || Input.GetMouseButtonDown(button);
    }

    public bool GetMouseButton(int button)
    {
        return Input.GetMouseButton(button);
    }

    public void SetMouseButtonUp(int button)
    {
        mouseButtonsUp.Add(button);
    }

    public bool GetMouseButtonUp(int button)
    {
        return mouseButtonsUp.Contains(button) || Input.GetMouseButtonUp(button);
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

    public void SetMouseLocalPosition(Vector3 mouseLocalPosition)
    {
        this.mouseLocalPosition = mouseLocalPosition;
    }

    public Vector3 GetMouseLocalPosition()
    {
        if (mouseLocalPosition != null)
        {
            return mouseLocalPosition.Value;
        }
        return MyComponents.transform.InverseTransformPoint(Functions.GetMouseWorldPosition());
    }

    public Vector3 GetMouseWorldPosition()
    {
        if (mouseLocalPosition != null)
        {
            return MyComponents.transform.TransformPoint(mouseLocalPosition.Value);
        }
        return Functions.GetMouseWorldPosition();
    }

}
