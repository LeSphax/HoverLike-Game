using UnityEngine;
using UnityEngine.SceneManagement;

namespace Navigation
{
    public class NavigationManager : MonoBehaviour
    {
        public static SceneLoader loader;
        public static event EmptyEventHandler FinishedLoadingGame;

        void Start()
        {
            loader = GetComponent<SceneLoader>();
            loader.FinishedLoading += FinishedLoadingScene;
        }

        public void GoBack()
        {
            if (IsSceneMain())
            {
                LoadScene(Scenes.Main);
            }
        }

        private static bool IsSceneMain()
        {
            return SceneManager.GetActiveScene().buildIndex == Scenes.MainIndex;
        }

        internal static void LoadScene(string scene)
        {
            loader.StartLoading(scene);
        }

        private static void FinishedLoadingScene()
        {
            if (IsSceneMain())
            {
                FinishedLoadingGame.Invoke();
                FinishedLoadingGame = null;
            }
        }
    }
}

