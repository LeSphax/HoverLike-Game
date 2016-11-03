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
        Debug.LogWarning("ChooseTarget");
        powerBar = GetComponent<PowerBar>();
        powerBar.StartFilling();
        this.callback = callback;
        isActivated = true;
    }

    void Update()
    {
        if (isActivated)
            if (Input.GetMouseButtonUp(0) && powerBar.IsFilling())
            {
                isActivated = false;
                Activate();
                powerBar.Hide();
            }
            else if (Input.GetMouseButtonDown(1) && powerBar.IsFilling())
            {
                isActivated = false;
                powerBar.Hide();
            }
    }

    private void Activate()
    {
        Vector3 position = Functions.GetMouseWorldPosition();
        callback.Invoke(Players.MyPlayer.controller.gameObject, position);
    }
}
