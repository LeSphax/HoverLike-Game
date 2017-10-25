using PlayerManagement;
using UnityEngine;

public class MoveTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        if (Players.MyPlayer != null)
        {
            Vector3 direction;
            if (UserSettings.GetKeyCode(0) == KeyCode.A)
            {
                direction = new MovementInputPacket(
                        Input.GetKey(KeyCode.Z),
                        Input.GetKey(KeyCode.S),
                        Input.GetKey(KeyCode.Q),
                        Input.GetKey(KeyCode.D)
                    ).GetDirection();
            }
            else
            {
                direction = new MovementInputPacket(
                        Input.GetKey(KeyCode.W),
                        Input.GetKey(KeyCode.S),
                        Input.GetKey(KeyCode.A),
                        Input.GetKey(KeyCode.D)
                    ).GetDirection();
            }
            callback.Invoke(true, Players.MyPlayer.controller, direction);
        }
    }
}
