using UnityEngine;
using UnityEngine.UI;

public class PasswordPanel : SlideBall.MonoBehaviour
{
    public InputField input;

    private string roomName;

    public void PasswordEntered()
    {
        ((NetworkManagement)MyComponents.NetworkManagement).ConnectToRoom(roomName,input.text);
        Debug.Log(input.text);
    }

    public static void InstantiatePanel(string roomName, Transform popupTransform)
    {
        PasswordPanel pass = Instantiate(ResourcesGetter.PasswordPanelPrefab).GetComponent<PasswordPanel>();
        pass.transform.SetParent(popupTransform,false);
        pass.roomName = roomName;
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
