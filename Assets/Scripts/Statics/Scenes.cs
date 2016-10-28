using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes
{

    public const string Main = "Main";
    public const string Lobby = "Lobby";

    public const short LobbyIndex = 0;
    public const short MainIndex = 1;


    public static bool IsCurrentScene(int sceneBuildIndex)
    {
        return SceneManager.GetActiveScene().buildIndex == sceneBuildIndex;
    }

    public static short currentSceneId
    {
        get
        {
            return (short)SceneManager.GetActiveScene().buildIndex;
        }
    }
}
