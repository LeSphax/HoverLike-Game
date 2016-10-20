using UnityEngine;
using System.Collections;

public class Spawns : MonoBehaviour
{

    public GameObject[] team0;
    public GameObject[] team1;

    public GameObject GetSpawn(Team teamNumber, int spawnNumber)
    {
        GameObject[] spawns;
        switch (teamNumber)
        {
            case Team.FIRST:
                spawns = team0;
                break;
            case Team.SECOND:
                spawns = team1;
                break;
            default:
                spawns = team0;
                Debug.LogError("This team number doesn't exist " + teamNumber);
                break;
        }
        return spawns[spawnNumber];
    }


}
