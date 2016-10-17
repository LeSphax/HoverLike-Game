// C# example.
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SpecialBuild
{
    private const string path = "C:/Programmation/Workspace/UnityProjects/Hover/Builds/PC/Build.exe";

    private static float startWaitingTime = -1;

    [MenuItem("MyTools/EditorAsServer %g")]
    public static void EditorAsServer()
    {
        BuildGame();
        RunGame();

        ChangeScene(Paths.SCENE_LOBBY);
        EditorApplication.isPlaying = true;
        //var proc = new System.Diagnostics.Process();
        //proc.StartInfo.FileName = path;
        //proc.Start();
    }

    [MenuItem("MyTools/EditorAsClient %h")]
    public static void EditorAsClient()
    {
        BuildGame();
        RunGame();

        //startWaitingTime = Time.realtimeSinceStartup;
        //EditorApplication.update += OnEditorUpdate;


    }

    [MenuItem("MyTools/Build Only %j")]
    public static void BuildOnly()
    {
        BuildGame();


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

    private static void BuildGame()
    {
        MakeViewIds();

        // Get filename.
        //string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { Paths.SCENE_LOBBY, Paths.SCENE_MAIN };

#if UNITY_WEBGL
        Debug.Log(BuildPipeline.BuildPlayer(levels, path, BuildTarget.WebGL, BuildOptions.None));
#else
        Debug.Log(BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows64, BuildOptions.Development));
#endif

    }

    [MenuItem("MyTools/MakeViewIds %e")]
    public static void MakeViewIds()
    {
        ChangeScene(Paths.SCENE_MAIN);
        //MyGameObjects.RoomActivation.SetActive(true);
        Debug.ClearDeveloperConsole();
        MyGameObjects.NetworkViewsManagement.NextViewId = 0;
        foreach (ANetworkView view in MyGameObjects.Scene.GetComponentsInChildren<ANetworkView>())
        {

            view.ViewId = MyGameObjects.NetworkViewsManagement.NextViewId;
            Debug.Log(view + "   " + view.ViewId);
            MyGameObjects.NetworkViewsManagement.NextViewId++;
        }
        //MyGameObjects.RoomActivation.SetActive(false);
    }

    [MenuItem("MyTools/RunGame %q")]
    private static void RunGame()
    {
        var proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = path;
        proc.Start();
    }

    private static void ChangeScene(string sceneName)
    {
        if (EditorSceneManager.GetActiveScene().name != sceneName)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(sceneName);
        }
    }
}
