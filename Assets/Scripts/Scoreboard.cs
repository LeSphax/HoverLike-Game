using UnityEngine;
using UnityEngine.UI;
public class Scoreboard : SlideBall.MonoBehaviour {

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
        MyGameObjects.NetworkManagement.ConnectedToServer += UpdateScoreBoard;
    }

    [MyRPC]
    public void UpdateScoreBoard()
    {
        textScoreLeft.text = "" + MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.TeamScore[0]);
        textScoreRight.text = "" + MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.TeamScore[1]);
    }

    public static void incrementTeamScore(int teamNumber)
    {
        if (MyGameObjects.NetworkManagement.isServer)
        {
            if (Time.realtimeSinceStartup - timeLastGoal > 1)
            {
                timeLastGoal = Time.realtimeSinceStartup;
                string key = PropertiesKeys.TeamScore[teamNumber];
                int newScore = MyGameObjects.Properties.GetProperty<int>(key)+1;
                MyGameObjects.Properties.SetProperty(key, newScore);
                scoreboard.GetNetworkView().RPC("UpdateScoreBoard",RPCTargets.All);
            }
        }

    }

}
