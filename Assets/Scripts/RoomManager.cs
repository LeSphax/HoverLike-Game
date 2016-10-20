using Byn.Net;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] teamPanels;

    public void PutPlayerInTeam(PlayerInfo player, Team team)
    {
        //Debug.Log(player + " PutPlayerInTeam " + team);
        player.transform.SetParent(teamPanels[(int)team].transform, false);
    }

    public void ChangeTeam(int teamNumber)
    {
        Players.MyPlayer.Team = (Team)teamNumber;
    }

    public void CreateMyPlayerInfo()
    {
        MyGameObjects.NetworkViewsManagement.Instantiate(Paths.PLAYER_INFO, Vector3.zero, Quaternion.identity, Players.MyPlayer.id).GetComponent<PlayerInfo>();
    }
}