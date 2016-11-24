using UnityEngine;

public class TimeSlowTargeting : AbilityTargeting
{
    [SerializeField]
    private static GameObject prefabTargeter;
    private static GameObject PrefabTargeter
    {
        get
        {
            if (prefabTargeter == null)
            {
                prefabTargeter = Resources.Load<GameObject>(Paths.TIME_SLOW_TARGETER);
            }
            return prefabTargeter;
        }
    }

    public static float DIAMETER_TIME_SLOW_ZONE
    {
        get
        {
            return PrefabTargeter.transform.localScale.x * 10;
        }
    }
    private GameObject targeter;

    private CastOnTarget callback;

    public override void ChooseTarget(CastOnTarget callback)
    {
        this.callback = callback;
        targeter = (GameObject)Instantiate(PrefabTargeter, transform, true);
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
                callback.Invoke(null, targeter.transform.position);
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
        targeter.transform.position = Functions.GetMouseWorldPosition() + Vector3.up * 0.2f;
    }
}
