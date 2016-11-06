using UnityEngine;

public class DisconnectedPopUp : MonoBehaviour
{

    private static bool showPopUp = false;
    public static bool ShowPopUp
    {
        set
        {
            showPopUp = value;
            if (disconnectedPopUp != null)
                disconnectedPopUp.SetActive(value);
        }
    }
    private static GameObject disconnectedPopUp;

    void Awake()
    {
        disconnectedPopUp = gameObject;
        if (!showPopUp)
        {
            disconnectedPopUp.SetActive(false);
        }
    }

    public void OkHandler()
    {
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        disconnectedPopUp = null;
    }
}
