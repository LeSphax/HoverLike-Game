using System;
using UnityEngine;
using UnityEngine.UI;
public class Scoreboard : SlideBall.NetworkMonoBehaviour
{

    private static int[] scores;

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
                AudioClip but = ResourcesGetter.ButSound;
                myAudio.clip = but;
            }
            return myAudio;
        }
    }

    void Awake()
    {
        scores = new int[2] { 0, 0 };
        scoreboard = gameObject;
        textScoreLeft = transform.GetChild(0).GetComponent<Text>();
        textScoreRight = transform.GetChild(1).gameObject.GetComponent<Text>();
    }

    [MyRPC]
    public void UpdateScoreBoard(int[] scores, bool playAudio)
    {
        Scoreboard.scores = scores;
        textScoreLeft.text = "" + scores[0];
        textScoreRight.text = "" + scores[1];
        if (playAudio)
            Audio.Play();
    }

    public static bool IncrementTeamScore(int teamNumber)
    {
        if (NetworkingState.IsServer)
        {
            if (Time.time - timeLastGoal > 1)
            {
                timeLastGoal = Time.time;
                scores[teamNumber] += 1;
                scoreboard.GetNetworkView().RPC("UpdateScoreBoard", RPCTargets.All, scores, true);
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public static void ResetScore()
    {
        scores = new int[2] { 0, 0 };
        scoreboard.GetNetworkView().RPC("UpdateScoreBoard", RPCTargets.All, scores, false);
    }

    internal static Team GetWinningTeam()
    {
        if (scores[0] > scores[1])
        {
            return Team.RED;
        }
        else if (scores[0] < scores[1])
        {
            return Team.BLUE;
        }
        else
        {
            return Team.NONE;
        }
    }
}
