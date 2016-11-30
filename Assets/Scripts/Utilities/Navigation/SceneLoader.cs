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

        public void StartLoading(string levelName)
        {
            this.levelName = levelName;
            StartCoroutine("load");
        }

        IEnumerator load()
        {
            async = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
            SceneManager.sceneLoaded += LevelLoaded;
            async.allowSceneActivation = true;
            fading = false;
            yield return async;
        }

        public void ActivateScene()
        {
            async.allowSceneActivation = true;
            CameraFade.FinishedFade -= ActivateScene;
        }


        private void Update()
        {
            //if (async != null && async.progress >= 0.9f)
            //{
            //    if (!fading)
            //    {
            //        fading = true;
            //        //CameraFade.FinishedFade += ActivateScene;
            //        //CameraFade.StartFade(CameraFade.FadeType.FADEIN);
            //    }
            //}
        }

        void LevelLoaded(Scene scene, LoadSceneMode mode)
        {
            if (FinishedLoading != null)
                FinishedLoading.Invoke();
            // CameraFade.InstantFadeOut();
            async = null;
            SceneManager.sceneLoaded -= LevelLoaded;
        }

        private static Camera GetMainCamera()
        {
            return Camera.main;
        }
    }
}