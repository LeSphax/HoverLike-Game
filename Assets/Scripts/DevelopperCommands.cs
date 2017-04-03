using AbilitiesManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopperCommands : MonoBehaviour
{
    public static bool ActivateAI = false;
    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        if (Functions.IsDevelopperComboPressed())
        {
            if (MyComponents.MatchManager != null)
                if (Input.GetKeyDown(KeyCode.S))
                    MyComponents.MatchManager.View.RPC("SetReady", RPCTargets.All);
                else if (Input.GetKeyDown(KeyCode.E))
                    MyComponents.MatchManager.View.RPC("ManualEnd", RPCTargets.All);
                else if (Input.GetKeyDown(KeyCode.M))
                    MyComponents.MatchManager.View.RPC("ManualEntry", RPCTargets.Server);
                else if (Input.GetKeyDown(KeyCode.B))
                {
                    Debug.Log("Call ManualScoreGoal");
                    MyComponents.MatchManager.View.RPC("ManualScoreGoal", RPCTargets.Server, 0);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                    MyComponents.MatchManager.View.RPC("ManualScoreGoal", RPCTargets.Server, 1);
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Delete Player Prefs");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
            if (Input.GetKeyDown(KeyCode.W))
                ActivateAI = !ActivateAI;
            if (Input.GetKeyDown(KeyCode.X))
                AIRandomMovement.activateOnServer = !AIRandomMovement.activateOnServer;
            if (MyComponents.BallState != null && MyComponents.MatchManager != null && Vector3.Distance(MyComponents.BallState.transform.position, Vector3.zero) > 300f)
            {
                MyComponents.MatchManager.View.RPC("ManualEntry", RPCTargets.Server);
            }
        }
    }

}
