using UnityEngine;
using System.Collections;
using TimeSlow;
using AbilitiesManagement;
using CustomAnimations;

public class RendererTimeSlow : AVisualEffect
{

    private Material material;

    public void InitView(object[] parameters)
    {
        Color newColor = (Color)parameters[0];
        newColor.a = material.color.a;
        material.color = newColor;
    }

    private Renderer m_Renderer;
    private Renderer M_Renderer
    {
        get
        {
            if (m_Renderer == null)
            {
                m_Renderer = GetComponent<Renderer>();
            }
            return m_Renderer;
        }
    }

    private float currentY;

    protected override void Awake()
    {
        base.Awake();
        material = GetComponent<Renderer>().material;
        transform.localScale = Vector3.one * TimeSlowTargeting.DIAMETER_TIME_SLOW_ZONE;
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
