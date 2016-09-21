using System;
using System.Collections.Generic;
using UnityEngine;

public class DashEffect : AbilityEffect
{
    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        target.transform.LookAt(position + Vector3.up * target.transform.position.y);
        Vector3 force = new Vector3(position.x - target.transform.position.x, 0, position.z - target.transform.position.z);
        force.Normalize();
        target.GetComponent<CustomRigidbody>().AddForce(force * 100);
    }
}
