using UnityEngine;
using UnityEngine.UI;
public class Scoreboard : SlideBall.MonoBehaviour
{

    private static Text textScoreLeft;
    private static Text textScoreRight;

    private static GameObject scoreboard;

    private static float timeLastGoal = 0;

    private AudioSource myAudio;
    private AudioSource Audio
    {
        get
        {
            if (myAudio == null)
            {
                myAudio = GetComponent<AudioSource>();
                AudioClip but = Resources.Load<AudioClip>("Audio/But");
                myAudio.clip = but;
            }
            return myAudio;
        }
    }

    void Awake()
    {
        scoreboard = gameObject;
        textScoreLeft = transform.GetChild(0).GetComponent<Text>();
        textScoreRight = transform.GetChild(1).gameObject.GetComponent<Text>();
    }

    [MyRPC]
    public void UpdateScoreBoard(bool playAudio)
    {
        textScoreLeft.text = "" + MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.TeamScore[0]);
        textScoreRight.text = "" + MyGameObjects.Properties.GetProperty<int>(PropertiesKeys.TeamScore[1]);
        if (playAudio)
            Audio.Play();
    }

    public static void IncrementTeamScore(int teamNumber)
    {
        if (MyGameObjects.NetworkManagement.isServer)
        {
            if (Time.realtimeSinceStartup - timeLastGoal > 1)
            {
                timeLastGoal = Time.realtimeSinceStartup;
                string key = PropertiesKeys.TeamScore[teamNumber];
                int newScore = MyGameObjects.Properties.GetProperty<int>(key) + 1;
                MyGameObjects.Properties.SetProperty(key, newScore);
                scoreboard.GetNetworkView().RPC("UpdateScoreBoard", RPCTargets.All, true);
            }
        }

    }

    public static void ResetScore()
    {
        MyGameObjects.Properties.SetProperty(PropertiesKeys.TeamScore[(int)Team.FIRST], 0);
        MyGameObjects.Properties.SetProperty(PropertiesKeys.TeamScore[(int)Team.SECOND], 0);
        scoreboard.GetNetworkView().RPC("UpdateScoreBoard", RPCTargets.All, false);
    }

}
