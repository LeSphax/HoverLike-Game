using PlayerManagement;
using UnityEngine;

namespace PlayerBallControl
{
    public class MoveEffect : AbilityEffect
    {

        public override void ApplyOnTarget(GameObject player, Vector3 position)
        {
            player.GetComponent<PlayerController>().View.RPC("CreateTarget",RPCTargets.Server,position);
        }

    }
}