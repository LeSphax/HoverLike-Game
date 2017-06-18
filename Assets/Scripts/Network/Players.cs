using System;
using System.Collections.Generic;
using Byn.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using PlayerBallControl;
using SlideBall.Networking;
using Ball;
using Navigation;

namespace PlayerManagement
{
    public delegate void OwnerChangeHandler(ConnectionId previousPlayer, ConnectionId newPlayer);


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
        public static event OwnerChangeHandler PlayerOwningBallChanged;

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

        public void ChangePlayerOwningBall(ConnectionId previousPlayer, ConnectionId newPlayer, bool sendUpdate)
        {
            //Debug.LogError("PlayerOwningBallChanged " + previousPlayer + "   " + newPlayer);
            if (MyComponents.NetworkManagement.IsServer)
                ballOwnerChanged = sendUpdate;
            if (newPlayer != previousPlayer)
            {
                if (previousPlayer != BallState.NO_PLAYER_ID)
                {
                    players[previousPlayer].HasBall = false;
                    players[previousPlayer].controller.animator.SetBool("Catch", false);
                }
                if (newPlayer != BallState.NO_PLAYER_ID)
                {
                    players[newPlayer].HasBall = true;
                    MyComponents.BallState.AttachBall(newPlayer);
                    players[newPlayer].controller.animator.SetBool("Catch", true);
                }
                else
                {
                    MyComponents.BallState.AttachBall(BallState.NO_PLAYER_ID);
                }
                if (PlayerOwningBallChanged != null)
                    PlayerOwningBallChanged.Invoke(previousPlayer, newPlayer);
            }
        }


        public override void ReceiveNetworkMessage(ConnectionId id, NetworkMessage message)
        {
            int currentIndex = 0;
            bool hasBallOwnerChanged = BitConverter.ToBoolean(message.data, currentIndex);
            currentIndex++;
            if (hasBallOwnerChanged)
            {
                Assert.IsTrue(!MyComponents.NetworkManagement.IsServer);
                if (MyComponents.BallState != null)
                    MyComponents.BallState.SetAttached(new ConnectionId(BitConverter.ToInt16(message.data, currentIndex)), false);
                currentIndex += 2;
            }
            while (message.data.Length > currentIndex)
                HandlePlayerPacket(message.data, ref currentIndex);
        }

        private static void HandlePlayerPacket(byte[] data, ref int currentIndex)
        {
            PlayerFlags flags = (PlayerFlags)data[currentIndex];
            ConnectionId playerId = new ConnectionId(BitConverter.ToInt16(data, currentIndex + 1));
            currentIndex += 3;
            Player player = GetOrCreatePlayer(playerId);
            //Debug.LogError("Receive Packet " + flags + "   " + currentIndex);
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

        private static byte[] CreatePlayerPacket(Player player, PlayerFlags flags)
        {
            byte[] id = BitConverter.GetBytes(player.id.id);
            byte[] data = new byte[3] { (byte)flags, id[0], id[1] };
            //Debug.LogError("Create Packet " + flags);
            if (flags.HasFlag(PlayerFlags.TEAM))
            {
                data = data.Concatenate(new byte[1] { (byte)player.Team });
            }
            if (flags.HasFlag(PlayerFlags.SPAWNINGPOINT))
            {
                data = data.Concatenate(BitConverter.GetBytes(player.SpawnNumber));
            }
            if (flags.HasFlag(PlayerFlags.SCENEID))
            {
                data = data.Concatenate(BitConverter.GetBytes(player.SceneId));
            }
            if (flags.HasFlag(PlayerFlags.AVATAR_SETTINGS))
            {
                data = data.Concatenate(new byte[1] { (byte)player.AvatarSettingsType });
            }
            if (flags.HasFlag(PlayerFlags.PLAY_AS_GOALIE))
            {
                data = data.Concatenate(BitConverter.GetBytes(player.PlayAsGoalie));
            }
            if (flags.HasFlag(PlayerFlags.STATE))
            {
                data = data.Concatenate(new byte[1] { (byte)player.CurrentState });
            }
            if (flags.HasFlag(PlayerFlags.NICKNAME))
            {
                data = data.Concatenate(BitConverter.GetBytes((short)(Encoding.UTF8.GetBytes(player.Nickname).Length)));
                data = data.Concatenate(Encoding.UTF8.GetBytes(player.Nickname));
            }
            //Debug.LogError("CreatePacket " + flags + "   " + data.Length);
            return data;
        }

        private static Player GetOrCreatePlayer(ConnectionId id)
        {
            Player player;
            if (!players.TryGetValue(id, out player))
            {
                player = CreatePlayer(id);
                MyComponents.GlobalSound.Play(ResourcesGetter.JoinRoomSound);
            }

            return player;
        }

        private static Team GetInitialTeam()
        {
            Assert.IsTrue(MyComponents.NetworkManagement.IsServer);
            Team team;
            if (Players.GetPlayersInTeam(Team.BLUE).Count <= Players.GetPlayersInTeam(Team.RED).Count)
                team = Team.BLUE;
            else
                team = Team.RED;
            return team;
        }

        public static Player CreatePlayer(ConnectionId id)
        {
            Player player;
            player = new Player(id);
            players.Add(id, player);
            if (NewPlayerCreated != null)
                NewPlayerCreated.Invoke(id);
            MyComponents.NetworkManagement.SetNumberPlayersInSignaling();
            player.Team = GetInitialTeam();
            return player;
        }

        public byte[] UpdatePlayer(Player player)
        {
            byte[] data = CreatePlayerPacket(player, player.flagsChanged);
            player.flagsChanged = PlayerFlags.NONE;
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
            Assert.IsTrue(MyComponents.NetworkManagement.IsServer);
            byte[] packet = BitConverter.GetBytes(false);
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
            Debug.Log("Player with id " + connectionId + " has left");
            Destroy(players[connectionId].gameobjectAvatar);
            if (MyComponents.NetworkManagement.IsServer)
            {
                players[connectionId].flagsChanged = PlayerFlags.DESTROYED;
            }
            else
            {
                players.Remove(connectionId);
                if (connectionId == MyPlayer.id)
                {
                    MyComponents.PopUp.Show(Language.Instance.texts["Got_Kicked"]);
                    MyComponents.ResetNetworkComponents();
                    NavigationManager.LoadScene(Scenes.Lobby);
                }
            }
            MyComponents.NetworkManagement.SetNumberPlayersInSignaling();
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
            if (ballOwnerChanged)
            {
                packet = ArrayExtensions.Concatenate(BitConverter.GetBytes(IdPlayerOwningBall.id), packet);
            }
            if (packet.Length > 0)
            {
                packet = ArrayExtensions.Concatenate(BitConverter.GetBytes(ballOwnerChanged), packet);
                Assert.IsTrue(BitConverter.ToBoolean(packet, 0) == ballOwnerChanged);
                if (ballOwnerChanged)
                    Assert.IsTrue(BitConverter.ToInt16(packet, 1) == IdPlayerOwningBall.id);
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
}