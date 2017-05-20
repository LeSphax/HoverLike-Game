﻿using System;
using AbilitiesManagement;
using UnityEngine;

public class DashEffect : AbilityEffect
{


    public override void ApplyOnTarget(params object[] parameters)
    {
        base.ApplyOnTarget(parameters);
        PlayerController controller = (PlayerController)parameters[0];
        Vector3 position = (Vector3)parameters[1];
        controller.View.RPC("Dash", RPCTargets.Server, position);
    }

}

public class DashPersistentEffect : PersistentEffect
{
    public float speed = 130f;
    public float endSpeed;
    public const float dashDuration = 0.25f;

    private Vector3 force;
    private Rigidbody myRigidbody;

    public DashPersistentEffect(AbilitiesManager manager, Vector3 position, float endSpeed) : base(manager)
    {
        this.endSpeed = endSpeed;
        myRigidbody = manager.GetComponent<Rigidbody>();
        
        //
        manager.controller.targetManager.CancelTarget();

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

    public override void StopEffect()
    {
        myRigidbody.velocity = force * endSpeed;
    }
}
