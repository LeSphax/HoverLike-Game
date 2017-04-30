using UnityEngine;
using System.Collections;

public class EditorVariables : MonoBehaviour
{

    private const string HEROKU_URL = "ws://slideball-signaling.herokuapp.com/";
    private const string HEROKU_TEST_URL = "ws://slideball-signaling-test.herokuapp.com/";
    private const string BCS_URL = "wss://because-why-not.com:12777/chatapp";

    private const string LOCALHOST_URL = "ws://localhost:5000";

    public enum Server
    {
        LOCALHOST,
        HEROKU,
        HEROKU_TEST,
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
                case Server.HEROKU_TEST:
                    return HEROKU_TEST_URL;
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

    public bool editorIsServer;
    public static bool EditorIsServer;

    public int numberPlayersToStartGame;
    public static int NumberPlayersToStartGame;

    public bool addLatency;
    public static bool AddLatency;

    public int numberFramesLatency;
    public static int NumberFramesLatency;
    internal static bool CanScoreGoals;

    public bool playAsGoalieInitialValue;
    internal static bool PlayAsGoalieInitialValue;

    public bool noCooldowns;
    internal static bool NoCooldowns;

    public float editorFloat;
    public static float EditorFloat;

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        StartGameImmediately = false;
        EditorIsServer = false;
        NumberFramesLatency = 0;
        NumberPlayersToStartGame = 0;
        AddLatency = false;
        testURLParameters = false;
        serverURL = Server.HEROKU;
        playAsGoalieInitialValue = false;
        NoCooldowns = false;
#else
        StartGameImmediately = startGameImmediately;
        EditorIsServer = editorIsServer;
        NumberFramesLatency = numberFramesLatency;
        NumberPlayersToStartGame = numberPlayersToStartGame;
        AddLatency = addLatency;
        TestURLParameters = testURLParameters;
        serverURL = server;
        PlayAsGoalieInitialValue = playAsGoalieInitialValue;
        NoCooldowns = noCooldowns;
        EditorFloat = editorFloat;
#endif
        CanScoreGoals = true;
    }
}
