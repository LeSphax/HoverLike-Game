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

        private static void FinishedLoading()
        {
            if (FinishedLoadingScene != null)
            {
                Debug.Log("InvokeFinishedLoading");
                FinishedLoadingScene.Invoke();
            }
            if (Scenes.IsCurrentScene(Scenes.LobbyIndex))
                MyComponents.NetworkViewsManagement.ResetViews();
            MyComponents.NullifyComponents();
        }

        public static void Reset()
        {
            FinishedLoadingScene = null;
        }
    }
}

