using UnityEngine;

public class Goal : MonoBehaviour
{

    public int teamNumber = 1;

    void OnTriggerEnter(Collider collider)
    {
        if (MyComponents.NetworkManagement.isServer && collider.gameObject.tag == Tags.Ball)
        {
            if (!MyComponents.BallState.IsAttached())
                MyComponents.MatchManager.TeamScored(teamNumber);
        }
    }



}
