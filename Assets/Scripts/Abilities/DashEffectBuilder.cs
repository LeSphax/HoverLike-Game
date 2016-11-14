using System;
using UnityEngine;

public class DashEffectBuilder : AbilityEffectBuilder
{
    public override AbilityEffect GetEffect(params object[] parameters)
    {
        return new DashEffect((Vector3)parameters[0]);
    }

}

public class DashEffect : PersistentAbilityEffect
{
    public const float SPEED = 200f;
    public float endSpeed = 30f;
    public float dashDuration = 0.15f;
    public Vector3 target;

    private Vector3 force;
    private CustomRigidbody myRigidbody;

    private float time;

    public DashEffect() { }

    public DashEffect(Vector3 target)
    {
        this.target = target;
    }

    public override void ApplyEffect(PlayerPhysicsModel model)
    {
        base.ApplyEffect(model);
        PlayerController controller = model.controller;
        myRigidbody = model.GetComponent<CustomRigidbody>();
        //
        controller.DestroyTarget();
        model.transform.LookAt(target - target.y * Vector3.up + Vector3.up * model.transform.position.y);
        //
        force = new Vector3(target.x - model.transform.position.x, 0, target.z - model.transform.position.z);
        force.Normalize();
        //
        myRigidbody.velocity = Vector3.zero;
        time = 0;
    }


    public override void Simulate(float dt)
    {
        myRigidbody.velocity = force * SPEED;
        time += dt;
        if (time >= dashDuration)
        {
            StopDashing();
            UnApplyEffect(model);
        }
    }

    private void StopDashing()
    {
        myRigidbody.velocity = force * endSpeed;
    }

    public override int Deserialize(byte[] data, int currentIndex)
    {
        target = BitConverterExtensions.ToVector3(data, currentIndex);
        return 12;
    }

    public override InputFlag GetInputFlag()
    {
        return InputFlag.FIRST;
    }

    public override byte[] Serialize()
    {
        return BitConverterExtensions.GetBytes(target);
    }

    public override void UnApplyEffect(PlayerPhysicsModel playerPhysicsModel)
    {
        base.UnApplyEffect(playerPhysicsModel);
    }

    public override bool IsSerialisable()
    {
        return true;
    }

}
