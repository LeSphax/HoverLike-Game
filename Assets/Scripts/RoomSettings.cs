using UnityEngine;
using UnityEngine.UI;

public class RoomSettings : SlideBall.MonoBehaviour
{

    public Dropdown nbPlayersDropdown;
    public InputField passwordInput;

    private void Start()
    {
        if (!MyComponents.NetworkManagement.isServer)
            View.RPC("SettingsAsked", RPCTargets.Server);
    }

    public int NumberPlayers
    {
        get
        {
            return nbPlayersDropdown.value + 1;
        }
        set
        {
            nbPlayersDropdown.value = value - 1;
        }
    }

    public string Password
    {
        get
        {
            return passwordInput.text;
        }
        set
        {
            passwordInput.text = value;
        }
    }

    [MyRPC]
    private void SettingsAsked()
    {
        Save();
    }

    public void Save()
    {
        View.RPC("SetSettings", RPCTargets.Others, NumberPlayers, Password);
    }

    [MyRPC]
    public void SetSettings(int nbPlayers, string password)
    {
        NumberPlayers = nbPlayers;
        Password = password;
    }

}