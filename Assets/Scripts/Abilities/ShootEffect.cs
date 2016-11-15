using PlayerManagement;
using UnityEngine;

namespace PlayerBallControl
{
    public class ShootEffect : AbilityEffect
    {

        public override void ApplyOnTarget(GameObject target, Vector3 position)
        {
            target.GetComponent<PlayerBallController>().ThrowBall(position, GetComponent<PowerBar>().powerValue);
        }

    }
}