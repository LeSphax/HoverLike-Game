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

        private bool ballOwnerChanged;

        private ConnectionId IdPlayerOwningBall
        {
            get
            {
                if (MyComponents.BallState != null)
                {
                    return MyComponents.BallState.IdPlayerOwningBall;
                }
                return BallState.NO_PLAYER_ID;
            }
        }

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
        }

        public void PlayerOwningBallChanged(ConnectionId previousPlayer, ConnectionId newPlayer, bool sendUpdate)
        {
            ballOwnerChanged = sendUpdate;

            if (newPlayer != previousPlayer)
            {
                if (previousPlayer != BallState.NO_PLAYER_ID)
                    players[previousPlayer].HasBall = false;
                if (newPlayer != BallState.NO_PLAYER_ID)
                {
                    players[newPlayer].HasBall = true;
                    MyComponents.BallState.AttachBall(newPlayer);
                }
                else
                {
                    MyComponents.BallState.AttachBall(BallState.NO_PLAYER_ID);
                }
            }
        }


        public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
        {
            int currentIndex = 0;
            if (MyComponents.BallState != null)
                MyComponents.BallState.SetAttached(new ConnectionId(BitConverter.ToInt16(message.data, currentIndex)), false);
            currentIndex += 2;
            while (message.data.Length > currentIndex)
                HandlePlayerPacket(message.data, ref currentIndex);
        }

        private static void HandlePlayerPacket(byte[] data, ref int currentIndex)
        {

            PlayerFlags flags = (PlayerFlags)data[currentIndex];
            ConnectionId playerId = new ConnectionId(BitConverter.ToInt16(data, currentIndex + 1));
            currentIndex += 3;
            Player player = GetOrCreatePlayer(playerId);
            if (flags.HasFlag(PlayerFlags.TEAM))
            {
                player.Team = (Team)data[currentIndex];
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.SPAWNINGPOINT))
            {
                player.spawnNumber = BitConverter.ToInt16(data, currentIndex);
                currentIndex += 2;
            }
            if (flags.HasFlag(PlayerFlags.SCENEID))
            {
                player.SceneId = BitConverter.ToInt16(data, currentIndex);
                currentIndex += 2;
            }
            if (flags.HasFlag(PlayerFlags.AVATAR_SETTINGS))
            {
                player.avatarSettingsType = (AvatarSettings.AvatarSettingsTypes)data[currentIndex];
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.PLAY_AS_GOALIE))
            {
                player.PlayAsGoalie = BitConverter.ToBoolean(data, currentIndex);
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.STATE))
            {
                player.state = (Player.State)data[currentIndex];
                player.NotifyStateChanged();
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.NICKNAME))
            {
                short length = BitConverter.ToInt16(data, currentIndex);
                currentIndex += 2;
                player.Nickname = Encoding.UTF8.GetString(data, currentIndex, length);
                currentIndex += length;
            }
            if (flags.HasFlag(PlayerFlags.DESTROYED))
            {
                Remove(player.id);
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

        public byte[] UpdatePlayer(Player player)
        {
            byte[] data = CreatePlayerPacket(player, player.flagsChanged);
            player.flagsChanged = PlayerFlags.NONE;
            return data;
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
                data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes((short)(Encoding.UTF8.GetBytes(player.Nickname).Length)));
                data = ArrayExtensions.Concatenate(data, Encoding.UTF8.GetBytes(player.Nickname));
            }
            //Debug.LogError("CreatePacket " + flags + "   " + data.Length);
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

        public void SendPlayersData(ConnectionId recipientId)
        {
            byte[] packet = BitConverter.GetBytes(IdPlayerOwningBall.id);
            foreach (Player player in players.Values)
                packet = ArrayExtensions.Concatenate(packet, CreatePlayerPacket(player, PlayerFlags.ALL));
            MyComponents.NetworkManagement.SendData(ViewId, MessageType.Properties, packet, recipientId);
        }

        protected override void Start()
        {
            base.Start();
            Reset();
        }

        internal static void Remove(ConnectionId connectionId)
        {
            Destroy(players[connectionId].gameobjectAvatar);
            if (MyComponents.NetworkManagement.isServer)
            {
                players[connectionId].flagsChanged = PlayerFlags.DESTROYED;
            }
            else
            {
                players.Remove(connectionId);
            }

        }

        protected void FixedUpdate()
        {
            SendChanges();
        }

        public void SendChanges()
        {
            byte[] packet = new byte[0];
            foreach (Player player in new List<Player>(players.Values))
            {
                bool destroyPlayer = player.flagsChanged.HasFlag(PlayerFlags.DESTROYED);
                if (player.flagsChanged != PlayerFlags.NONE)
                    packet = ArrayExtensions.Concatenate(packet, UpdatePlayer(player));
                if (destroyPlayer)
                {
                    players.Remove(player.id);
                }
            }
            if (packet.Length > 0 || ballOwnerChanged)
            {
                packet = ArrayExtensions.Concatenate(BitConverter.GetBytes(IdPlayerOwningBall.id), packet);
                ballOwnerChanged = false;
                MyComponents.NetworkManagement.SendData(ViewId, MessageType.Properties, packet);
            }
        }
    }
    [Flags]
    public enum PlayerFlags
    {
        NONE = 0,
        PLAY_AS_GOALIE = 1 << 0,
        TEAM = 1 << 1,
        NICKNAME = 1 << 2,
        SPAWNINGPOINT = 1 << 3,
        AVATAR_SETTINGS = 1 << 4,
        STATE = 1 << 5,
        SCENEID = 1 << 6,
        DESTROYED = 1 << 7,
        ALL = ~(~0 << 7),//PLAY_AS_GOALIE + TEAM + NICKNAME + SPAWNINGPOINT + AVATAR_SETTINGS + STATE + SCENEID,
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

        internal PlayerFlags flagsChanged;

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
                    flagsChanged |= PlayerFlags.TEAM;
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
                    flagsChanged |= PlayerFlags.NICKNAME;
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
                flagsChanged |= PlayerFlags.SPAWNINGPOINT;
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
                    flagsChanged |= PlayerFlags.SCENEID;
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
                flagsChanged |= PlayerFlags.STATE;
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
                flagsChanged |= PlayerFlags.AVATAR_SETTINGS;
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
                    flagsChanged |= PlayerFlags.PLAY_AS_GOALIE;
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