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

    private int updateLatency = 0;

    private Team CurrentTeam
    {
        set
        {
            GetComponent<Image>().color = Colors.Teams[(int)value];
            RoomManager.Instance.PutPlayerInTeam(this, value);
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
    public float Latency
    {
        set
        {
            latency.text = Mathf.Round(value) + " ms";
        }
    }

    private void SetLatency(ConnectionId id, float latency)
    {
        if (id == playerConnectionId)
        {
            Latency = latency;
            updateLatency++;
            if (updateLatency == 10)
            {
                updateLatency = 0;
                Debug.Log("SendLatency");
                //We could avoid sending it to the owner of the view
                View.RPC("RPCSetLatency", RPCTargets.Others, latency);
            }
        }
    }

    [MyRPC]
    private void RPCSetLatency(float latency)
    {
        Latency = latency;
    }

    void Start()
    {
        Latency = 0;
        Player.gameobjectAvatar = gameObject;
        if (View.isMine)
            View.RPC("GetInitialTeam", RPCTargets.Server, null);
        if (MyComponents.NetworkManagement.isServer)
        {
            MyComponents.TimeManagement.LatencyChanged += SetLatency;
        }
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
        playerConnectionId = (ConnectionId)parameters[0];
        PlayerName = Player.Nickname;
        if (Player.Team != Team.NONE)
            CurrentTeam = Player.Team;
        Player.TeamChanged += (value) => CurrentTeam = value;
    }

    void OnDestroy()
    {
        MyComponents.TimeManagement.LatencyChanged -= SetLatency;
    }
}
