using UnityEngine;

public class Goal : MonoBehaviour {

    public int teamNumber = 1;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == Tags.Ball)
        {
            Scoreboard.incrementTeamScore(teamNumber);
        }
    }
	
}
