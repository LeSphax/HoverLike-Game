using PlayerManagement;

public class NoMovementEffect : AbilityEffect
{
    public float duration;

    public override void ApplyOnTarget(params object[] parameters)
    {
        MyComponents.Players.players[PlayerId].State.Movement = MovementState.NO_MOVEMENT;
        Invoke("StopFreezing", duration);
    }

    private void StopFreezing()
    {
        if  (MyComponents.Players.players[PlayerId].State.Movement == MovementState.NO_MOVEMENT)
            MyComponents.Players.players[PlayerId].State.Movement = MovementState.PLAYING;
    }
}
