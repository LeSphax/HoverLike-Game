using AbilitiesManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AVisualEffect : MonoBehaviour
{
    protected virtual void Awake()
    {
        AbilitiesManager.visualEffects.Add(this);
    }

    public virtual void ClearEffect()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        AbilitiesManager.visualEffects.Remove(this);
    }

}
