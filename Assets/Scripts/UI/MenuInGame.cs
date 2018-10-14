using Navigation;
using UnityEngine;

public class MenuInGame : SlideBall.MonoBehaviour
{


    public GameObject background;
    public MatchPanel matchPanel;
    GUIPart previousPart;
    private NetworkManagement mNetworkManagement;

    private void Start()
    {
        OpenMatch();
    }

    public void OpenSettings()
    {
        UserSettingsPanel.InstantiateSettingsPanel().transform.SetParent(transform.parent, false);
    }

    public void OpenMatch()
    {
        CloseMenu();
        matchPanel.Open(!matchPanel.gameObject.activeSelf);
    }

    public void OpenMenu()
    {
        background.SetActive(true);
        previousPart = InputManager.currentPart;
        InputManager.currentPart = GUIPart.MENU;
    }

    public void CloseMenu()
    {
        background.SetActive(false);
        InputManager.currentPart = previousPart;
    }

    public void LeaveRoom()
    {
        MyComponents.ResetNetworkComponents();
        NavigationManager.LoadScene(Scenes.Lobby, true);
    }

    public void ReturnToRoom()
    {
        MyComponents.ResetGameComponents();
        NavigationManager.LoadScene(Scenes.Room, true);
    }

    private void OnEnable()
    {
        mNetworkManagement = (NetworkManagement)MyComponents.NetworkManagement;
        mNetworkManagement.RoomClosed += LeaveRoom;
    }

    private void OnDisable()
    {
        mNetworkManagement.RoomClosed -= LeaveRoom;

    }
}
