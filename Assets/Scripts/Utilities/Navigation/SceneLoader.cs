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

        bool waitToShowNextLevel = false;
        bool useFade = false;
        bool isFading = false;
        bool finishedFade = false;

        public void StartLoading(string levelName, bool useFade, bool waitToShowNextLevel)
        {
            this.useFade = useFade;
            this.levelName = levelName;
            this.waitToShowNextLevel = waitToShowNextLevel;
            StartCoroutine("load");
        }

        IEnumerator load()
        {
            async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
            SceneManager.sceneLoaded += LevelLoaded;
            if (useFade)
                async.allowSceneActivation = false;
            yield return async;
        }

        public void FinishedFade()
        {
            finishedFade = true;
            CameraFade.FinishedFade -= FinishedFade;
        }


        private void Update()
        {
            if (useFade)
            {
                if (async != null)
                {
                    if (!isFading)
                    {
                        isFading = true;
                        CameraFade.FinishedFade += FinishedFade;
                        CameraFade.StartFade(CameraFade.FadeType.FADEIN);
                    }
                }
                if (finishedFade && async != null && async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                    if (!waitToShowNextLevel)
                        CameraFade.StartFade(CameraFade.FadeType.FADEOUT);
                    isFading = false;
                    finishedFade = false;
                    async = null;
                }
            }
        }

        public void ShowNewLevel()
        {
            if (CameraFade.IsBlack)
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