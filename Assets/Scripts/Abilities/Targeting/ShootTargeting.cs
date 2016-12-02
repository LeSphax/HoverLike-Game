using PlayerManagement;
using UnityEngine;

[RequireComponent(typeof(PowerBar))]
public class ShootTargeting : AbilityTargeting
{
    PowerBar powerBar;
    CastOnTarget callback;

    private bool isActivated;

    public override void ChooseTarget(CastOnTarget callback)
    {
        powerBar = GetComponent<PowerBar>();
        powerBar.StartFilling();
        this.callback = callback;
        isActivated = true;
    }

    public override void CancelTargeting()
    {
        if (isActivated && powerBar.IsFilling())
        {
            isActivated = false;
            powerBar.Hide();
        }
    }

    public override void ReactivateTargeting()
    {
        if (isActivated && powerBar.IsFilling())
        {
            isActivated = false;
            Activate();
            powerBar.Hide();
        }
    }

    private void Activate()
    {
        Vector3 position = Functions.GetMouseWorldPosition();
        callback.Invoke(Players.MyPlayer.controller, position);
    }
}
