using Byn.Net;
using PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : PlayerView
{

    [SerializeField]
    private Text playerName;
    [SerializeField]
    private Text latency;

    private Team CurrentTeam
    {
        set
        {
            GetComponent<Image>().color = Colors.Teams[(int)value];
            MyGameObjects.RoomManager.PutPlayerInTeam(this, value);
        }
    }

    public string PlayerName
    {
        get
        {
            return playerName.text;
        }
        set
        {
            playerName.text = value;
            gameObject.name = value;
        }
    }
    public int Latency
    {
        set
        {
            latency.text = value + " ms";
        }
    }


    void Start()
    {
        if (View.isMine)
            View.RPC("GetInitialTeam", RPCTargets.Server, null);
    }

    [MyRPC]
    public void GetInitialTeam(ConnectionId RPCSenderId)
    {
        if (Players.GetNumberPlayersInTeam(Team.FIRST) <= Players.GetNumberPlayersInTeam(Team.SECOND))
            View.RPC("SetInitialTeam", RPCSenderId, Team.FIRST);
        else
            View.RPC("SetInitialTeam", RPCSenderId, Team.SECOND);
    }

    [MyRPC]
    public void SetInitialTeam(Team team)
    {
        Debug.Log(this + " SetInitialTeam " + team);
        Players.MyPlayer.Team = team;
    }

    public void InitView(object[] parameters)
    {
        connectionId = (ConnectionId)parameters[0];
        PlayerName = Player.Nickname;
        if (Player.Team != Team.NONE)
            CurrentTeam = Player.Team;
        Player.NickNameChanged += (value) => PlayerName = value;
        Player.TeamChanged += (value) => CurrentTeam = value;
    }
}
