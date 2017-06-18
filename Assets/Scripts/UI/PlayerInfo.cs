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
        }
    }

    void Start()
    {
        Latency = 0;
        if (MyComponents.NetworkManagement.IsServer)
        {
            MyComponents.TimeManagement.LatencyChanged += SetLatency;
        }
    }

    public void KickPlayer()
    {
        Players.Remove(Player.id);
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
