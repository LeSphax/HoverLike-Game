using UnityEngine;

namespace PlayerBallControl
{
    public class MoveEffect : AbilityEffect
    {
        GameObject moveUIAnimationPrefab;

        void Awake()
        {
            moveUIAnimationPrefab = Resources.Load<GameObject>(Paths.MOVE_UI_ANIMATION);
        }

        public override void ApplyOnTarget(params object[] parameters)
        {
            PlayerController controller = (PlayerController)parameters[0];
            Vector3 position = (Vector3)parameters[1];
            controller.abilitiesManager.View.RPC("Move", RPCTargets.Server, new Vector2(position.x,position.z));
            Instantiate(moveUIAnimationPrefab, position, Quaternion.identity);
        }

    }
}