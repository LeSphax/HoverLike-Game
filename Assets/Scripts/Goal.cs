using UnityEngine;

public class Goal : SlideBall.MonoBehaviour
{
    const float BASE_SIZE = 35;

    public event TeamEventHandler GoalScored;

    public int teamNumber = 1;

    public GameObject back;
    public GameObject top;
    public GameObject left;
    public GameObject right;
    public BoxCollider playersBlocker;
    public BoxCollider goalDetector;


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

    public void SetSize(float size)
    {
        back.transform.localScale = new Vector3(size, back.transform.localScale.y, back.transform.localScale.z);
        top.transform.localScale = new Vector3(size +2, top.transform.localScale.y, top.transform.localScale.z);
        left.transform.localPosition = new Vector3(size/2, left.transform.localPosition.y, left.transform.localPosition.z);
        right.transform.localPosition = new Vector3(-size/2, right.transform.localPosition.y, right.transform.localPosition.z);
        playersBlocker.size = new Vector3(size - 3, playersBlocker.size.y, playersBlocker.size.z);
        goalDetector.size = new Vector3(size - 5, goalDetector.size.y, goalDetector.size.z);
    }



}
