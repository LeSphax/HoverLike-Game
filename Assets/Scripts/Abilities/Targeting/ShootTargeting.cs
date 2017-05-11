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
        ShowArmingAnimation(true);
    }

    //Instant for self to hide latency
    private void ShowArmingAnimation(bool isArming)
    {
        Players.MyPlayer.controller.abilitiesManager.View.RPC("Arm", RPCTargets.Server, isArming);
        Players.MyPlayer.controller.abilitiesManager.EffectsManager.ShowArmAnimation(isArming);
    }

    public override void CancelTargeting()
    {
        if (isActivated && powerBar.IsFilling())
        {
            isActivated = false;
            powerBar.Hide();
            ShowArmingAnimation(false);
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
        callback.Invoke(true,Players.MyPlayer.controller, position);
    }
}
