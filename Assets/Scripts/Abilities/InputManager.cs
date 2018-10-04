using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool GetKeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    public bool GetKey(KeyCode key)
    {
        return Input.GetKey(key);
    }

    public bool GetKeyUp(KeyCode key)
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
