using PlayerManagement;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] teamPanels;
    public GameObject StartButton;

    public void OnEnable()
    {
        if (MyGameObjects.NetworkManagement.isServer)
        {
            StartButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
        }
    }

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

    public void Reset()
    {
        foreach (GameObject teamPanel in teamPanels)
        {
            foreach (Transform child in teamPanel.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}