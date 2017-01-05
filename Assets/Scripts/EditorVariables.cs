using UnityEngine;
using System.Collections;

public class EditorVariables : MonoBehaviour
{

    private const string HEROKU_URL = "ws://sphaxtest.herokuapp.com";
    private const string BCS_URL = "wss://because-why-not.com:12777/chatapp";

    private const string LOCALHOST_URL = "ws://localhost:5000";

    public enum Server
    {
        LOCALHOST,
        HEROKU,
        BCS,
    }

    public Server server;
    private static Server serverURL;
    public static string ServerURL
    {
        get
        {
            switch (serverURL)
            {
                case Server.LOCALHOST:
                    return LOCALHOST_URL;
                case Server.HEROKU:
                    return HEROKU_URL;
                case Server.BCS:
                    return BCS_URL;
                default:
                    throw new UnhandledSwitchCaseException(serverURL);
            }
        }
    }

    public bool testURLParameters;
    public static bool TestURLParameters;

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
#if UNITY_WEBGL && !UNITY_EDITOR
        StartGameImmediately = false;
        NumberFramesLatency = 0;
        NumberPlayersToStartGame = 0;
        AddLatency = false;
        testURLParameters = false;
        serverURL = Server.HEROKU;
#else
        StartGameImmediately = startGameImmediately;
        NumberFramesLatency = numberFramesLatency;
        NumberPlayersToStartGame = numberPlayersToStartGame;
        AddLatency = addLatency;
        TestURLParameters = testURLParameters;
        serverURL = server;
#endif
    }
}
