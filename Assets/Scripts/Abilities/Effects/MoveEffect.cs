using UnityEngine;

namespace PlayerBallControl
{
    public class MoveEffect : AbilityEffect
    {
        public override void ApplyOnTarget(params object[] parameters)
        {
            PlayerController controller = (PlayerController)parameters[0];
            Vector3 direction = (Vector3)parameters[1];
            Vector3 mousePosition = MyComponents.InputManager.GetMouseLocalPosition();
            mousePosition.y = 0;
            mousePosition = MyComponents.transform.TransformPoint(mousePosition);

            Vector3 previousRotation = controller.transform.rotation.eulerAngles;
            controller.transform.LookAt(mousePosition);
            controller.transform.rotation = Quaternion.Euler(previousRotation.x, controller.transform.rotation.eulerAngles.y, previousRotation.z);

            controller.abilitiesManager.View.RPC("Move", RPCTargets.Server, direction, mousePosition);
        }

    }
}