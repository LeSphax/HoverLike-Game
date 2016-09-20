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

    public static bool IsPlayer(string tag)
    {
        return tag == Player || tag == MyPlayer;
    }
}
