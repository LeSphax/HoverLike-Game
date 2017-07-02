using UnityEngine;
using UnityEngine.UI;

public class PasswordPanel : MonoBehaviour
{
    public InputField input;

    private string roomName;

    public void PasswordEntered()
    {
        MyComponents.NetworkManagement.ConnectToRoom(roomName,input.text);
    }

    public static void InstantiatePanel(string roomName)
    {
        PasswordPanel pass = Instantiate(ResourcesGetter.PasswordPanelPrefab).GetComponent<PasswordPanel>();
        pass.transform.SetParent(MyComponents.PopUp.transform,false);
        pass.roomName = roomName;
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
