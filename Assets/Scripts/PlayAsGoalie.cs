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
        toggle.isOn = EditorVariables.PlayAsGoalieInitialValue;
        SetActivated();
    }

    public void SetActivated()
    {
        Players.MyPlayer.PlayAsGoalie = toggle.isOn;
    }
}
