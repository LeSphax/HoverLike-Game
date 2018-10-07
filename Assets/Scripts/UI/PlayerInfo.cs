using Byn.Net;
using PlayerManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public delegate void TeamChangeHandler(PlayerInfo playerInfo, Team newTeam);

[RequireComponent(typeof(AudioSource))]
public class PlayerInfo : SlideBall.MonoBehaviour
{

    [SerializeField]
    private Text playerName;
    [SerializeField]
    private Text latency;

    private Player correspondingPlayer;
    public Player MyPlayer
    {
        get
        {
            return correspondingPlayer;
        }
        set
        {
            correspondingPlayer = value;
            PlayerName = MyPlayer.Nickname;
            if (MyPlayer.Team != Team.NONE)
                CurrentTeam = MyPlayer.Team;
            MyPlayer.eventNotifier.ListenToEvents(ChangeNickname, PlayerFlags.NICKNAME);
            MyPlayer.eventNotifier.ListenToEvents(RefreshTeam, PlayerFlags.TEAM);
            MyPlayer.eventNotifier.ListenToEvents(Destroy, PlayerFlags.DESTROYED);
        }
    }


    public event TeamChangeHandler TeamChanged;

    private Team CurrentTeam
    {
        set
        {
            Assert.IsTrue(value == Team.BLUE || value == Team.RED, "" + (int)value);
            GetComponent<Image>().color = Colors.Teams[(int)value];
            if (TeamChanged != null) TeamChanged.Invoke(this, value);
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
        if (id == MyPlayer.id)
        {
            Latency = latency;
        }
    }

    void Start()
    {
        Latency = 0;
        if (NetworkingState.IsServer)
        {
            MyComponents.TimeManagement.LatencyChanged += SetLatency;
        }
    }

    public void KickPlayer()
    {
        MyComponents.Players.RemovePlayer(MyPlayer.id);
    }

    private void RefreshTeam(Player player)
    {
        Assert.IsTrue(player.id == MyPlayer.id);
        CurrentTeam = MyPlayer.Team;
    }

    private void ChangeNickname(Player player)
    {
        PlayerName = player.Nickname;
    }

    void Destroy(Player player)
    {
        Destroy(gameObject);
        Player.UpdatePlayersDataOnDestroy(player.id, MyComponents);
    }

    void OnDestroy()
    {
        if (MyComponents.TimeManagement != null)
            MyComponents.TimeManagement.LatencyChanged -= SetLatency;
        if (correspondingPlayer != null)
        {
            MyPlayer.eventNotifier.StopListeningToEvents(ChangeNickname, PlayerFlags.NICKNAME);
            MyPlayer.eventNotifier.StopListeningToEvents(RefreshTeam, PlayerFlags.TEAM);
            MyPlayer.eventNotifier.StopListeningToEvents(Destroy, PlayerFlags.DESTROYED);
        }
    }
}
