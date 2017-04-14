// C# example.
using SlideBall;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpecialBuild
{
    private const string path = "C:/Programmation/Workspace/UnityProjects/SlideBall/Builds/PC/Slideball.exe";
    private const string path_WebGL = "C:/Programmation/Workspace/UnityProjects/SlideBall/Builds/";

    private static string[] levels = new string[] { Paths.SCENE_LOBBY, Paths.SCENE_ROOM, Paths.SCENE_MAIN };

    private static float startWaitingTime = -1;

    private static List<System.Diagnostics.Process> processes = new List<System.Diagnostics.Process>();


    private static int numberOfGamesToLaunch;

    [MenuItem("MyTools/EditorAsServer %g")]
    public static void BuildWithLobby()
    {
        if (BuildOnly())
        {
            for (int i = 0; i < numberOfGamesToLaunch; i++)
                RunGame();

            ChangeScene(Paths.SCENE_LOBBY);
            EditorApplication.isPlaying = true;
        }
    }

    [MenuItem("MyTools/EditorAsClient %h")]
    public static void BuildWithoutLobby()
    {
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Enabled;
        if (BuildOnly())
        {
            ChangeScene(Paths.SCENE_LOBBY);
        }
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
    }

    public static bool BuildOnly()
    {
        return BuildGame(levels);
    }

    static void OnEditorUpdate()
    {
        if (Time.realtimeSinceStartup - startWaitingTime > 5)
        {
            Debug.Log("Go ! ");
            EditorApplication.isPlaying = true;
            EditorApplication.update -= OnEditorUpdate;
        }

    }

    private static bool BuildGame(string[] levels)
    {
        PrepareBuild();
        ChangeScene(Paths.SCENE_LOBBY);
        EditorVariables editorVariables = ((EditorVariables)UnityEngine.Object.FindObjectOfType(typeof(EditorVariables)));
        if (editorVariables.numberPlayersToStartGame < 2)
        {
            editorVariables.numberPlayersToStartGame = 2;
        }
        numberOfGamesToLaunch = editorVariables.numberPlayersToStartGame - 1;

        // Get filename.
        //string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

        string x = BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows64, BuildOptions.Development);
        if (x.Contains("cancelled") || x.Contains("error"))
        {
            Debug.LogError(x);
            ChangeScene(Paths.SCENE_LOBBY);
            return false;
        }
        Debug.Log(x);
        return true;

    }

    private static void PrepareBuild()
    {
        KillGames();
        EditorApplication.isPlaying = false;
        MakeViewIds();
    }

    [MenuItem("MyTools/Build WebGL %j")]
    public static void BuildWebGL()
    {
        PrepareBuild();
        ChangeScene(Paths.SCENE_LOBBY);
        BeforeBuildWebGl();

        //((NicknamePanel)UnityEngine.Object.FindObjectOfType(typeof(NicknamePanel))).autoNickName = false;
        string x = BuildPipeline.BuildPlayer(levels, path_WebGL, BuildTarget.WebGL, BuildOptions.None);
        Debug.Log(x);
    }

    public static void BeforeBuildWebGl()
    {
    }

    [MenuItem("MyTools/MakeViewIds %e")]
    public static void MakeViewIds()
    {
        Debug.Log("MakeViewIDS");
        short nextViewId = NetworkViewsManagement.INVALID_VIEW_ID + 1;
        foreach (string level in levels)
        {
            MakeViewIds(level, ref nextViewId);
        }

        var NetworkSettings = Settings.GetSettings(Settings.NETWORK_SETTINGS);
        if (!NetworkSettings.ContainsKey(Settings.NEXT_VIEW_ID))
            NetworkSettings.Add(Settings.NEXT_VIEW_ID, "" + nextViewId);
        else
            NetworkSettings[Settings.NEXT_VIEW_ID] = "" + nextViewId;
        Settings.SaveSettings(Settings.NETWORK_SETTINGS, NetworkSettings);
    }

    public static void MakeViewIds(string scene, ref short nextViewId)
    {
        ChangeScene(scene);
        foreach (ANetworkView view in GameObject.FindObjectsOfType(typeof(ANetworkView)))
        {

            view.ViewId = nextViewId;
            Debug.Log(view + "   " + view.ViewId);
            nextViewId++;
        }
        EditorSceneManager.SaveScene(EditorSceneManager.GetSceneByPath(scene));
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }



    [MenuItem("MyTools/RunGame %q")]
    private static void RunGame()
    {
        var proc = new System.Diagnostics.Process();
        processes.Add(proc);
        proc.StartInfo.FileName = path;
        proc.Start();
    }

    [MenuItem("MyTools/KillGames %k")]
    private static void KillGames()
    {
        foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("Slideball"))
        {
            process.Kill();
        }
    }


    private static void ChangeScene(string sceneName, bool tried = false)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            try
            {
                EditorSceneManager.SaveOpenScenes();// SaveScene();// SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(sceneName);
            }
            catch(InvalidOperationException)
            {
                if (!tried)
                {
                    ChangeScene(sceneName, true);
                }
            }
        }
    }
}
