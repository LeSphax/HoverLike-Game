using Byn.Net;
using System.Collections.Generic;

public class PlayerInput
{
    public ConnectionId playerId;
    public List<AbilityEffect> effects;

    public void AddEffects(List<AbilityEffect> effects)
    {
        this.effects.AddRange(effects);
    }

    public PlayerInput(ConnectionId playerId, List<AbilityEffect> effects)
    {
        this.playerId = playerId;
        this.effects = effects;
    }

}
