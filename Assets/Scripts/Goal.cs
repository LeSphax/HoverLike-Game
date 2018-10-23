using UnityEngine;

public class Goal : SlideBall.MonoBehaviour
{
    public event TeamEventHandler GoalScored;

    public int teamNumber = 1;

    void OnTriggerEnter(Collider collider)
    {
        if (EditorVariables.CanScoreGoals && NetworkingState.IsServer && collider.gameObject.tag == Tags.Ball)
        {
            if (!MyComponents.BallState.IsAttached() && !MyComponents.BallState.UnCatchable)
            {
                if (MyComponents.MatchManager != null)
                    MyComponents.MatchManager.TeamScored(teamNumber);
                if (GoalScored != null)
                {
                    GoalScored.Invoke((Team)teamNumber);
                }
            }
        }
    }



}
