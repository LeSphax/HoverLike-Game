using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffect : AbilityEffect
{

    public float TimeBeforeTeleportation;

    private GameObject target;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        this.target = target;
        target.GetComponent<PlayerController>().StopMoving();

        MyComponents.NetworkViewsManagement.Instantiate("Effects/Teleportation", target.transform.position, Quaternion.identity);
        MyComponents.NetworkViewsManagement.Instantiate("Effects/Teleportation", position, Quaternion.identity);

        Invoke("Teleport", TimeBeforeTeleportation);
    }

    private void Teleport()
    {
        target.transform.position = Players.MyPlayer.SpawningPoint;
    }
}
