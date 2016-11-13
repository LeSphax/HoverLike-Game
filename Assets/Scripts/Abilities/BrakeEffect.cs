using System;
using UnityEngine;

public class BrakeEffect : AbilityEffectBuilder
{
    public float BRAKE_AMOUNT =20f;

    public void ApplyOnTarget(GameObject target, Vector3 position)
    {
        Vector3 currentVelocity = target.GetComponent<CustomRigidbody>().velocity;
        if (currentVelocity.magnitude != 0)
        {
            float reduction = (currentVelocity.magnitude - BRAKE_AMOUNT * Time.deltaTime) / currentVelocity.magnitude;
            target.GetComponent<CustomRigidbody>().velocity *= reduction;
            target.GetComponent<PlayerController>().DestroyTarget();
        }
    }

    public override AbilityEffect GetEffect(params object[] parameters)
    {
        throw new NotImplementedException();
    }
}
