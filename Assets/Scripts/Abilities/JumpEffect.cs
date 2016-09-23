using UnityEngine;

public class JumpEffect : AbilityEffect
{

    public float jumpForce = 2000000f;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        Debug.Log("Jump");
        target.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce,ForceMode.VelocityChange);
    }
}
