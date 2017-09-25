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
        public static event ConnectionEventHandler NewPlayerInstantiated;
        public static event OwnerChangeHandler PlayerOwningBallChanged;
        public static event EmptyEventHandler PlayersDataReceived;

        public static void NotifyNewPlayerInstantiated(ConnectionId playerId)
        {
            if (NewPlayerInstantiated != null)
            {
                NewPlayerInstantiated.Invoke(playerId);
            }
        }

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

        internal static void SetState(MovementState movementState)
        {
            foreach (Player player in players.Values)
            {
                player.State.Movement = movementState;
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
            if (PlayersDataReceived != null)
                PlayersDataReceived.Invoke();
        }

        private void HandlePlayerPacket(byte[] data, ref int currentIndex)
        {

            short i = BitConverter.ToInt16(data, currentIndex);
            PlayerFlags flags = (PlayerFlags)i;
            currentIndex += 2;
            ConnectionId playerId = new ConnectionId(BitConverter.ToInt16(data, currentIndex));
            currentIndex += 2;
            Player player = GetOrCreatePlayer(playerId);
            //Debug.Log("Receive " + playerId + "    "  + flags);
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
                player.NotifyAvatarSettingsChanged();
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.PLAY_AS_GOALIE))
            {
                player.PlayAsGoalie = BitConverter.ToBoolean(data, currentIndex);
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.MOVEMENT_STATE))
            {
                player.State.movement = (MovementState)data[currentIndex];
                player.NotifyMovementStateChanged();
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.STEALING_STATE))
            {
                player.State.stealing = (StealingState)data[currentIndex];
                player.NotifyStealingStateChanged();
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.IS_HOST))
            {
                player.isHost = BitConverter.ToBoolean(data, currentIndex);
                currentIndex++;
            }
            if (flags.HasFlag(PlayerFlags.NICKNAME))
            {
                short length = BitConverter.ToInt16(data, currentIndex);
                currentIndex += 2;
                player.Nickname = Encoding.UTF8.GetString(data, currentIndex, length);
                player.NotifyNicknameChanged();
                currentIndex += length;
            }
            if (flags.HasFlag(PlayerFlags.DESTROYED))
            {
                RemovePlayer(player.id);
            }
        }

        private static byte[] CreatePlayerPacket(Player player, PlayerFlags flags)
        {
            //Debug.Log("Send " + player.id.id + "    " + flags);
            byte[] id = BitConverter.GetBytes(player.id.id);
            byte[] flagsBytes = BitConverter.GetBytes((short)flags);
            byte[] data = new byte[4] { flagsBytes[0], flagsBytes[1], id[0], id[1] };
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
            if (flags.HasFlag(PlayerFlags.MOVEMENT_STATE))
            {
                data = data.Concatenate(new byte[1] { (byte)player.State.movement });
            }
            if (flags.HasFlag(PlayerFlags.STEALING_STATE))
            {
                data = data.Concatenate(new byte[1] { (byte)player.State.stealing });
            }
            if (flags.HasFlag(PlayerFlags.IS_HOST))
            {
                data = data.Concatenate(BitConverter.GetBytes(player.IsHost));
            }
            if (flags.HasFlag(PlayerFlags.NICKNAME))
            {
                data = data.Concatenate(BitConverter.GetBytes((short)(Encoding.UTF8.GetBytes(player.Nickname).Length)));
                data = data.Concatenate(Encoding.UTF8.GetBytes(player.Nickname));
            }

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

        public static Player CreatePlayer(ConnectionId id)
        {
            Player player;
            player = new Player(id);
            if (players.Count == 0)
                player.IsHost = true;
            players.Add(id, player);
            MyComponents.NetworkManagement.RefreshRoomData();
            if (MyComponents.NetworkManagement.IsServer)
            {
                if (Scenes.IsCurrentScene(Scenes.MainIndex))
                {
                    player.gameobjectAvatar = Functions.InstantiatePlayer(id);
                }
            }

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
            Debug.LogWarning("Send Players data " + recipientId);
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

        internal void RemovePlayer(ConnectionId connectionId)
        {
            if (MyComponents.NetworkManagement.IsServer)
            {
                Player player;
                if (players.TryGetValue(connectionId, out player))
                {
                    if (player.IsHost && players.Count > 1)
                    {
                        List<ConnectionId> remainingPlayers = new List<ConnectionId>(players.Keys);

                        remainingPlayers.Remove(connectionId);
                        remainingPlayers.Sort();
                        players[remainingPlayers[0]].IsHost = true;
                    }
                    player.flagsChanged = PlayerFlags.DESTROYED;
                    player.Destroy();
                    SendChanges();
                }
            }
            else
            {
                if (connectionId == MyPlayer.id)
                {
                    MyComponents.PopUp.Show(Language.Instance.texts["Got_Kicked"]);
                    MyComponents.ResetNetworkComponents();
                    NavigationManager.LoadScene(Scenes.Lobby);
                }
                else
                {
                    players[connectionId].Destroy();
                }
            }


        }

        protected void FixedUpdate()
        {
            SendChanges();
        }

        private void LateUpdate()
        {
            Dictionary<ConnectionId, Player> copy = new Dictionary<ConnectionId, Player>(players);
            copy.Values.ForEach(player => player.eventNotifier.FireEvents());
        }

        public void SendChanges()
        {
            byte[] packet = new byte[0];
            foreach (Player player in new List<Player>(players.Values))
            {
                if (player.flagsChanged != PlayerFlags.NONE)
                    packet = ArrayExtensions.Concatenate(packet, UpdatePlayer(player));
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
    public enum PlayerFlags : short
    {
        NONE = 0,
        PLAY_AS_GOALIE = 1 << 0,
        TEAM = 1 << 1,
        NICKNAME = 1 << 2,
        SPAWNINGPOINT = 1 << 3,
        AVATAR_SETTINGS = 1 << 4,
        MOVEMENT_STATE = 1 << 5,
        SCENEID = 1 << 6,
        IS_HOST = 1 << 7,
        STEALING_STATE = 1 << 8,
        DESTROYED = 1 << 9,
        ALL = ~(~0 << 9),//PLAY_AS_GOALIE + TEAM + NICKNAME + SPAWNINGPOINT + AVATAR_SETTINGS + STATE + SCENEID,
    }
}