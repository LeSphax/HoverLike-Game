using PlayerManagement;
using UnityEngine;

namespace PlayerBallControl
{
    public class ShootEffect : AbilityEffect
    {

        public override void ApplyOnTarget(params object[] parameters)
        {
            PlayerController controller = (PlayerController)parameters[0];
            Vector3 position = (Vector3)parameters[1];
            controller.ballController.ThrowBall(position, GetComponent<PowerBar>().powerValue);
        }

    }
}