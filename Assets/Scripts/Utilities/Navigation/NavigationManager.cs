using UnityEngine;
using UnityEngine.SceneManagement;

namespace Navigation
{
    public delegate void SceneChangeEventHandler(short previousSceneId, short newSceneId);

    public class NavigationManager : SlideBall.MonoBehaviour
    {
        public static short previousSceneIndex;
        public static SceneLoader loader;
        public static event SceneChangeEventHandler FinishedLoadingScene;

        void Start()
        {
            loader = GetComponent<SceneLoader>();
            loader.FinishedLoading += FinishedLoading;
        }

        private static bool IsSceneMain()
        {
            return Scenes.currentSceneId == Scenes.MainIndex;
        }

        internal static void LoadScene(string scene, bool fading = false, bool waitToShowNextLevel =false)
        {
            Debug.Log("LoadScene");
            previousSceneIndex = Scenes.currentSceneId;
            loader.StartLoading(scene, fading, waitToShowNextLevel);
        }

        private void FinishedLoading()
        {
            if (FinishedLoadingScene != null)
            {
                FinishedLoadingScene.Invoke(previousSceneIndex, Scenes.currentSceneId);
            }
            if (Scenes.IsCurrentScene(Scenes.LobbyIndex))
                MyComponents.NetworkViewsManagement.ResetViews();
            MyComponents.NullifyComponents();
        }

        public static void ShowLevel()
        {
            loader.ShowNewLevel();
        }

        public static void Reset()
        {
            FinishedLoadingScene = null;
        }
    }
}

