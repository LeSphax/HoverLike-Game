using UnityEngine;

class TimeSimulation : SlideBall.MonoBehaviour
{

    private static float timeInSeconds = 1;
    public static float TimeInSeconds
    {
        get
        {
            if (NetworkingState.IsServer)
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
        if (NetworkingState.IsServer)
        {
            TimeInSeconds += UnityEngine.Time.fixedDeltaTime;
        }
    }


}
