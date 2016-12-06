using System;
using CustomAnimations;
using UnityEngine;


public static class CameraFade
{
    public delegate void EmptyEventHandler();
    public static event EmptyEventHandler FinishedFade;


    private static MyAnimation currentAnimation;

    private static GameObject _screenFader;
    private static GameObject screenFader
    {
        get
        {
            if (_screenFader == null)
            {
                GameObject screenFaderPrefab = Resources.Load<GameObject>(Paths.SCREEN_FADER);
                _screenFader = UnityEngine.Object.Instantiate(screenFaderPrefab);
                UnityEngine.Object.DontDestroyOnLoad(_screenFader);
            }
            return _screenFader;
        }
    }

    public static bool IsBlack = false;

    public enum FadeType
    {
        FADEIN,
        FADEOUT,
    }

    private static FadeType currentType = FadeType.FADEIN;

    private static void FinishedFadeHandler(MonoBehaviour sender)
    {
        if (currentType.Equals(FadeType.FADEOUT))
            screenFader.SetActive(false);
        if (FinishedFade != null)
            FinishedFade.Invoke();
    }

    public static void StartFade(FadeType type)
    {
        StartFade(type, Color.black);
    }

    private static void CreateAnimation(Color targetColor)
    {
        currentAnimation = ScreenFaderAnimation.CreateScreenFaderAnimation(screenFader.transform.GetChild(0).gameObject, Color.clear, targetColor, 2.0F);
        currentAnimation.FinishedAnimating += FinishedFadeHandler;
    }

    internal static void InstantFadeOut()
    {
        screenFader.SetActive(false);
    }

    public static void StartFade(FadeType type, Color targetColor)
    {
        screenFader.SetActive(true);
        CreateAnimation(targetColor);

        currentType = type;

        switch (currentType)
        {
            case FadeType.FADEIN:
                currentAnimation.StartAnimating();
                IsBlack = true;
                Debug.Log("IsBlack" + IsBlack);
                break;
            case FadeType.FADEOUT:
                currentAnimation.StartReverseAnimating();
                IsBlack = false;
                Debug.Log("IsBlack" + IsBlack);
                break;
        }

    }
}
