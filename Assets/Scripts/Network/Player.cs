using Byn.Net;
using PlayerBallControl;
using UnityEngine;
using UnityEngine.Assertions;

namespace PlayerManagement
{
    public class Player
    {
        public enum State
        {
            PLAYING,
            FROZEN,
            NO_MOVEMENT,
        }

        public MultipleEvents eventNotifier;

        public delegate void PlayerChangeHandler(Player player);
        public delegate void HasBallChangeHandler(bool hasBall);
        public delegate void StateChangeHandler(State newState);
        public delegate void NicknameChangeHandler(string nickname);
        public delegate void SceneChangeHandler(ConnectionId connectionId, short scene);

        public event HasBallChangeHandler HasBallChanged;

        public PlayerController controller;
        public PlayerBallController ballController;
        public GameObject gameobjectAvatar;

        internal PlayerFlags flagsChanged;

        internal void NotifyTeamChanged()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.TEAM);
        }

        internal void NotifyStateChanged()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.STATE);
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
            return MyComponents.BallState.GetIdOfPlayerOwningBall() == id;
        }

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
                if (MyComponents.NetworkManagement.IsServer)
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
                if (id == Players.myPlayerId)
                    flagsChanged |= PlayerFlags.SCENEID;
                eventNotifier.changedAttributes.Add(PlayerFlags.SCENEID);
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
                if (id == Players.myPlayerId)
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

        internal void Destroy()
        {
            eventNotifier.changedAttributes.Add(PlayerFlags.DESTROYED);
        }

        public bool IsMyPlayer { get { return id == Players.myPlayerId; } }

        public Player(ConnectionId id)
        {
            this.id = id;
            eventNotifier = new MultipleEvents(this);
        }

        public override string ToString()
        {
            return "Player " + (IsMyPlayer ? "(mine) " : "") + id + " Nickname: " + Nickname + " Team : " + Team + "   HasBall : " + hasBall + " Avatar " + AvatarSettingsType + " State " + CurrentState + " SpawningPoint " + SpawningPoint;
        }


    }
}