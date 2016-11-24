using System;
using UnityEngine;

public abstract class AbilityEffect : MonoBehaviour
{
    public abstract void ApplyOnTarget(params object[] parameters);
}