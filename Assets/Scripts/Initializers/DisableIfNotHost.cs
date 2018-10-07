using PlayerManagement;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class DisableIfNotHost : SlideBall.MonoBehaviour
{
    void Start()
    {
        if (!EditorVariables.HeadlessServer)
        {
           MyComponents.Players.MyPlayer.IsHostChanged += HostChanged;
            HostChanged(MyComponents.Players.MyPlayer.IsHost);
        }
    }

    private void HostChanged(bool isHost)
    {
        GetComponent<Selectable>().interactable = isHost;
    }
}
