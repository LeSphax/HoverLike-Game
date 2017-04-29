using Navigation;
using UnityEngine;

public class MenuInGame : MonoBehaviour {


    public GameObject background;
    SlideBallInputs.GUIPart previousPart;

    public void OpenSettings()
    {
        UserSettingsPanel.InstantiateSettingsPanel().transform.SetParent(transform.parent, false);
    }

    public void OpenMenu()
    {
        background.SetActive(true);
        previousPart = SlideBallInputs.currentPart;
        SlideBallInputs.currentPart = SlideBallInputs.GUIPart.MENU;
    }

    public void CloseMenu()
    {
        background.SetActive(false);
        SlideBallInputs.currentPart = previousPart;
    }

    public void LeaveRoom()
    {
        MyComponents.ResetNetworkComponents();
        NavigationManager.LoadScene(Scenes.Lobby, true);
    }

    public void ReturnToRoom()
    {
        MyComponents.ResetGameComponents();
        NavigationManager.LoadScene(Scenes.Room,true);
    }

    private void OnEnable()
    {
        MyComponents.NetworkManagement.RoomClosed += LeaveRoom;
    }

    private void OnDisable()
    {
        MyComponents.NetworkManagement.RoomClosed -= LeaveRoom;
    }
}
