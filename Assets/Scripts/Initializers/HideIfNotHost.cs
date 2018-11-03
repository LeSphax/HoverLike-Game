using PlayerManagement;
using UnityEngine;

public class HideIfNotHost : SlideBall.MonoBehaviour
{

    void Start()
    {
        if (!EditorVariables.HeadlessServer)
        {
           MyComponents.MyPlayer.IsHostChanged += HostChanged;
            HostChanged(MyComponents.MyPlayer.IsHost);
        }
    }

    void HostChanged(bool isHost)
    {
        gameObject.SetActive(isHost);
    }
}
