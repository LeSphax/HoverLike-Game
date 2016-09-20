using System;
using UnityEngine;

public abstract class AbilityEffect : MonoBehaviour
{
    public abstract void ApplyOnTarget(GameObject target, Vector3 position);
}