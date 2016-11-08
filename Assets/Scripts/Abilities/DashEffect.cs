using UnityEngine;

public class DashEffect : AbilityEffect
{
    private bool dashing = false;
    public float speed = 8f;
    public float endSpeed = 3f;
    public float dashDuration = 0.4f;

    private Vector3 force;
    private CustomRigidbody myRigidbody;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        myRigidbody = target.GetComponent<CustomRigidbody>();
        //
        target.GetComponent<PlayerController>().DestroyTarget();
        target.transform.LookAt(position + Vector3.up * target.transform.position.y);
        //
        force = new Vector3(position.x - target.transform.position.x, 0, position.z - target.transform.position.z);
        force.Normalize();
        //
        myRigidbody.velocity = Vector3.zero;
        dashing = true;
        Invoke("StopDashing", dashDuration);
    }

    void FixedUpdate()
    {
        if (dashing)
        {
            myRigidbody.velocity = force * speed;
        }
    }

    private void StopDashing()
    {
        dashing = false;
        myRigidbody.velocity = force * endSpeed;
    }

}
