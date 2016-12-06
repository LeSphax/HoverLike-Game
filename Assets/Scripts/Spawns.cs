using UnityEngine;
using System.Collections;

public class Spawns : MonoBehaviour
{

    [SerializeField]
    private GameObject[] team0;
    [SerializeField]
    private GameObject[] team1;
    [SerializeField]
    private GameObject ballSpawn;

    public Vector3 BallSpawn
    {
        get
        {
            return ballSpawn.transform.position;
        }
    }

    public Vector3 GetSpawn(Team teamNumber, int spawnNumber)
    {
        GameObject[] spawns;
        switch (teamNumber)
        {
            case Team.BLUE:
                spawns = team0;
                break;
            case Team.RED:
                spawns = team1;
                break;
            default:
                spawns = team0;
                Debug.LogError("This team number doesn't exist " + teamNumber);
                break;
        }
        return spawns[spawnNumber].transform.position;
    }


}
