using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

public class StartFromLobby : MonoBehaviour
{

    // class doesn't matter, add to anything in the Editor folder
    // for any beginners reading, this is c#

    [MenuItem("MyTools/Play-Stop, But From Prelaunch Scene %y")]
    public static void PlayFromPrelaunchScene()
    {
        if (EditorApplication.isPlaying == true)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(Paths.SCENE_LOBBY);
        EditorApplication.isPlaying = true;
    }
}
