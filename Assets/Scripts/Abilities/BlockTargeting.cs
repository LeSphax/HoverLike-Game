using PlayerManagement;
using UnityEngine;

public class BlockTargeting : AbilityTargeting
{
    public override void ChooseTarget(CastOnTarget callback)
    {
        GameObject gameObject = Players.MyPlayer.gameobjectAvatar;
        callback.Invoke(gameObject, gameObject.transform.position);
    }
}
