using UnityEngine;

public class DashEffect : AbilityEffect
{
    private bool dashing = false;
    public float speed = 10f;
    private float dashDuration = 0.25f;

    private Vector3 force;
    private Rigidbody myRigidbody;
    private GameObject target;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        this.target = target;
        myRigidbody = target.GetComponent<Rigidbody>();
        //
        target.GetComponent<PlayerController>().DestroyTarget();
        target.transform.LookAt(position + Vector3.up * target.transform.position.y);
        //
        force = new Vector3(position.x - target.transform.position.x, 0, position.z - target.transform.position.z);
        force.Normalize();
        //
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
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
        target.GetComponent<PlayerMovementView>().ClampPlayerVelocity();
    }

}
