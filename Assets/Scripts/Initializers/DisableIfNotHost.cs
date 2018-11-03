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
           MyComponents.MyPlayer.IsHostChanged += HostChanged;
            HostChanged(MyComponents.MyPlayer.IsHost);
        }
    }

    private void HostChanged(bool isHost)
    {
        GetComponent<Selectable>().interactable = isHost;
    }
}
