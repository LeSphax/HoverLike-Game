using System;
using AbilitiesManagement;
using UnityEngine;

public class DashEffect : AbilityEffect
{


    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        target.GetComponent<PlayerController>().View.RPC("Dash", RPCTargets.Server, position);
    }

}

public class PersistentDashEffect : PersistentEffect
{
    float time;
    public float speed = 8f;
    public float endSpeed = 3f;
    public float dashDuration = 0.4f;

    private Vector3 force;
    private Rigidbody myRigidbody;

    public PersistentDashEffect(AbilitiesManager manager, Vector3 position) : base(manager)
    {
        myRigidbody = manager.GetComponent<Rigidbody>();
        //
        manager.controller.DestroyTarget();
        manager.transform.LookAt(position + Vector3.up * manager.transform.position.y);
        //
        force = new Vector3(position.x - manager.transform.position.x, 0, position.z - manager.transform.position.z);
        force.Normalize();
        //
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
    }

    internal override void ApplyEffect(float dt)
    {
        time += dt;
        if (time < dashDuration)
            myRigidbody.velocity = force * speed;
        else
            EndDash();
    }

    private void EndDash()
    {
        myRigidbody.velocity = force * endSpeed;
        DestroyEffect();
    }
}
