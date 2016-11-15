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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.P))
        {
            shown = !shown;
            text.enabled = shown;
        }
        if (shown)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Delay += 0.01f;
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                Delay -= 0.01f;
            }
            text.text = "Delay : " + Delay + " ms";
        }
    }
}
