using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PowerBar))]
public class ShootTargeting : AbilityTargeting
{
    PowerBar powerBar;
    CastOnTarget callback;

    public override List<AbilityEffect> StartTargeting(CastOnTarget callback)
    {
        powerBar = GetComponent<PowerBar>();
        powerBar.StartFilling();
        this.callback = callback;
        return null;
    }

    public override List<AbilityEffect> ReactivateTargeting()
    {
        if (powerBar.IsFilling())
        {
            Vector3 position = Functions.GetMouseWorldPosition();
            List<AbilityEffect> result = callback.Invoke(powerBar.powerValue, position);
            powerBar.Hide();
            return result;
        }
        return null;
    }

    public override void CancelTargeting()
    {
        if (powerBar.IsFilling())
        {
            powerBar.Hide();
        }
    }

}
