using PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class PlayAsGoalie : MonoBehaviour {

    [HideInInspector]
    public Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = false;
        SetActivated();
    }

    public void SetActivated()
    {
        Players.MyPlayer.PlayAsGoalie = toggle.isOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            toggle.isOn = !toggle.isOn;
    }
}
