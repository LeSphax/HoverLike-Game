using Byn.Net;
using Navigation;
using PlayerManagement;
using SlideBall.Networking;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
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
            Assert.IsTrue(value == Team.BLUE || value == Team.RED, "" + (int)value);
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
            //We could avoid sending it to the owner of the view
            View.RPC("RPCSetLatency", MessageFlags.SceneDependant, RPCTargets.Others, latency);
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
        if (View.isMine)
        {
            if (Player.Team != Team.NONE)
                SetInitialTeam(Player.Team);
            else
            {
                Debug.Log(gameObject.name + " InitialTeam");

                View.RPC("GetInitialTeam", RPCTargets.Server, null);
            }
            Debug.Log(gameObject.name + " SendPlayEnterRoom");

            View.RPC("PlayEnterRoomSound", RPCTargets.Others, null);
        }
        if (MyComponents.NetworkManagement.isServer)
        {
            MyComponents.TimeManagement.LatencyChanged += SetLatency;
        }
    }

    [MyRPC]
    private void GetKicked()
    {
        Debug.LogError("Getting kicked");
        Assert.IsTrue(View.isMine);
        MyComponents.PopUp.Show(Language.Instance.texts["Got_Kicked"]);
        MyComponents.RoomManager.GoBack();
    }

    public void KickPlayer()
    {
        View.RPC("GetKicked", Player.id);
    }

    [MyRPC]
    private void PlayEnterRoomSound()
    {
        Debug.Log(gameObject.name + " PlayEnter");
        GetComponent<AudioSource>().Play();
    }

    [MyRPC]
    public void GetInitialTeam(ConnectionId RPCSenderId)
    {
        Assert.IsTrue(MyComponents.NetworkManagement.isServer);
        Team team;
        if (Players.GetPlayersInTeam(Team.BLUE).Count <= Players.GetPlayersInTeam(Team.RED).Count)
            team = Team.BLUE;
        else
            team = Team.RED;
        View.RPC("SetInitialTeam", RPCSenderId, team);
        Players.players[RPCSenderId].Team = team;
    }

    [MyRPC]
    public void SetInitialTeam(Team team)
    {
        Players.MyPlayer.Team = team;
        NavigationManager.ShowLevel();
    }

    public void InitView(object[] parameters)
    {
        playerConnectionId = (ConnectionId)parameters[0];
        if (Player == null)
        {
            Destroy(gameObject);
            return;
        }
        PlayerName = Player.Nickname;
        if (Player.Team != Team.NONE)
            CurrentTeam = Player.Team;
        Player.gameobjectAvatar = gameObject;
        Player.NicknameChanged += ChangeNickname;
        Player.TeamChanged += ChangeTeam;
    }

    private void ChangeTeam(Team newTeam)
    {
        CurrentTeam = newTeam;
    }

    private void ChangeNickname(string nickname)
    {
        PlayerName = nickname;
    }

    void OnDestroy()
    {
        if (MyComponents.TimeManagement != null)
            MyComponents.TimeManagement.LatencyChanged -= SetLatency;
        if (Player != null)
        {
            Player.TeamChanged -= ChangeTeam;
            Player.NicknameChanged -= ChangeNickname;
        }
    }
}
