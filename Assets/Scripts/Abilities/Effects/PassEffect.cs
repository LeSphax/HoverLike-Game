using Byn.Net;
using PlayerManagement;

public class PassEffect : AbilityEffect
{
    public override void ApplyOnTarget(params object[] parameters)
    {
        base.ApplyOnTarget(parameters);
        ConnectionId targetId = (ConnectionId)parameters[0];
        if (targetId != Players.INVALID_PLAYER_ID)
            MyComponents.MyPlayer.controller.View.RPC("Pass", RPCTargets.Server, targetId);
    }
}