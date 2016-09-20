using UnityEngine;

public class BrakeEffect : AbilityEffect
{
    public float BRAKE_AMOUNT =20f;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        Vector3 currentVelocity = target.GetComponent<Rigidbody>().velocity;
        if (currentVelocity.magnitude != 0)
        {
            float reduction = (currentVelocity.magnitude - BRAKE_AMOUNT * Time.deltaTime) / currentVelocity.magnitude;
            target.GetComponent<Rigidbody>().velocity *= reduction;
            target.GetComponent<PlayerController>().DestroyTarget();
        }
    }
}
