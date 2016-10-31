using UnityEngine;

public class Goal : MonoBehaviour
{

    public int teamNumber = 1;

    void OnTriggerEnter(Collider collider)
    {
        if (MyGameObjects.NetworkManagement.isServer && collider.gameObject.tag == Tags.Ball)
        {
            MyGameObjects.MatchManager.TeamScored(teamNumber);
        }
    }



}
