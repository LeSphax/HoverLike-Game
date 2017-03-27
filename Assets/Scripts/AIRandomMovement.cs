using PlayerBallControl;
using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitiesManagement
{
    [RequireComponent(typeof(PlayerController))]
    public class AIRandomMovement : MonoBehaviour
    {
        private float MoveDelay = 2f;
        private float JumpDelay = 10f;

        private PlayerController controller;
        private AbilitiesManager abilitiesManager;

        public static bool activateOnServer;

        public static bool isActivated;

        // Use this for initialization
        void Start()
        {
            controller = GetComponent<PlayerController>();
        }

        private void Update()
        {
            //Changing scene
            if (controller.Player != null)
                if (controller.Player.IsMyPlayer && !isActivated)
                {
                    if (!isActivated && DevelopperCommands.ActivateAI)
                    {
                        if (activateOnServer || !MyComponents.NetworkManagement.isServer)
                        {
                            abilitiesManager = controller.abilitiesManager;
                            Move();
                            Steal();
                            Dash();
                            Pass();

                        }
                        isActivated = true;
                    }
                    if (isActivated && !DevelopperCommands.ActivateAI)
                    {
                        CancelInvoke();
                        isActivated = false;

                    }
                }

        }

        private void Move()
        {
            Vector2 target = new Vector2(GetRandomPointInTerrain().x, GetRandomPointInTerrain().y);
            abilitiesManager.View.RPC("Move", RPCTargets.Server, target);
            InvokeRandom("Move", MoveDelay, MoveDelay * 2);
        }

        private static Vector3 GetRandomPointInTerrain()
        {
            return Functions.GetRandomPointInVolume(Vector3.zero, new Vector3(-30, 5, -70), new Vector3(30, 5, 70));
        }

        private void InvokeMove()
        {
            Invoke("Move", Random.Range(MoveDelay, MoveDelay * 2f));
        }

        private void Jump()
        {
            abilitiesManager.View.RPC("Jump", RPCTargets.Server);
            InvokeRandom("Jump", JumpDelay, JumpDelay * 2);
        }

        private void InvokeJump()
        {
            Invoke("Jump", Random.Range(JumpDelay, JumpDelay));
        }

        private void Steal()
        {
            abilitiesManager.View.RPC("Steal", RPCTargets.Server, 0.5f);
            InvokeRandom("Steal", 1, 1.5f);
        }

        private void Dash()
        {
            Vector3 target = GetRandomPointInTerrain();
            abilitiesManager.View.RPC("Dash", RPCTargets.Server, target);
            InvokeRandom("Dash", 3, 8);
        }

        private void Pass()
        {
            List<Player> friends = Players.GetPlayersInTeam(controller.Player.Team);
            friends.Remove(controller.Player);
            if (friends.Count > 0)
            {
                abilitiesManager.View.RPC("Pass", RPCTargets.Server, friends[Random.Range(0, friends.Count - 1)].id);
                InvokeRandom("Pass", 1.5f, 3);
            }
        }

        private void InvokeRandom(string methodName, float minTime, float maxTime)
        {
            Invoke(methodName, Random.Range(minTime, maxTime));
        }


    }
}