using UnityEngine;
using System.Collections;
using Byn.Net;
using PlayerManagement;
using CustomAnimations;
using UnityEngine.Assertions;

public class PassManager : MonoBehaviour
{

    Team throwerTeam;
    Vector3 targetPosition;
    Vector3 startPosition;

    float startTime;
    float currentTime;

    MovementAnimation myAnimation;


    [SerializeField]
    private GameObject prefabTargeter;
    private float sizeOverlapSphere
    {
        get
        {
            return prefabTargeter.transform.localScale.x * 10;
        }
    }


    // Use this for initialization
    void Start()
    {
        MyComponents.BallState.SetAttached(BallState.NO_PLAYER_ID);
        MyComponents.BallState.Uncatchable = true;

        myAnimation = MovementAnimation.CreateMovementAnimation(MyComponents.BallState.gameObject, targetPosition, 1.0f);
        myAnimation.FinishedAnimating += TargetReached;
        myAnimation.StartAnimating();
    }

    void TargetReached(MonoBehaviour sender)
    {
        Collider[] colliders = Physics.OverlapSphere(targetPosition, sizeOverlapSphere, LayersGetter.PlayersMask());

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
                int matchingTeam = controller.Player.Team == throwerTeam ? 1 : 0;

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

        Destroy(myAnimation);
        Destroy(gameObject);
    }

    void InitView(object[] parameters)
    {
        throwerTeam = Players.players[(ConnectionId)parameters[0]].Team;
        targetPosition = (Vector3)parameters[1];
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
