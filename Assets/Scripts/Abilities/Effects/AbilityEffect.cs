using System;
using UnityEngine;

//Apply an effect using parameters given by the corresponding abilityTargeting.
public abstract class AbilityEffect : MonoBehaviour
{
    public abstract void ApplyOnTarget(params object[] parameters);
}