using UnityEngine;

class TimeSimulation : MonoBehaviour
{

    private static float timeInSeconds = 1;
    public static float TimeInSeconds
    {
        get
        {
            if (MyComponents.NetworkManagement.IsServer)
            {
                return timeInSeconds;
            }
            else
            {
                return TimeManagement.SimulationTimeInSeconds - ClientDelay.Delay;
            }
        }
        private set
        {
            timeInSeconds = value;
        }
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
        if (MyComponents.NetworkManagement.IsServer)
        {
            TimeInSeconds += UnityEngine.Time.fixedDeltaTime;
        }
    }


}
