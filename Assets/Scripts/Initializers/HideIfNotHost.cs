using PlayerManagement;
using UnityEngine;

public class HideIfNotHost : MonoBehaviour
{

    void Start()
    {
        if (!EditorVariables.HeadlessServer)
        {
            Players.MyPlayer.IsHostChanged += HostChanged;
            HostChanged(Players.MyPlayer.IsHost);
        }
    }

    void HostChanged(bool isHost)
    {
        gameObject.SetActive(isHost);
    }
}
