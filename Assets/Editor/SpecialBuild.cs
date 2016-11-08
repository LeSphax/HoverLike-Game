// C# example.
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SpecialBuild
{
    private const string path = "C:/Programmation/Workspace/UnityProjects/Hover/Builds/PC/Build.exe";
    private const string path_WebGL = "C:/Programmation/Workspace/UnityProjects/Hover/Builds/";

    private static string[] levels = new string[] { Paths.SCENE_LOBBY, Paths.SCENE_MAIN };

    private static float startWaitingTime = -1;

    private static List<System.Diagnostics.Process> processes = new List<System.Diagnostics.Process>();

    [MenuItem("MyTools/EditorAsServer %g")]
    public static void BuildWithLobby()
    {
        if (BuildOnly())
        {
            RunGame();

            ChangeScene(Paths.SCENE_LOBBY);
            EditorApplication.isPlaying = true;
        }
    }

    [MenuItem("MyTools/EditorAsClient %h")]
    public static void BuildWithoutLobby()
    {
        //BuildGame(new string[] { Paths.SCENE_MAIN });
        if (BuildOnly())
        {
            RunGame();

            ChangeScene(Paths.SCENE_LOBBY);
        }
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

        // Get filename.
        //string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

        string x = BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows64, BuildOptions.Development);
        if (x.Contains("cancelled"))
        {
            Debug.LogError(x);
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

    [MenuItem("MyTools/Build Only %j")]
    public static void BuildWebGL()
    {
        PrepareBuild();
        ChangeScene(Paths.SCENE_LOBBY);
        ((LobbyManager)UnityEngine.Object.FindObjectOfType(typeof(LobbyManager))).StartGameImmediately = false;
        ((NickNamePanel)UnityEngine.Object.FindObjectOfType(typeof(NickNamePanel))).autoNickName = false;
        string x = BuildPipeline.BuildPlayer(levels, path_WebGL, BuildTarget.WebGL, BuildOptions.None);
        Debug.Log(x);
    }

    [MenuItem("MyTools/MakeViewIds %e")]
    public static void MakeViewIds()
    {
        Debug.Log("MakeViewIDS");
        short nextViewId = NetworkViewsManagement.INVALID_VIEW_ID + 1;
        MakeViewIds(Paths.SCENE_LOBBY, ref nextViewId);
        MakeViewIds(Paths.SCENE_MAIN, ref nextViewId);
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
            //EditorSceneManager.SaveOpenScenes();
        }
        EditorSceneManager.SaveScene(EditorSceneManager.GetSceneByPath(scene));
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
        foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("Build"))
        {
            process.Kill();
        }
    }


    private static void ChangeScene(string sceneName)
    {
        if (EditorSceneManager.GetActiveScene().name != sceneName)
        {
            EditorSceneManager.SaveOpenScenes();// SaveScene();// SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(sceneName);
        }
    }

    [MenuItem("MyTools/Save %i")]
    public static void SaveScene()
    {
        ChangeScene(Paths.SCENE_MAIN);

    }
}
