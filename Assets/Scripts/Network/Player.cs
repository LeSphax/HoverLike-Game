using Byn.Net;
using PlayerBallControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public delegate void PlayerChangeHandler(Player player);
        public delegate void HasBallChangeHandler(bool hasBall);
        public delegate void StateChangeHandler(State newState);
        public delegate void NicknameChangeHandler(string nickname);
        public delegate void SceneChangeHandler(ConnectionId connectionId, short scene);


        public event PlayerChangeHandler TeamChanged;
        public event SceneChangeHandler SceneChanged;
        public event StateChangeHandler StateChanged;
        public event HasBallChangeHandler HasBallChanged;
        public event NicknameChangeHandler NicknameChanged;
        public event PlayerChangeHandler AvatarChanged;
        public event PlayerChangeHandler PlayAsGoalieChanged;
        public event PlayerChangeHandler Destroyed;

        public PlayerController controller;
        public PlayerBallController ballController;
        public GameObject gameobjectAvatar;

        internal PlayerFlags flagsChanged;

        internal void NotifyTeamChanged()
        {
            if (TeamChanged != null)
                TeamChanged.Invoke(this);
        }

        internal void NotifyStateChanged()
        {
            if (StateChanged != null)
                StateChanged.Invoke(state);
        }

        internal void NotifyAvatarSettingsChanged()
        {
            if (AvatarChanged != null)
                AvatarChanged.Invoke(this);
        }

        internal void NotifyNicknameChanged()
        {
            if (NicknameChanged != null)
                NicknameChanged.Invoke(nickname);
        }

        internal bool IsHoldingBall()
        {
            return MyComponents.BallState.GetIdOfPlayerOwningBall() == id;
        }

        public ConnectionId id;
        public bool isReady = false;

        internal Team team = Team.NONE;

        public void ChangeTeam(Team newTeam)
        {
            Team = newTeam;
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
                if (SceneChanged != null)
                    SceneChanged.Invoke(id, sceneId);
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
                if (PlayAsGoalieChanged != null)
                    PlayAsGoalieChanged.Invoke(this);
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
            if(Destroyed != null)
            {
                Destroyed.Invoke(this);
            }
        }

        public bool IsMyPlayer { get { return id == Players.myPlayerId; } }

        public Player(ConnectionId id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return "Player " + (IsMyPlayer ? "(mine) " : "") + id + " Nickname: " + Nickname + " Team : " + Team + "   HasBall : " + hasBall + " Avatar " + AvatarSettingsType + " State " + CurrentState + " SpawningPoint " + SpawningPoint;
        }


    }
}