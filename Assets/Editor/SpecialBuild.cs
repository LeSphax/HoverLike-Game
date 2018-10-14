// C# example.
using SlideBall;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpecialBuild
{
    private const string path = "C:/Users/sbker/Desktop/Workspace/UnityProjects/hover/Builds/PC/Slideball.exe";
    private const string path_WebGL = "C:/Users/sbker/Desktop/Workspace/UnityProjects/SlideBall/Builds/";
    private const string path_ML_1 = "C:/Users/Sebas/Desktop/Workspace/UnityProjects/ml-agents/Builds/slideBall/1/slideball1.exe";
    private const string path_ML_2 = "C:/Users/Sebas/Desktop/Workspace/UnityProjects/ml-agents/Builds/slideBall/2/slideball2.exe";
    private const string path_ML_3 = "C:/Users/Sebas/Desktop/Workspace/UnityProjects/ml-agents/Builds/slideBall/3/slideball3.exe";


    private static string[] levels = new string[] { Paths.SCENE_LOBBY, Paths.SCENE_MAIN };

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

        return BuildPlayer(levels, path, BuildTarget.StandaloneWindows64, BuildOptions.Development);

    }

    private static bool BuildPlayer(string[] levels, string locationPathName, BuildTarget target, BuildOptions options)
    {
        BuildReport buildReport = BuildPipeline.BuildPlayer(levels, locationPathName, target, options);
        if (buildReport.summary.result == BuildResult.Cancelled || buildReport.summary.result == BuildResult.Failed)
        {
            Debug.LogError(buildReport.summary);
            return false;
        }
        Debug.Log("Build Finished in " + buildReport.summary.totalTime + " s");
        Debug.Log("Total size: " + buildReport.summary.totalSize);
        Debug.Log("File Location: " + buildReport.summary.outputPath);
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

        BuildPlayer(levels, path_WebGL, BuildTarget.WebGL, BuildOptions.None);
    }

    [MenuItem("MyTools/Build ML #_1")]
    public static void BuildML1()
    {
        BuildML(path_ML_1);
    }

    [MenuItem("MyTools/Build ML #_2")]
    public static void BuildML2()
    {
        BuildML(path_ML_2);
    }

    [MenuItem("MyTools/Build ML #_3")]
    public static void BuildML3()
    {
        BuildML(path_ML_3);
    }

    private static void BuildML(string path)
    {
        PrepareBuild();
        ChangeScene(Paths.SCENE_ML);
        MakeViewIdsCurrentScene();

        BuildPlayer(new[] { Paths.SCENE_ML }, path, BuildTarget.StandaloneWindows64, BuildOptions.None);
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

    [MenuItem("MyTools/MakeViewIdsCurrentScene %;")]
    public static void MakeViewIdsCurrentScene()
    {
        Debug.Log("MakeViewIDS Current Scene");
        short nextViewId = NetworkViewsManagement.INVALID_VIEW_ID + 1;
        MakeViewIds(SceneManager.GetActiveScene().path, ref nextViewId);

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
        foreach (ANetworkView view in Resources.FindObjectsOfTypeAll(typeof(ANetworkView)))
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
                EditorSceneManager.SaveOpenScenes();
                EditorSceneManager.OpenScene(sceneName);
            }
            catch (InvalidOperationException)
            {
                if (!tried)
                {
                    ChangeScene(sceneName, true);
                }
            }
        }
    }
}
