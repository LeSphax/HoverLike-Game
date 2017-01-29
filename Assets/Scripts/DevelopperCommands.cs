using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopperCommands : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift))
        {
            if (MyComponents.MatchManager != null)
                if (Input.GetKeyDown(KeyCode.S))
                    MyComponents.MatchManager.View.RPC("SetReady", RPCTargets.All);
                else if (Input.GetKeyDown(KeyCode.E))
                    MyComponents.MatchManager.View.RPC("ManualEnd", RPCTargets.All);
                else if (Input.GetKeyDown(KeyCode.M))
                    MyComponents.MatchManager.View.RPC("ManualEntry", RPCTargets.Server);
                else if (Input.GetKeyDown(KeyCode.B))
                    MyComponents.MatchManager.View.RPC("ManualScoreGoal", RPCTargets.Server, 0);
                else if (Input.GetKeyDown(KeyCode.R))
                    MyComponents.MatchManager.View.RPC("ManualScoreGoal", RPCTargets.Server, 1);
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Delete Player Prefs");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
        }
        if (MyComponents.BallState != null && MyComponents.MatchManager != null && Vector3.Distance(MyComponents.BallState.transform.position, Vector3.zero) > 300f)
        {
            MyComponents.MatchManager.View.RPC("ManualEntry", RPCTargets.Server);
        }
    }

}
