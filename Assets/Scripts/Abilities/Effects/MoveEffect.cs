using UnityEngine;

namespace PlayerBallControl
{
    public class MoveEffect : AbilityEffect
    {
        public override void ApplyOnTarget(params object[] parameters)
        {
            PlayerController controller = (PlayerController)parameters[0];
            Vector3 position = (Vector3)parameters[1];
            controller.abilitiesManager.View.RPC("Move", RPCTargets.Server, new Vector2(position.x,position.z));
            Instantiate(ResourcesGetter.MoveUIAnimationPrefab, position, Quaternion.identity);
        }

    }
}