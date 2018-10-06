using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

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
}
