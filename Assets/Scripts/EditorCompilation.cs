#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorCompilation : MonoBehaviour
{

    protected virtual void Update()
    {
        if (EditorApplication.isCompiling)
        {
            EditorApplication.isPlaying = false;
            Debug.LogWarning("Unity hot reloading not currently supported. Stopping Editor Playback.");
            return;
        }
    }

    [InitializeOnLoad]
    public class AutosaveOnRun
    {
        static AutosaveOnRun()
        {
            EditorApplication.playmodeStateChanged = () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                {
                    Debug.Log("Auto-Saving scene before entering Play mode: " + EditorSceneManager.GetActiveScene());

                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                    EditorApplication.SaveAssets();
                }
            };
        }
    }
}
#endif
