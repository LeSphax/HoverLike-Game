using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
public class Scoreboard : Photon.MonoBehaviour {

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
        Matchmaker.connectedEvent += UpdateScoreBoard;
    }

    [PunRPC]
    public void UpdateScoreBoard()
    {
        textScoreLeft.text = "" + CustomProperties.GetProperty<int>(NetworkRoomKeys.TeamScore[0]);
        textScoreRight.text = "" + CustomProperties.GetProperty<int>(NetworkRoomKeys.TeamScore[1]);
    }

    public static void incrementTeamScore(int teamNumber)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (Time.realtimeSinceStartup - timeLastGoal > 1)
            {
                timeLastGoal = Time.realtimeSinceStartup;
                string key = NetworkRoomKeys.TeamScore[teamNumber];
                int newScore = CustomProperties.GetProperty<int>(key)+1;
                Hashtable someCustomPropertiesToSet = new Hashtable() { { key, newScore}};
                PhotonNetwork.room.SetCustomProperties(someCustomPropertiesToSet);
                scoreboard.GetPhotonView().RPC("UpdateScoreBoard", PhotonTargets.All);
            }
        }

    }

}
