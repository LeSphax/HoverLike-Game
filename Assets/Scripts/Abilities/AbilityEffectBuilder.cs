using System;
using UnityEngine;

public abstract class AbilityEffectBuilder: MonoBehaviour 
{
    public abstract AbilityEffect GetEffect(params object[] parameters);
}