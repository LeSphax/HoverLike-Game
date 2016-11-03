using PlayerManagement;
using UnityEngine;

namespace PlayerBallControl
{
    public class ShootEffect : AbilityEffect
    {

        public override void ApplyOnTarget(GameObject target, Vector3 position)
        {
            target.GetComponent<PlayerBallController>().ClientThrowBall(position, GetComponent<PowerBar>().powerValue);
        }

    }
}