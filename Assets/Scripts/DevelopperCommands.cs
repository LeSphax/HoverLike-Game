using AbilitiesManagement;
using UnityEngine;

public class DevelopperCommands : SlideBall.MonoBehaviour
{
    public static bool activateAI = false;
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
            {
                View.RPC("ActivateAI", RPCTargets.All);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                View.RPC("ActivateOnServer", RPCTargets.All);


            }
            if (MyComponents.BallState != null && MyComponents.MatchManager != null && Vector3.Distance(MyComponents.BallState.transform.position, Vector3.zero) > 300f)
            {
                MyComponents.MatchManager.View.RPC("ManualEntry", RPCTargets.Server);
            }
        }
    }

    [MyRPC]
    private void ActivateAI()
    {
        activateAI = !activateAI;
                Debug.Log("ActivateAI  " + activateAI);
    }

    [MyRPC]
    private void ActivateOnServer()
    {
        AIRandomMovement.activateOnServer = !AIRandomMovement.activateOnServer;
        Debug.Log("ActivateOnServer  " + AIRandomMovement.activateOnServer);
    }

}
