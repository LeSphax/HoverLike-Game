using AbilitiesManagement;
using Byn.Net;
using PlayerManagement;
public class PlayerSpawner : SlideBall.MonoBehaviour
{
    [MyRPC]
    public void DesactivatePlayers(short syncId)
    {
        AbilitiesManager.ResetAllEffects();
        Players.players.Values.Map(player =>
        {
            player.controller.gameObject.SetActive(false);
        });
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    [MyRPC]
    public void ResetPlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            player.controller.ResetPlayer();
        });

        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    [MyRPC]
    public void ReactivatePlayers(short syncId)
    {
        Players.players.Values.Map(player =>
        {
            player.controller.gameObject.SetActive(true);
        });
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

}
