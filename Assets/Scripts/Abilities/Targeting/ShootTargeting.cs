using PlayerManagement;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PowerBar))]
[RequireComponent(typeof(Bezier))]
public class ShootTargeting : AbilityTargeting
{
    PowerBar powerBar;
    Bezier bezier;
    CastOnTarget callback;

    private bool curving;

    private bool isActivated;
    private bool IsActivated
    {
        get
        {
            return isActivated;
        }
        set
        {
            bezier.Activated = value;
            isActivated = value;
        }
    }

    private void Start()
    {
        powerBar = GetComponent<PowerBar>();
        bezier = GetComponent<Bezier>();
    }

    public override void ChooseTarget(CastOnTarget callback)
    {
        powerBar.StartFilling();

        this.callback = callback;
        IsActivated = true;
        ShowArmingAnimation(true);
    }

    //Instant for self to hide latency
    private void ShowArmingAnimation(bool isArming)
    {
        Players.MyPlayer.controller.abilitiesManager.View.RPC("Arm", RPCTargets.Server, isArming);
        Players.MyPlayer.controller.abilitiesManager.EffectsManager.ShowArmAnimation(isArming);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            curving = true;
            bezier.controlPoints[2] = Functions.GetMouseWorldPosition();
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            curving = false;
        }
        if (IsActivated)
        {
            bezier.controlPoints[0] = Players.MyPlayer.controller.transform.position;
            bezier.controlPoints[1] = Functions.GetMouseWorldPosition();
            if (!curving)
                bezier.controlPoints[2] = Functions.GetMouseWorldPosition();
        }
    }

    public override void CancelTargeting()
    {
        if (IsActivated && powerBar.IsFilling())
        {
            IsActivated = false;
            powerBar.Hide();
            ShowArmingAnimation(false);
        }
    }

    public override void ReactivateTargeting()
    {
        if (IsActivated && powerBar.IsFilling())
        {
            IsActivated = false;
            Activate();
            powerBar.Hide();
        }
    }

    private void Activate()
    {
        Vector3[] controlPoints = bezier.controlPoints.Select(point => point.Value).ToArray();
        callback.Invoke(true,Players.MyPlayer.controller, controlPoints);
    }
}
