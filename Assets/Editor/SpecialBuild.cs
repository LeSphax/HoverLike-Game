// C# example.
using System;
using UnityEditor;
using UnityEngine;

public class SpecialBuild
{
    private const string path = "C:/Programmation/Workspace/UnityProjects/Hover/Builds/PC/Build.exe";
    [MenuItem("MyTools/Build With PreProcess %g")]
    public static void BuildGame()
    {
        ANetworkView.nextViewId = 0;
        foreach (ANetworkView view in MyGameObjects.World.GetComponentsInChildren<ANetworkView>())
        {

            view.viewId = ANetworkView.nextViewId;
            Debug.Log((Component)view + "   " + view.viewId);
            ANetworkView.nextViewId++;
        }

        // Get filename.
        //string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] { "Assets/Main.unity" };

#if UNITY_WEBGL
        Debug.Log(BuildPipeline.BuildPlayer(levels, path, BuildTarget.WebGL, BuildOptions.None));
#else
        Debug.Log(BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows64, BuildOptions.Development));
#endif

        var proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = path;
        proc.Start();
    }

}
