using UnityEngine;
using AbilitiesManagement;
using CustomAnimations;
using System.Collections.Generic;

public class StealRenderer : IVisualEffect
{
    public Color STEAL_COLOR = Color.white;

    public GameObject[] objectsToAnimate;
    private Dictionary<Material, Color> materials = new Dictionary<Material, Color>();
    //private List<MyAnimation> animations = new List<MyAnimation>();

    protected override void Awake()
    {
        foreach (GameObject go in objectsToAnimate)
        {
            //FieldAnimation animation = FieldAnimation.Create(go, 0.5f, FieldDirection.DOWN);
            //animations.Add(animation);
            materials.Add(go.GetComponent<Renderer>().material, Color.black);
        }
    }

    public void StartAnimating(float duration)
    {
        AbilitiesManager.visualEffects.Add(this);
        materials.Clear();
        foreach (GameObject go in objectsToAnimate)
        {
            //FieldAnimation animation = FieldAnimation.Create(go, 0.5f, FieldDirection.DOWN);
            //animations.Add(animation);
            Material material = go.GetComponent<Renderer>().material;
            materials.Add(material, material.color);
            material.color = STEAL_COLOR;
            Debug.Log(material.name);
        }
        Invoke("ClearEffect", duration);
    }

    public override void ClearEffect()
    {
        //CancelInvoke("ClearEffect");
        foreach (var pair in materials)
        {
            pair.Key.color = pair.Value;
        }
    }

    void RestartAnimation(MyAnimation animation)
    {
        animation.StartAnimating(true);
    }
}
