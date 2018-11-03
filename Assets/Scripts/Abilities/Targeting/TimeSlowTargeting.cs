using UnityEngine;

public class TimeSlowTargeting : AbilityTargeting
{
    public static float DIAMETER_TIME_SLOW_ZONE
    {
        get
        {
            return ResourcesGetter.TimeSlowTargeterPrefab.transform.localScale.x * 10;
        }
    }
    private GameObject targeter;

    private CastOnTarget callback;

    public override void ChooseTarget(CastOnTarget callback)
    {
        this.callback = callback;
        targeter = (GameObject)Instantiate(ResourcesGetter.TimeSlowTargeterPrefab, transform, true);
        IsTargeting = true;
        UpdateTargeterPosition();
    }

    void Update()
    {
        if (targeter != null)
        {
            UpdateTargeterPosition();
            if (Input.GetMouseButtonDown(0))
            {
                callback.Invoke(true,targeter.transform.position - (targeter.transform.position.y - Constants.GROUND_LEVEL) * Vector3.up);
                CancelTargeting();
            }
        }
    }

    public override void CancelTargeting()
    {
        IsTargeting = false;
        Destroy(targeter);
    }

    private void UpdateTargeterPosition()
    {
        targeter.transform.localPosition = MyComponents.MyPlayer.InputManager.GetMouseLocalPosition() + Vector3.up * 0.2f;
    }
}
