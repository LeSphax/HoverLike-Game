using System;
using System.Collections.Generic;
using Byn.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using PlayerBallControl;

namespace PlayerManagement
{
    public class Players : ANetworkView
    {
        public static ConnectionId INVALID_PLAYER_ID = new ConnectionId(-100);

        //The server is the only one having access to this dictionnary
        public static Dictionary<ConnectionId, Player> players = new Dictionary<ConnectionId, Player>();

        public static event ConnectionEventHandler NewPlayerCreated;

        //This variable only exists on client, on the server it is contained in the dictionary 'players'
        public static ConnectionId myPlayerId;
        public static Player MyPlayer
        {
            get
            {
                Player player;
                if (players.TryGetValue(myPlayerId, out player))
                    return player;
                return null;
            }
        }

        internal static void SetState(Player.State state)
        {
            foreach (Player player in players.Values)
            {
                player.CurrentState = state;
            }
        }

        public static void Reset()
        {
            myPlayerId = INVALID_PLAYER_ID;
            NewPlayerCreated = null;
            players.Clear();
            MyComponents.Properties.AddListener(PropertiesKeys.IdPlayerOwningBall, MyComponents.Players.PlayerOwningBallChanged);
        }

        public void PlayerOwningBallChanged(object previousPlayer, object newPlayer)
        {
            ConnectionId newPlayerId = newPlayer == null ? BallState.NO_PLAYER_ID : (ConnectionId)newPlayer;
            ConnectionId previousPlayerId = previousPlayer == null ? BallState.NO_PLAYER_ID : (ConnectionId)previousPlayer;

            if (newPlayerId != previousPlayerId)
            {
                if (previousPlayerId != BallState.NO_PLAYER_ID)
                    players[previousPlayerId].HasBall = false;
                if (newPlayerId != BallState.NO_PLAYER_ID)
                {
                    players[newPlayerId].HasBall = true;
                    MyComponents.BallState.AttachBall(newPlayerId);
                }
                else
                {
                    MyComponents.BallState.AttachBall(BallState.NO_PLAYER_ID);
                }
            }
        }


