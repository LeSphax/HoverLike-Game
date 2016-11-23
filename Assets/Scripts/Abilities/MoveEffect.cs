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

        public override void ApplyOnTarget(GameObject player, Vector3 position)
        {
            player.GetComponent<PlayerController>().View.RPC("CreateTarget",RPCTargets.Server,position);
            Instantiate(moveUIAnimationPrefab, position, Quaternion.identity);
        }

    }
}