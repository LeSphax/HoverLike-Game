using PlayerManagement;
using UnityEngine;

public class HideIfNotHost : SlideBall.MonoBehaviour
{

    void Start()
    {
        if (!EditorVariables.HeadlessServer)
        {
           MyComponents.Players.MyPlayer.IsHostChanged += HostChanged;
            HostChanged(MyComponents.Players.MyPlayer.IsHost);
        }
    }

    void HostChanged(bool isHost)
    {
        gameObject.SetActive(isHost);
    }
}
