using UnityEngine;
using UnityEngine.UI;

public class ClientDelay : MonoBehaviour
{

    public Text text;

    private bool shown = false;

    private static float delay = -1;
    public static float Delay
    {
        get
        {
            if (delay == -1)
                delay = 0.05f;
            //delay = 3 * 1f / PhotonNetwork.sendRate + PhotonNetwork.GetPing() / 1000f;
            return delay;
        }
        set
        {
            delay = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        text.enabled = shown;

    }
}
