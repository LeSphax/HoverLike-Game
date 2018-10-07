//*************************************************************
//THIS CLASS WAS GENERATED, USE THE CORRESPONDING GENERATOR TO CHANGE IT (CodeGenerator folder)
//*************************************************************
using System;
using UnityEngine;
using UnityEngine.UI;
public class RoomSettings : SlideBall.NetworkMonoBehaviour
{
    public GameObject panel;
    public Dropdown numberPlayersDropdown;
    public InputField passwordInputField;
    private void Start()
    {
        Reset();
    }
    private float ParseFloat(string value, float defaultValue)
    {
        try { return float.Parse(value); }
        catch (Exception)
        {
            Debug.LogWarning("Couldn't parse " + value);
            return defaultValue;
        }
    }
    public int NumberPlayers
    {
        get
        {
            return MatchPanel.MaxNumberPlayers;
        }
        set
        {
            MatchPanel.MaxNumberPlayers = value;
        }
    }
    public string Password
    {
        get
        {
            return MatchPanel.Password;
        }
        set
        {
            MatchPanel.Password = value;
            MyComponents.NetworkManagement.RefreshRoomData();
        }
    }
    [MyRPC]
    private void SettingsAsked()
    {
        Save();
    }
    public void Save()
    {
        View.RPC("SetSettings", RPCTargets.All, numberPlayersDropdown.value + 1, passwordInputField.text);
    }
    [MyRPC]
    public void SetSettings(int numberPlayers, string password)
    {
        NumberPlayers = numberPlayers;
        numberPlayersDropdown.value = NumberPlayers - 1;
        Password = password;
        passwordInputField.text = Password;
    }
    public void Show(bool show)
    {
        if (show) { Reset(); }
        panel.SetActive(show);
    }
    private void Reset()
    {
        if (!NetworkingState.IsServer)
            View.RPC("SettingsAsked", RPCTargets.Server);
        numberPlayersDropdown.value = NumberPlayers - 1;
        passwordInputField.text = Password;
    }
}

