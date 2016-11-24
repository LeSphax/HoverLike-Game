using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class PassTargeting : AbilityTargeting
{
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

    private GameObject targeter;

    private CastOnTarget callback;

    public override void ChooseTarget(CastOnTarget callback)
    {
        this.callback = callback;
        targeter = (GameObject)Instantiate(PrefabTargeter, transform, true);
        IsTargeting = true;
        UpdateTargeterPosition();
    }

    void Update()
    {
        if (targeter != null)
        {
            UpdateTargeterPosition();
            if (Input.GetMouseButtonDown(0))
            {
                ConnectionId playerId = GetPlayerAtTargetPosition();
                if (playerId != Players.INVALID_PLAYER_ID)
                    callback.Invoke(playerId);
                CancelTargeting();
            }
        }
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
            PlayerController controller = GetParentController(go.transform);
            if (controller != null && controller.playerConnectionId != Players.myPlayerId /*&& controller.Player.Team == Players.MyPlayer.Team*/)
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


