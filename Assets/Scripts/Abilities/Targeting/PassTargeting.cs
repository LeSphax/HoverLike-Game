using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class PassTargeting : AbilityTargeting
{

    public static float DIAMETER_PASS_ZONE
    {
        get
        {
            return ResourcesGetter.PassTargeterPrefab.transform.localScale.x * 10;
        }
    }

    //A circle showing the area on which the ability will be cast.
    private GameObject targeter;

    private CastOnTarget callback;

    public override void ChooseTarget(CastOnTarget callback)
    {
        this.callback = callback;
        targeter = (GameObject)Instantiate(ResourcesGetter.PassTargeterPrefab, transform, true);
        IsTargeting = true;
        UpdateTargeterPosition();
    }

    void Update()
    {
        if (targeter != null)
        {
            UpdateTargeterPosition();
        }
    }

    public override void ReactivateTargeting()
    {
        ConnectionId playerId = GetPlayerAtTargetPosition();
        callback.Invoke(playerId != Players.INVALID_PLAYER_ID, playerId);
        CancelTargeting();
    }

    public override void CancelTargeting()
    {
        IsTargeting = false;
        Destroy(targeter);
    }

    private void UpdateTargeterPosition()
    {
        targeter.transform.position = Functions.GetMouseWorldPosition() + Vector3.up * 0.2f;
    }

    private ConnectionId GetPlayerAtTargetPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(targeter.transform.position, DIAMETER_PASS_ZONE / 2, LayersGetter.PlayersMask());

        float closestPlayerDistance = Mathf.Infinity;
        PlayerController closestPlayer = null;

        foreach (Collider hit in colliders)
        {
            GameObject go = hit.gameObject;
            PlayerController controller = GetParentPlayerController(go.transform);
            if (controller != null && controller.playerConnectionId != Players.myPlayerId && controller.Player.Team == Players.MyPlayer.Team)
            {
                float distance = Vector3.Distance(targeter.transform.position, controller.transform.position);

                if (distance < closestPlayerDistance)
                {
                    closestPlayerDistance = distance;
                    closestPlayer = controller;
                }
            }
        }
        if (closestPlayer != null)
            return closestPlayer.playerConnectionId;
        else
            return Players.INVALID_PLAYER_ID;
    }

    //Recursive function to catch the PlayerController of a gameObjects from one of its children.
    private PlayerController GetParentPlayerController(Transform t)
    {
        PlayerController controller = t.GetComponent<PlayerController>();
        if (controller != null)
        {
            return controller;
        }
        else if (t.parent != null)
            return GetParentPlayerController(t.parent);
        else
            return null;
    }
}


