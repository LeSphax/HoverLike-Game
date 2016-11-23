using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;

public class RoomManager : MonoBehaviour
{
    public GameObject[] teamPanels;
    public GameObject StartButton;

    private static RoomManager instance;
    public static RoomManager Instance
    {
        get
        {
            Assert.IsTrue(Scenes.IsCurrentScene(Scenes.LobbyIndex));
            if (instance == null)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Room).GetComponent<RoomManager>();
            }
            return instance;
        }
    }

    public void OnEnable()
    {
        if (MyComponents.NetworkManagement.isServer)
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
        player.transform.SetParent(teamPanels[(int)team].transform, false);
    }

    public void ChangeTeam(int teamNumber)
    {
        Players.MyPlayer.Team = (Team)teamNumber;
    }

    public void CreateMyPlayerInfo()
    {
        MyComponents.NetworkViewsManagement.Instantiate(Paths.PLAYER_INFO, Players.MyPlayer.id).GetComponent<PlayerInfo>();
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

    protected void OnDestroy()
    {
        instance = null;
    }
}