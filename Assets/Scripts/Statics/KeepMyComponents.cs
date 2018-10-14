using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyComponents))]
public class KeepMyComponents : MonoBehaviour {

    private static MyComponents instance;

    private void Awake()
    {
        MyComponents myComponents = GetComponent<MyComponents>();
        if (instance != null)
        {
            myComponents = instance;
        }
        else if (Scenes.IsCurrentScene(Scenes.LobbyIndex))
        {
            instance = myComponents;
        }
    }
}
