using UnityEngine;

public class Goal : SlideBall.MonoBehaviour
{

    public int teamNumber = 1;

    void OnTriggerEnter(Collider collider)
    {
        if (EditorVariables.CanScoreGoals && NetworkingState.IsServer && collider.gameObject.tag == Tags.Ball)
        {
            if (!MyComponents.BallState.IsAttached() && !MyComponents.BallState.UnCatchable)
                MyComponents.MatchManager.TeamScored(teamNumber);
        }
    }



}
