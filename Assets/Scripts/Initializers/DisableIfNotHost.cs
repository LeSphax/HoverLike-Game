using PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class DisableIfNotHost : MonoBehaviour
{
    void Start()
    {
        if (!EditorVariables.HeadlessServer)
        {
            Players.MyPlayer.IsHostChanged += HostChanged;
            HostChanged(Players.MyPlayer.IsHost);
        }
    }

    private void HostChanged(bool isHost)
    {
        GetComponent<Selectable>().interactable = isHost;
    }
}
