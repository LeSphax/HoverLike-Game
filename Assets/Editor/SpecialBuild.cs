// C# example.
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SpecialBuild
{
    private static short nextViewId = 0;
    private const string path = "C:/Programmation/Workspace/UnityProjects/Hover/Builds/PC/Build.exe";

    private static string[] levels = new string[] { Paths.SCENE_LOBBY, Paths.SCENE_MAIN };

    private static float startWaitingTime = -1;

    private static List<System.Diagnostics.Process> processes = new List<System.Diagnostics.Process>();

    [MenuItem("MyTools/EditorAsServer %g")]
    public static void BuildWithLobby()
    {
        BuildOnly();
        RunGame();

        ChangeScene(Paths.SCENE_LOBBY);
        EditorApplication.isPlaying = true;
        //var proc = new System.Diagnostics.Process();
        //proc.StartInfo.FileName = path;
        //proc.Start();
    }

    [MenuItem("MyTools/EditorAsClient %h")]
    public static void BuildWithoutLobby()
    {
        //BuildGame(new string[] { Paths.SCENE_MAIN });
        BuildOnly();
        RunGame();

        ChangeScene(Paths.SCENE_LOBBY);

        //ChangeScene(Paths.SCENE_MAIN);
        //EditorApplication.isPlaying = true;
        //startWaitingTime = Time.realtimeSinceStartup;
        //EditorApplication.update += OnEditorUpdate;


    }

    [MenuItem("MyTools/Build Only %j")]
    public static void BuildOnly()
    {
        BuildGame(levels);


        //startWaitingTime = Time.realtimeSinceStartup;
        //EditorApplication.update += OnEditorUpdate;


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

    private static void BuildGame(string[] levels)
    {
        KillGames();
        EditorApplication.isPlaying = false;
        MakeViewIds();

        // Get filename.
        //string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

//#if UNITY_WEBGL
        //Debug.Log(BuildPipeline.BuildPlayer(levels, path, BuildTarget.WebGL, BuildOptions.None));
        //#else
        Debug.Log(BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows64, BuildOptions.Development));
        //#endif

    }

    [MenuItem("MyTools/MakeViewIds %e")]
    public static void MakeViewIds()
    {
        nextViewId = 0;
        MakeViewIds(Paths.SCENE_LOBBY);
        MakeViewIds(Paths.SCENE_MAIN);
        if (!Settings.Data.ContainsKey(Settings.NEXT_VIEW_ID))
            Settings.Data.Add(Settings.NEXT_VIEW_ID, "" + nextViewId);
        else
            Settings.Data[Settings.NEXT_VIEW_ID] = "" + nextViewId;
        Settings.SaveSettings();
    }

    public static void MakeViewIds(string scene)
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
        foreach(System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("Build"))
        {
            Debug.Log(process);
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
