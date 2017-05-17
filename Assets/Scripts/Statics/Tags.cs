using System;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{

    public const string Ground = "Ground";
    public const string Ball = "Ball";
    public const string Target = "Target";
    public const string Wall = "Wall";
    public const string Player = "Player";
    public const string CatchDetector = "CatchDetector";
    public const string Spawns = "Spawns";
    public const string Scoreboard = "Scoreboard";
    public const string MyPlayer = "MyPlayer";
    public const string UI = "UI";
    public const string NetworkScripts = "NetworkScripts";
    public const string Room = "Room";
    public const string Scene = "Scene";
    public const string RoomScripts = "RoomScripts";
    public const string Mesh = "Mesh";
    public const string TeamColored = "TeamColored";
    public const string Abilities = "Abilities";
    public const string MainCamera = "MainCamera";

    public const string LobbyManager = "LobbyManager";
    public const string NavigationManager = "NavigationManager";
    public const string Countdown = "Countdown";
    public const string GoalZone = "GoalZone";
    public const string Tutorial = "Tutorial";
    public const string PopUp = "PopUp";
    public const string VictoryPose = "VictoryPose";
    public const string VictoryUI = "VictoryUI";
    public static string GlobalSound = "GlobalSound";
    public static string BattleriteCamera = "BattleriteCamera";
    public static string GameplaySettings = "GameplaySettings";

    public static bool IsPlayer(string tag)
    {
        return tag == Player || tag == MyPlayer;
    }
}
