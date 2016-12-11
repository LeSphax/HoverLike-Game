using UnityEngine;
using System.Collections;

public class EditorVariables : MonoBehaviour
{

    public bool startGameImmediately;
    public static bool StartGameImmediately;

    public int numberPlayersToStartGame;
    public static int NumberPlayersToStartGame;

    public bool addLatency;
    public static bool AddLatency;

    public int numberFramesLatency;
    public static int NumberFramesLatency;

    private void Awake()
    {
#if UNITY_WEBGL
        StartGameImmediately = false;
        NumberFramesLatency = 0;
        NumberPlayersToStartGame = 0;
        AddLatency = false;
#else
        StartGameImmediately = startGameImmediately;
        NumberFramesLatency = numberFramesLatency;
        NumberPlayersToStartGame = numberPlayersToStartGame;
        AddLatency = addLatency;
#endif
    }
}
