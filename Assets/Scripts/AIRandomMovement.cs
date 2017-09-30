using PlayerBallControl;
using PlayerManagement;
using System.Collections;
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

        bool noInvocation = false;

        public event EmptyEventHandler FinishedUsingAllAbilities;

        // Use this for initialization
        void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        private void Update()
        {
            //Changing scene
            if (controller.Player != null)
                if (controller.Player.IsMyPlayer)
                {
                    if (activateOnServer || !MyComponents.NetworkManagement.IsServer)
                    {
                        //Debug.LogError("Self "+isActivated + "    " + DevelopperCommands.activateAI);
                        if (!isActivated && DevelopperCommands.activateAI)
                        {
                            abilitiesManager = controller.abilitiesManager;
                            //Move();
                            //Jump();
                            //Steal();
                            //Dash();
                            Pass();
                            //Block();
                            isActivated = true;
                        }
                        if (isActivated && !DevelopperCommands.activateAI)
                        {
                            CancelInvoke();
                            isActivated = false;
                        }
                    }
                }
        }

        public void UseAllAbilities(bool attacker)
        {
            abilitiesManager = controller.abilitiesManager;
            StartCoroutine(CoUseAllAbilities(attacker));
        }

        IEnumerator CoUseAllAbilities(bool attacker)
        {
            Debug.Log("Use All Abilities " + attacker);
            noInvocation = true;
            Move(Vector3.zero);
            yield return new WaitForSeconds(0.3f);
            Dash(Vector3.zero);
            yield return new WaitForSeconds(0.3f);
            Steal();
            yield return new WaitForSeconds(0.1f);
            MyComponents.BallState.PutBallAtPosition(transform.position);
            yield return new WaitForSeconds(0.05f);
            Shoot();
            yield return new WaitForSeconds(0.1f);
            if (attacker)
            {
                MyComponents.BallState.PutBallAtPosition(transform.position);
                yield return new WaitForSeconds(0.05f);
                Pass();
                yield return new WaitForSeconds(0.1f);
            }
            if (!attacker)
            {
                Block();
                yield return new WaitForSeconds(0.1f);
            }
            if (!attacker)
            {
                TimeSlow();
                yield return new WaitForSeconds(0.1f);
            }
            if (!attacker)
            {
                Teleport();
                yield return new WaitForSeconds(2f);
            }
            enabled = false;
            noInvocation = false;
            if (FinishedUsingAllAbilities != null)
                FinishedUsingAllAbilities.Invoke();
        }

        private void Move()
        {
            Move(null);
        }

        private void Move(Vector2? target)
        {
            if (target == null)
                target = new Vector2(GetRandomPointInTerrain().x, GetRandomPointInTerrain().y);
            abilitiesManager.View.RPC("Move", RPCTargets.Server, target.Value);

            InvokeRandom("Move", MoveDelay, MoveDelay * 2);
        }

        private void Shoot()
        {
            Vector3[] controlPoints = new Vector3[]
            {
                controller.transform.position,
                GetRandomPointInTerrain(),
                GetRandomPointInTerrain()
            };
            abilitiesManager.View.RPC("Shoot", RPCTargets.Server, controlPoints, 1.0f);

            InvokeRandom("Shoot", MoveDelay, MoveDelay * 2);
        }

        private void Jump()
        {
            abilitiesManager.View.RPC("Jump", RPCTargets.Server);
            InvokeRandom("Jump", JumpDelay, JumpDelay * 2);
        }

        private void Steal()
        {
            abilitiesManager.View.RPC("Steal", RPCTargets.Server, 0.5f);
            InvokeRandom("Steal", 1, 1.5f);
        }

        private void Block()
        {
            abilitiesManager.View.RPC("Block", RPCTargets.Server);
            InvokeRandom("Block", 3, 4.5f);
        }

        private void Dash()
        {
            Dash(null);
        }

        private void Dash(Vector3? target)
        {
            if (target == null)
                target = GetRandomPointInTerrain();
            abilitiesManager.View.RPC("Dash", RPCTargets.Server, target.Value);
            InvokeRandom("Dash", 3, 5);
        }

        private void TimeSlow()
        {
            Vector3 target = GetRandomPointInTerrain();
            abilitiesManager.View.RPC("TimeSlow", RPCTargets.Server, target);
            InvokeRandom("TimeSlow", 3, 8);
        }

        private void Teleport()
        {
            abilitiesManager.View.RPC("Teleport", RPCTargets.Server);
            InvokeRandom("Teleport", 3, 8);
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
            if (!noInvocation)
                Invoke(methodName, Random.Range(minTime, maxTime));
        }

        private static Vector3 GetRandomPointInTerrain()
        {
            return Functions.GetRandomPointInVolume(Vector3.zero, new Vector3(-30, 5, -70), new Vector3(30, 5, 70));
        }
    }
}