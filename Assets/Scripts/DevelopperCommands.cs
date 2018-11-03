using AbilitiesManagement;
using PlayerManagement;
using System.Text;
using UnityEngine;

public class DevelopperCommands : SlideBall.NetworkMonoBehaviour
{
    public static bool activateAI = false;
    public static bool askedForReport = false;
    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        if (Functions.IsDevelopperComboPressed())
        {
            if (MyComponents.MatchManager != null)
            {
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
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Delete Player Prefs");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                View.RPC("ActivateAI", RPCTargets.All);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                View.RPC("ActivateOnServer", RPCTargets.All);
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                MyComponents.GameplaySettings.Show(true);
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                askedForReport = true;
                View.RPC("SendNetworkData", RPCTargets.All);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("[REPORT]" + ActualAbilitiesLatency.PrintAll());
            }
            if (MyComponents.BallState != null && MyComponents.MatchManager != null && Vector3.Distance(MyComponents.BallState.transform.position, Vector3.zero) > 300f)
            {
                MyComponents.BallState.transform.position = new Vector3(0, 10, 0);
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

    [MyRPC]
    private void SendNetworkData()
    {
        Debug.Log("SendNetworkData");
        if (!NetworkingState.IsServer)
        {
            StringBuilder builder = new StringBuilder(4096);
            builder.Append("[REPORT]" + MyComponents.MyPlayer.Nickname);
            //builder.Append("[REPORT]Average difference : " + MyNetworkView.averageDifference + ", Number of Resets : " + MyNetworkView.nbOfResets);
            builder.Append("[REPORT]" + ActualAbilitiesLatency.Print());
            //builder.Append("[REPORT]Packet loss ratio : " + ((float)ObservedComponent.NumberPacketsMissed / ObservedComponent.NumberPacketsReceived) + ", Missed : "
                //+ ObservedComponent.NumberPacketsMissed + ", Received : " + ObservedComponent.NumberPacketsReceived);
            View.RPC("GetAndPrintIndividualNetworkReport", RPCTargets.All, builder.ToString());
        }
    }

    [MyRPC]
    private void GetAndPrintIndividualNetworkReport(string report)
    {
        if (askedForReport)
        {
            Debug.Log(report);
        }
    }
}
