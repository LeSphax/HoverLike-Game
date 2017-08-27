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
        curving = false;
        if (SlideBallInputs.GetKey(UserSettings.GetKeyCode(6), SlideBallInputs.GUIPart.ABILITY))
        {
            curving = true;
            bezier.controlPoints[0] = Players.MyPlayer.controller.transform.position;
            bezier.controlPoints[2] = Functions.GetMouseWorldPosition();
        }
        ShowPowerOnCurve();
    }

    //Instant for self to hide latency
    private void ShowArmingAnimation(bool isArming)
    {
        Players.MyPlayer.controller.abilitiesManager.View.RPC("Arm", RPCTargets.Server, isArming);
        Players.MyPlayer.controller.abilitiesManager.EffectsManager.ShowArmAnimation(isArming);
    }

    private void Update()
    {

        if (IsActivated)
        {
            //Debug.Log(curveLength);
            //Debug.Log(proportion);

            if (SlideBallInputs.GetKeyDown(UserSettings.GetKeyCode(6), SlideBallInputs.GUIPart.ABILITY))
            {
                curving = true;
                bezier.controlPoints[2] = Functions.GetMouseWorldPosition();
            }
            else if (SlideBallInputs.GetKeyUp(UserSettings.GetKeyCode(6), SlideBallInputs.GUIPart.ABILITY))
            {
                curving = false;
            }
            bezier.controlPoints[0] = Players.MyPlayer.controller.transform.position;
            bezier.controlPoints[1] = Functions.GetMouseWorldPosition();
            if (!curving)
                bezier.controlPoints[2] = Functions.GetMouseWorldPosition();

            ShowPowerOnCurve();
        }
    }

    private void ShowPowerOnCurve()
    {
        float v0 = BallMovementView.GetShootPowerLevel(powerBar.powerValue);
        float curveLength = 0;
        if (bezier.controlPoints[1].HasValue)
            curveLength = BezierMaths.LengthBezier3(bezier.controlPoints.Select(point => point.Value).ToArray(), 10);
        else if (bezier.controlPoints[0].HasValue && bezier.controlPoints[2].HasValue)
            curveLength = Vector3.Distance(bezier.controlPoints[0].Value, bezier.controlPoints[2].Value);

        float proportion = 0;
        if (curveLength != 0)
            proportion = v0 / curveLength;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(new Color(Mathf.Clamp(1 / proportion, 0, 1), Mathf.Clamp(1 - 1 / proportion, 0, 1), 0), proportion) },
            new GradientAlphaKey[] { new GradientAlphaKey(proportion == 0 ? 0 : 1, 0.0f), new GradientAlphaKey(1, 0.8f * proportion), new GradientAlphaKey(1 - 1 / proportion, proportion) }
            );
        GetComponent<LineRenderer>().colorGradient = gradient;

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
        callback.Invoke(true, Players.MyPlayer.controller, controlPoints);
    }
}
