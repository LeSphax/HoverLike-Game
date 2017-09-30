using PlayerManagement;
using UnityEngine;

public class MoveTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        if (Players.MyPlayer != null)
        {
            Vector3 direction = new MovementInputPacket(
                    Input.GetKey(KeyCode.W),
                    Input.GetKey(KeyCode.S),
                    Input.GetKey(KeyCode.A),
                    Input.GetKey(KeyCode.D)
                ).GetDirection();
            callback.Invoke(true, Players.MyPlayer.controller, direction);
        }
    }
}
