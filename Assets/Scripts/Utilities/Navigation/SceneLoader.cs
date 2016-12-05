using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Navigation
{
    public class SceneLoader : MonoBehaviour
    {
        public event EmptyEventHandler FinishedLoading;
        string levelName;
        AsyncOperation async;
        bool fading = false;
        bool finishedFade = false;

        public void StartLoading(string levelName)
        {
            this.levelName = levelName;
            StartCoroutine("load");
        }

        IEnumerator load()
        {
            async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
            SceneManager.sceneLoaded += LevelLoaded;
            async.allowSceneActivation = false;
            yield return async;
        }

        public void FinishedFade()
        {
            Debug.Log("FinishedFade");
            finishedFade = true;
            CameraFade.FinishedFade -= FinishedFade;
        }


        private void Update()
        {
            if (async != null)
            {
                if (!fading)
                {
                    fading = true;
                    CameraFade.FinishedFade += FinishedFade;
                    CameraFade.StartFade(CameraFade.FadeType.FADEIN);
                }
            }
            if (finishedFade && async != null && async.progress >= 0.9f)
            {
                Debug.Log("FinishedFade");
                async.allowSceneActivation = true;
                fading = false;
                finishedFade = false;
                async = null;
            }
        }

        public void ShowNewLevel()
        {
            Debug.Log("ShowNewLevel");
            CameraFade.StartFade(CameraFade.FadeType.FADEOUT);
        }

        void LevelLoaded(Scene scene, LoadSceneMode mode)
        {
            if (FinishedLoading != null)
                FinishedLoading.Invoke();
            async = null;
            SceneManager.sceneLoaded -= LevelLoaded;
        }

        private static Camera GetMainCamera()
        {
            return Camera.main;
        }
    }
}