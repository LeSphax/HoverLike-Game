using PlayerManagement;
using UnityEngine;

public class BlockEffect : AbilityEffect
{

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        target.GetComponent<PlayerController>().View.RPC("Block", RPCTargets.Server);
    }

}
