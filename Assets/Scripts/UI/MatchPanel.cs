using PlayerManagement;
using UnityEngine;

public class MatchPanel : MonoBehaviour
{
    public Transform[] TeamPanel;

    public void Open()
    {
        gameObject.SetActive(true);
        foreach (Player player in Players.players.Values)
        {
            GameObject playerInfo = Instantiate(ResourcesGetter.PlayerInfoPrefab);
            playerInfo.transform.SetParent(TeamPanel[(int)player.Team]);
            playerInfo.GetComponent<PlayerInfo>().InitView(new object[] { player.id });
        }
    }
}