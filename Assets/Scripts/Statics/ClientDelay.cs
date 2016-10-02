using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClientDelay : MonoBehaviour
{

    public Text text;

    private bool shown = false;

    private static double delay = -1;
    public static double Delay
    {
        get
        {
            if (delay == -1)
#if UNITY_WEBGL
                delay = 0.3f;

#else
                delay = 3 * 1f / PhotonNetwork.sendRate + PhotonNetwork.GetPing() / 1000f;
#endif
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
        if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.D))
        {
            shown = !shown;
            text.enabled = shown;
        }
        if (shown)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Delay += 0.01f;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                Delay -= 0.01f;
            }
            text.text = "Delay : " + Delay + " ms";
        }
    }
}
