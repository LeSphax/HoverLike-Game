using System;
using System.Collections.Generic;
using Byn.Net;
using System.Text;
using UnityEngine;

namespace PlayerManagement
{
    public class Players : ANetworkView
    {
        //The server is the only one having access to this dictionnary
        public static Dictionary<ConnectionId, Player> players = new Dictionary<ConnectionId, Player>();

        //This variable only exists on client, on the server it is contained in the dictionary 'players'
        public static ConnectionId myPlayerId;
        public static Player MyPlayer
        {
            get
            {
                return GetOrCreatePlayer(myPlayerId);
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
                short walla = BitConverter.ToInt16(message.data, currentIndex);
                Debug.LogError("Get Spahdfofh " + walla);
                player.spawningPoint = walla;
                currentIndex += 2;
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
                player = new Player(id);
                players.Add(id, player);
            }

            return player;
        }

        public void UpdatePlayer(Player player, PlayerFlags flags)
        {
            byte[] data = CreatePlayerPacket(player, flags);
            MyGameObjects.NetworkManagement.SendData(ViewId, MessageType.Properties, data);
        }

        public void UpdatePlayer(Player player, PlayerFlags flags, ConnectionId recipientId)
        {
            byte[] data = CreatePlayerPacket(player, flags);
            MyGameObjects.NetworkManagement.SendData(ViewId, MessageType.Properties, data, recipientId);
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
                data = ArrayExtensions.Concatenate(data, BitConverter.GetBytes(player.SpawningPoint));
            }
            if (flags.HasFlag(PlayerFlags.NICKNAME))
            {
                data = ArrayExtensions.Concatenate(data, Encoding.UTF8.GetBytes(player.Nickname));
            }
            return data;
        }


        public static int GetNumberPlayersInTeam(Team team)
        {
            int result = 0;
            foreach (var player in players.Values)
            {
                if (player.Team == team)
                {
                    result++;
                }
            }
            return result;
        }

        public void SendProperties(ConnectionId recipientId)
        {
            Debug.Log("This could be optimized into a single packet");
            foreach (Player player in players.Values)
                UpdatePlayer(player, PlayerFlags.NICKNAME | PlayerFlags.TEAM, recipientId);
        }

        protected override void Start()
        {
            base.Start();
            MyGameObjects.NetworkManagement.NewPlayerConnectedToRoom += SendProperties;
        }

    }
    [Flags]
    public enum PlayerFlags
    {
        TEAM = 2,
        NICKNAME = 4,
        SPAWNINGPOINT = 8,
    }
    public class Player
    {
        public delegate void TeamChangeHandler(Team team);
        public delegate void NicknameChangeHandler(string nickname);


        public event TeamChangeHandler TeamChanged;
        public event NicknameChangeHandler NickNameChanged;

        public ConnectionId id;
        public bool isReady = false;
        private Team team = Team.NONE;
        public Team Team
        {
            get
            {
                return team;
            }
            set
            {
                team = value;
                if (id == Players.MyPlayer.id)
                    MyGameObjects.Players.UpdatePlayer(this, PlayerFlags.TEAM);
                if (TeamChanged != null)
                    TeamChanged.Invoke(value);
            }
        }
        private string nickname;
        public string Nickname
        {
            get
            {
                return nickname;
            }
            set
            {
                nickname = value;
                if (id == Players.MyPlayer.id)
                    MyGameObjects.Players.UpdatePlayer(this, PlayerFlags.NICKNAME);
                if (NickNameChanged != null)
                    NickNameChanged.Invoke(value);
            }
        }

        internal short spawningPoint;
        public short SpawningPoint
        {
            get
            {
                return spawningPoint;
            }
            set
            {
                Debug.LogError("Set Spawning point " + value);
                spawningPoint = value;
                MyGameObjects.Players.UpdatePlayer(this, PlayerFlags.SPAWNINGPOINT);
            }
        }

        public Player(ConnectionId id)
        {
            this.id = id;
        }
    }



}