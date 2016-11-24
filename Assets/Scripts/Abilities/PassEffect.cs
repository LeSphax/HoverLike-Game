using AbilitiesManagement;
using PlayerManagement;
using UnityEngine;

public class PassEffect : AbilityEffect
{

    public override void ApplyOnTarget(GameObject targetGameObject, Vector3 targetPosition)
    {
        Players.MyPlayer.controller.View.RPC("Pass", RPCTargets.Server, targetPosition);
    }



}

public class PassPersistentEffect : PersistentEffect
{
    Vector3 targetPosition;
    Vector3 controlPosition;
    Vector3 startPosition;

    private static GameObject prefabTargeter;
    private static GameObject PrefabTargeter
    {
        get
        {
            if (prefabTargeter == null)
            {
                prefabTargeter = Resources.Load<GameObject>(Paths.PASS_TARGETER);
            }
            return prefabTargeter;
        }
    }

    public static float DIAMETER_PASS_ZONE
    {
        get
        {
            return PrefabTargeter.transform.localScale.x * 10;
        }
    }

    public PassPersistentEffect(AbilitiesManager manager, Vector3 targetPosition) : base(manager)
    {
        MyComponents.BallState.SetAttached(BallState.NO_PLAYER_ID);
        MyComponents.BallState.Uncatchable = true;

        this.targetPosition = targetPosition;
        startPosition = manager.transform.position;
        controlPosition = startPosition + (targetPosition - startPosition) / 2 + Vector3.up * 20f;

        duration = 1.0f;
    }

    protected override void Apply(float dt)
    {
        MyComponents.BallState.transform.position = Functions.Bezier3(startPosition, controlPosition, targetPosition, time / duration);
    }

    protected override void StopEffect()
    {
        Collider[] colliders = Physics.OverlapSphere(targetPosition, DIAMETER_PASS_ZONE / 2, LayersGetter.PlayersMask());

        float closestPlayerDistance = Mathf.Infinity;
        int closestPlayerMatchingTeam = 0;
        PlayerController closestPlayer = null;

        foreach (Collider hit in colliders)
        {
            GameObject go = hit.gameObject;
            PlayerController controller = GetParentController(go.transform);
            if (controller != null)
            {
                float distance = Vector3.Distance(targetPosition, controller.transform.position);
                int matchingTeam = controller.Player.Team == manager.controller.Player.Team ? 1 : 0;

                if (distance < closestPlayerDistance || closestPlayerMatchingTeam < matchingTeam)
                {
                    closestPlayerDistance = distance;
                    closestPlayerMatchingTeam = matchingTeam;
                    closestPlayer = controller;
                }
            }
        }
        MyComponents.BallState.Uncatchable = false;

        if (closestPlayer != null)
            MyComponents.BallState.SetAttached(closestPlayer.playerConnectionId);
    }

    private PlayerController GetParentController(Transform t)
    {
        PlayerController controller = t.GetComponent<PlayerController>();
        if (controller != null)
        {
            return controller;
        }
        else if (t.parent != null)
            return GetParentController(t.parent);
        else
            return null;
    }
}