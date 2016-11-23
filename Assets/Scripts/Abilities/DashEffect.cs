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

public class DashPersistentEffect : PersistentEffect
{
    public float speed = 100f;
    public float endSpeed = 30f;
    public float dashDuration = 0.25f;

    private Vector3 force;
    private Rigidbody myRigidbody;

    public DashPersistentEffect(AbilitiesManager manager, Vector3 position) : base(manager)
    {
        myRigidbody = manager.GetComponent<Rigidbody>();
        //
        manager.controller.DestroyTarget();
        manager.transform.LookAt(position + Vector3.up * manager.transform.position.y);
        //
        force = new Vector3(position.x - manager.transform.position.x, 0, position.z - manager.transform.position.z);
        force.Normalize();
        //
        duration = dashDuration;
        myRigidbody.velocity = Vector3.zero;
        myRigidbody.angularVelocity = Vector3.zero;
    }


    protected override void Apply(float dt)
    {
        myRigidbody.velocity = force * speed;
    }

    protected override void StopEffect()
    {
        myRigidbody.velocity = force * endSpeed;
    }
}
