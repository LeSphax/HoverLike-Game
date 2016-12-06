using UnityEngine;
using UnityEngine.SceneManagement;

namespace Navigation
{
    public class NavigationManager : MonoBehaviour
    {
        public static SceneLoader loader;
        public static event EmptyEventHandler FinishedLoadingScene;

        void Start()
        {
            loader = GetComponent<SceneLoader>();
            loader.FinishedLoading += FinishedLoading;
        }

        private static bool IsSceneMain()
        {
            return SceneManager.GetActiveScene().buildIndex == Scenes.MainIndex;
        }

        internal static void LoadScene(string scene)
        {
            loader.StartLoading(scene, false, false);
        }

        internal static void LoadScene(string scene, bool fading, bool waitToShowNextLevel)
        {
            loader.StartLoading(scene, fading, waitToShowNextLevel);
        }

        private static void FinishedLoading()
        {
            if (FinishedLoadingScene != null)
            {
                FinishedLoadingScene.Invoke();
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

