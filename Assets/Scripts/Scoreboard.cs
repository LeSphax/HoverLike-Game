using UnityEngine;
using UnityEngine.UI;
public class Scoreboard : Photon.MonoBehaviour {


    private static int scoreLeft = 0;
    private static int scoreRight= 0;

    private static Text textScoreLeft;
    private static Text textScoreRight;

    private static GameObject scoreboard;

    private static float timeLastGoal = 0;

    void Awake()
    {
        scoreboard = gameObject;
        textScoreLeft= transform.GetChild(0).GetComponent<Text>();
        textScoreRight= transform.GetChild(1).gameObject.GetComponent<Text>();
    }

    void Start()
    {
        UpdateScoreBoard();
    }

    private static void UpdateScoreBoard()
    {
        textScoreLeft.text = "" + scoreLeft;
        textScoreRight.text = "" + scoreRight;
    }

    public static void incrementTeamScore(int teamNumber)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (Time.realtimeSinceStartup - timeLastGoal > 1)
            {
                timeLastGoal = Time.realtimeSinceStartup;
                scoreboard.GetPhotonView().RPC("increment", PhotonTargets.All, teamNumber);
            }
        }

    }

    [PunRPC]
    private void increment(int teamNumber)
    {
        if (teamNumber == 2)
        {
            scoreRight++;
        }
        else
        {
            scoreLeft++;
        }
        UpdateScoreBoard();
    }

}
