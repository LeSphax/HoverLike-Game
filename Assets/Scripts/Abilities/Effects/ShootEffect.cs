using PlayerManagement;
using UnityEngine;

namespace PlayerBallControl
{
    public class ShootEffect : AbilityEffect
    {
        public override void ApplyOnTarget(params object[] parameters)
        {
            base.ApplyOnTarget(parameters);
            PlayerController controller = (PlayerController)parameters[0];
            Vector3[] controlPoints = (Vector3[])parameters[1];
            if (Mathf.Approximately(0, Vector3.Distance(controlPoints[1], controlPoints[2])))
                controller.abilitiesManager.View.RPC("Shoot", RPCTargets.Server, controlPoints[2], GetComponent<PowerBar>().powerValue);
            else
                controller.abilitiesManager.View.RPC("ShootCurved", RPCTargets.Server, controlPoints, GetComponent<PowerBar>().powerValue);

        }

    }
}