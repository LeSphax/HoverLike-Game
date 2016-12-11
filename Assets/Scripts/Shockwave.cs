using UnityEngine;
using System.Collections;
using CustomAnimations;

public class Shockwave : MonoBehaviour
{

    public event AnimationEventHandler FinishedAnimating;
    public float scale;

    ScaleAnimation scaleAnimation;
    GetComponentsFadeAnimation fadeAnimation;
    // Use this for initialization
    void Start()
    {
        scaleAnimation = ScaleAnimation.CreateScaleAnimation(gameObject, 0, scale, 0.25f, false);
        fadeAnimation = GetComponentsFadeAnimation.CreateGetComponentsFadeAnimation(gameObject, 0.25f);
        scaleAnimation.StartAnimating();
        fadeAnimation.StartReverseAnimating();
        fadeAnimation.FinishedAnimating += FinishedAnimating;
    }

}
