using Byn.Net;
using PlayerBallControl;
using UnityEngine;
using UnityEngine.Assertions;

namespace PlayerManagement
{
    public class Player
    {
        public MultipleEvents eventNotifier;

        public delegate void PlayerChangeHandler(Player player);
        public delegate void HasBallChangeHandler(bool hasBall);
        public delegate void IsHostChangeHandler(bool hasBall);
        public delegate void StateChangeHandler(PlayerState newState);
        public delegate void NicknameChangeHandler(string nickname);
        public delegate void SceneChangeHandler(ConnectionId connectionId, short scene);

        public event HasBallChangeHandler HasBallChanged;
        public event IsHostChangeHandler IsHostChanged;

        public InputManager InputManager
        {
            get
            {
                return controller.inputManager;
            }
        }
        public PlayerController controller;
        public PlayerBallController ballController;
        public GameObject gameobjectAvatar;

        internal PlayerFlags flagsChanged;

        internal void NotifyTeamChanged()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.TEAM);
        }

        internal void NotifyMovementStateChanged()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.MOVEMENT_STATE);
        }

        internal void NotifyStealingStateChanged()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.STEALING_STATE);
        }

        internal void NotifyAvatarSettingsChanged()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.AVATAR_SETTINGS);

        }

        internal void NotifyNicknameChanged()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.NICKNAME);
        }

        internal bool IsHoldingBall()
        {
            return myComponents.BallState.GetIdOfPlayerOwningBall() == id;
        }

        public MyComponents myComponents;
        public ConnectionId id;
        public bool isReady = false;

        internal Team team = Team.NONE;

        public void ChangeTeam(Team team)
        {
            this.team = team;
            flagsChanged |= PlayerFlags.TEAM;
            NotifyTeamChanged();
        }

        public Team Team
        {
            get
            {
                return team;
            }
            set
            {
                team = value;
                if (NetworkingState.IsServer)
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
                if (id == myComponents.Players.myPlayerId)
                    flagsChanged |= PlayerFlags.NICKNAME;
                NotifyNicknameChanged();
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

        internal short sceneId = -1;
        public short SceneId
        {
            get
            {
                return sceneId;
            }
            set
            {
                sceneId = value;
                if (id == myComponents.Players.myPlayerId)
                    flagsChanged |= PlayerFlags.SCENEID;
                eventNotifier.changedAttributes.Add(PlayerFlags.SCENEID);
            }
        }

        public PlayerState State;

        internal AvatarSettings.AvatarSettingsTypes avatarSettingsType = AvatarSettings.AvatarSettingsTypes.NONE;
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
                NotifyAvatarSettingsChanged();
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
                if (id == myComponents.Players.myPlayerId)
                    flagsChanged |= PlayerFlags.PLAY_AS_GOALIE;
                eventNotifier.changedAttributes.Add(PlayerFlags.PLAY_AS_GOALIE);
            }
        }

        public AvatarSettings MyAvatarSettings
        {
            get
            {
                Assert.IsTrue(avatarSettingsType != AvatarSettings.AvatarSettingsTypes.NONE);
                return AvatarSettings.Data[avatarSettingsType];
            }
        }

        public Vector3 SpawningPoint
        {
            get
            {
                return myComponents.Spawns.GetSpawn(Team, SpawnNumber);
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

        internal bool isHost;
        public bool IsHost
        {
            get
            {
                return isHost;
            }
            set
            {
                isHost = value;
                flagsChanged |= PlayerFlags.IS_HOST;
                if (IsHostChanged != null)
                    IsHostChanged.Invoke(value);
            }
        }

        internal void Destroy()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.DESTROYED);
        }

        public bool IsMyPlayer { get { return id == myComponents.Players.myPlayerId; } }

        public Player(ConnectionId id, MyComponents myComponents)
        {
            this.myComponents = myComponents;
            this.id = id;
            eventNotifier = new MultipleEvents(this);
            State = new PlayerState(this, MovementState.FROZEN, StealingState.IDLE);
        }

        public override string ToString()
        {
            return "Player " + (IsMyPlayer ? "(mine) " : "") + id + " Nickname: " + Nickname + " Team : " + Team + "   HasBall : " + hasBall + " Avatar " + AvatarSettingsType + " State " + State + " SpawningPoint " + SpawningPoint;
        }

        public static void UpdatePlayersDataOnDestroy(ConnectionId id, MyComponents myComponents)
        {
            Debug.LogWarning("Remove player" + id);
            myComponents.Players.players.Remove(id);
            myComponents.NetworkManagement.RefreshRoomData();
        }

    }
}