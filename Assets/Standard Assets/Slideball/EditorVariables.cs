using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class EditorVariables : MonoBehaviour
{

    private const string HEROKU_URL = "ws://slideball-signaling.herokuapp.com/";
    private const string HEROKU_TEST_URL = "ws://slideball-signaling-test.herokuapp.com/";
    private const string BCS_URL = "wss://because-why-not.com:12777/chatapp";

    private const string LOCALHOST_URL = "ws://localhost:5000";

    [DllImport("__Internal")]
    private static extern string GetSignalingServerURL();

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
#if UNITY_WEBGL && !UNITY_EDITOR
            return GetSignalingServerURL();
#else
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
#endif
        }
    }

    public bool headlessServer;
    public static bool HeadlessServer;

    public bool testURLParameters;
    public static bool TestURLParameters;

    public bool joinRoomImmediately;
    public static bool JoinRoomImmediately;

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
    public static bool CanScoreGoals;

    public bool playAsGoalieInitialValue;
    public static bool PlayAsGoalieInitialValue;

    public bool noCooldowns;
    public static bool NoCooldowns;

    private void Awake()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        HeadlessServer = false;
        StartGameImmediately = false;
        EditorIsServer = false;
        NumberFramesLatency = 0;
        NumberPlayersToStartGame = 0;
        AddLatency = false;
        testURLParameters = false;
        //serverURL = Server.HEROKU;
        playAsGoalieInitialValue = false;
        NoCooldowns = false;
#else
        StartGameImmediately = startGameImmediately;
        JoinRoomImmediately = joinRoomImmediately;
        EditorIsServer = editorIsServer;
        NumberFramesLatency = numberFramesLatency;
        NumberPlayersToStartGame = numberPlayersToStartGame;
        AddLatency = addLatency;
        TestURLParameters = testURLParameters;
        serverURL = server;
        PlayAsGoalieInitialValue = playAsGoalieInitialValue;
        NoCooldowns = noCooldowns;
#if !UNITY_EDITOR
        string commandLineOptions = System.Environment.CommandLine;
        if (commandLineOptions.Contains("-servermode"))
        {
            Debug.LogError("Server mode!");
            HeadlessServer = true;
            JoinRoomImmediately = true;
            EditorIsServer = false;
            AddLatency = false;
            NoCooldowns = false;
            NumberFramesLatency = 0;
            StartGameImmediately = false;
            TestURLParameters = false;
        }
        else{
            HeadlessServer = false;
        }
        if (commandLineOptions.Contains("-nographics"))
        {
            Debug.Log("No Graphics Mode");
           // AudioListener.pause = true;
        }
#else
        HeadlessServer = headlessServer;
#endif
#endif
        CanScoreGoals = true;
    }
}
