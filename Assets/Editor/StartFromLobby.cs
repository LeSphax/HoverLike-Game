using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

public class StartFromLobby : MonoBehaviour
{

    [MenuItem("MyTools/Move to lobby %l")]
    public static void PlayFromPrelaunchScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(Paths.SCENE_LOBBY);
    }

    [MenuItem("MyTools/Move to main %m")]
    public static void MoveToMain()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(Paths.SCENE_MAIN);
    }
}