        public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
        {
            int currentIndex = 3;
            PlayerFlags flags = (PlayerFlags)message.data[0];
            ConnectionId playerId = new ConnectionId(BitConverter.ToInt16(message.data, 1));
            Player player = GetOrCreatePlayer(playerId);
            if (flags.HasFlag(PlayerFlags.TEAM))
            {
                player.Team = (Team)message.data[currentIndex];
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.SPAWNINGPOINT))
            {
                player.spawnNumber = BitConverter.ToInt16(message.data, currentIndex);
                currentIndex += 2;
            }
            if (flags.HasFlag(PlayerFlags.SCENEID))
            {
                player.SceneId = BitConverter.ToInt16(message.data, currentIndex);
                currentIndex += 2;
            }
            if (flags.HasFlag(PlayerFlags.AVATAR_SETTINGS))
            {
                player.avatarSettingsType = (AvatarSettings.AvatarSettingsTypes)message.data[currentIndex];
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.PLAY_AS_GOALIE))
            {
                player.PlayAsGoalie = BitConverter.ToBoolean(message.data, currentIndex);
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.STATE))
            {
                player.state = (Player.State)message.data[currentIndex];
                player.NotifyStateChanged();
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.NICKNAME))
            {
                player.Nickname = Encoding.UTF8.GetString(message.data, currentIndex, message.data.Length - currentIndex);
            }
        }

        private static Player GetOrCreatePlayer(ConnectionId id)
        {
            Player player;
            if (!players.TryGetValue(id, out player))
            {
                player = CreatePlayer(id);
            }

            return player;
        }

        public static Player CreatePlayer(ConnectionId id)
        {
            Player player;
            player = new Player(id);
            players.Add(id, player);
            if (NewPlayerCreated != null)
                NewPlayerCreated.Invoke(id);
            return player;
        }

        public void UpdatePlayer(Player player, PlayerFlags flags)
        {
            byte[] data = CreatePlayerPacket(player, flags);
            MyComponents.NetworkManagement.SendData(ViewId, MessageType.Properties, data);
        }

        public void UpdatePlayer(Player player, PlayerFlags flags, ConnectionId recipientId)
        {
            byte[] data = CreatePlayerPacket(player, flags);
            MyComponents.NetworkManagement.SendData(ViewId, MessageType.Properties, data, recipientId);
        }

        private static byte[] CreatePlayerPacket(Player player, PlayerFlags flags)
        {
            byte[] id = BitConverter.GetBytes(player.id.id);
            byte[] data = new byte[3] { (byte)flags, id[0], id[1] };
            if (flags.HasFlag(PlayerFlags.TEAM))
            {
                data = ArrayExtensions.Concatenate(data, new byte[1] { (byte)player.Team });
            }
            if (flags.HasFlag(PlayerFlags.SPAWNINGPOINT))
            {
                data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes(player.SpawnNumber));
            }
            if (flags.HasFlag(PlayerFlags.SCENEID))
            {
                data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes(player.SceneId));
            }
            if (flags.HasFlag(PlayerFlags.AVATAR_SETTINGS))
            {
                data = ArrayExtensions.Concatenate(data, new byte[1] { (byte)player.AvatarSettingsType });
            }
            if (flags.HasFlag(PlayerFlags.PLAY_AS_GOALIE))
            {
                data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes(player.PlayAsGoalie));
            }
            if (flags.HasFlag(PlayerFlags.STATE))
            {
                data = ArrayExtensions.Concatenate(data, new byte[1] { (byte)player.CurrentState });
            }
            if (flags.HasFlag(PlayerFlags.NICKNAME))
            {
                data = ArrayExtensions.Concatenate(data, Encoding.UTF8.GetBytes(player.Nickname));
            }
            return data;
        }

        public static List<Player> GetPlayersInTeam(Team team)
        {
            List<Player> result = new List<Player>();
            foreach (var player in players.Values)
            {
                if (player.Team == team)
                {
                    result.Add(player);
                }
            }
            return result;
        }

        public void SendProperties(ConnectionId recipientId)
        {
            foreach (Player player in players.Values)
                UpdatePlayer(player, PlayerFlags.NICKNAME | PlayerFlags.TEAM, recipientId);
        }

        protected override void Start()
        {
            base.Start();
            MyComponents.NetworkManagement.NewPlayerConnectedToRoom += SendProperties;
            Reset();
        }

        internal static void Remove(ConnectionId connectionId)
        {
            Assert.IsTrue(MyComponents.NetworkManagement.isServer);
            Destroy(players[connectionId].gameobjectAvatar);
            players.Remove(connectionId);
        }
    }
    [Flags]
    public enum PlayerFlags
    {
        TEAM = 2,
        NICKNAME = 4,
        SPAWNINGPOINT = 8,
        AVATAR_SETTINGS = 16,
        STATE = 32,
        SCENEID = 64,
        PLAY_AS_GOALIE = 128,
    }
    public class Player
    {
        public enum State
        {
            CONNECTED,
            READY,
            PLAYING,
            FROZEN,
            NO_MOVEMENT,
        }

        public delegate void TeamChangeHandler(Team team);
        public delegate void HasBallChangeHandler(bool hasBall);
        public delegate void StateChangeHandler(State newState);
        public delegate void NicknameChangeHandler(string nickname);
        public delegate void SceneChangeHandler(ConnectionId connectionId, short scene);


        public event TeamChangeHandler TeamChanged;
        public event SceneChangeHandler SceneChanged;
        public event StateChangeHandler StateChanged;
        public event HasBallChangeHandler HasBallChanged;

        public PlayerController controller;
        public PlayerBallController ballController;
        public GameObject gameobjectAvatar;

        internal void NotifyTeamChanged()
        {
            if (TeamChanged != null)
                TeamChanged.Invoke(team);
        }

        internal void NotifySceneChanged()
        {
            if (SceneChanged != null)
                SceneChanged.Invoke(id, sceneId);
        }

        internal void NotifyStateChanged()
        {
            if (StateChanged != null)
                StateChanged.Invoke(state);
        }

        internal bool IsHoldingBall()
        {
            return MyComponents.BallState.GetIdOfPlayerOwningBall() == id;
        }

        public ConnectionId id;
        public bool isReady = false;

        internal Team team = Team.NONE;

        public Team Team
        {
            get
            {
                return team;
            }
            set
            {
                team = value;
                if (id == Players.myPlayerId)
                    MyComponents.Players.UpdatePlayer(this, PlayerFlags.TEAM);
                NotifyTeamChanged();
            }
        }
        internal string nickname;
        public string Nickname
        {
            get
            {
                return nickname;
            }
            set
            {
                nickname = value;
                if (id == Players.myPlayerId)
                    MyComponents.Players.UpdatePlayer(this, PlayerFlags.NICKNAME);
            }
        }

        internal short spawnNumber;
        public short SpawnNumber
        {
            get
            {
                return spawnNumber;
            }
            set
            {
                spawnNumber = value;
                MyComponents.Players.UpdatePlayer(this, PlayerFlags.SPAWNINGPOINT);
            }
        }

        internal short sceneId;
        public short SceneId
        {
            get
            {
                return sceneId;
            }
            set
            {
                sceneId = value;
                if (id == Players.myPlayerId)
                    MyComponents.Players.UpdatePlayer(this, PlayerFlags.SCENEID);
                NotifySceneChanged();
            }
        }

        internal State state;
        public State CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                MyComponents.Players.UpdatePlayer(this, PlayerFlags.STATE);
                NotifyStateChanged();
            }
        }

        internal AvatarSettings.AvatarSettingsTypes avatarSettingsType;
        public AvatarSettings.AvatarSettingsTypes AvatarSettingsType
        {
            get
            {
                return avatarSettingsType;
            }
            set
            {
                avatarSettingsType = value;
                MyComponents.Players.UpdatePlayer(this, PlayerFlags.AVATAR_SETTINGS);
            }
        }

        internal bool playAsGoalie;
        public bool PlayAsGoalie
        {
            get
            {
                return playAsGoalie;
            }
            set
            {
                playAsGoalie = value;
                if (id == Players.myPlayerId)
                    MyComponents.Players.UpdatePlayer(this, PlayerFlags.PLAY_AS_GOALIE);
            }
        }

        public AvatarSettings MyAvatarSettings
        {
            get
            {
                return AvatarSettings.Data[avatarSettingsType];
            }
        }

        public Vector3 SpawningPoint
        {
            get
            {
                return MyComponents.Spawns.GetSpawn(Team, SpawnNumber);
            }
        }

        private bool hasBall;
        public bool HasBall
        {
            get
            {
                return hasBall;
            }
            set
            {
                hasBall = value;
                if (HasBallChanged != null)
                    HasBallChanged.Invoke(value);
            }
        }

        public bool IsMyPlayer { get { return id == Players.myPlayerId; } }

        public Player(ConnectionId id)
        {
            this.id = id;
        }
    }



}