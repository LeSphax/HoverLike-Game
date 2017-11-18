using CustomAnimations;
using TimeSlow;
using UnityEngine;

public class RendererTimeSlow : AVisualEffect
{
    public GameObject circle;

    private Material material;
    private Material circleMaterial;

    public void InitView(object[] parameters)
    {
        Color newColor = (Color)parameters[0];
        circleMaterial.color = newColor;
        newColor.a = material.color.a;
        material.color = newColor;
    }

    private float currentY;

    protected override void Awake()
    {
        base.Awake();
        material = GetComponent<Renderer>().material;
        transform.localScale = Vector3.one * TimeSlowTargeting.DIAMETER_TIME_SLOW_ZONE;
        circleMaterial = circle.GetComponent<Renderer>().material;
        circleMaterial.SetFloat("Scale", TimeSlowTargeting.DIAMETER_TIME_SLOW_ZONE);
    }

    private void Start()
    {
        AppearAnimation animation = AppearAnimation.Create(gameObject, 0, transform.localScale.y / 2, 0.5f);
        DestroyAfterTimeout destroyer = gameObject.AddComponent<DestroyAfterTimeout>();
        destroyer.timeout = TimeSlowPersistentEffect.DURATION;
        destroyer.m_animation = animation;
        animation.StartAnimating();
    }
}
