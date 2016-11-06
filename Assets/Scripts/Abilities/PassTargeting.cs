using UnityEngine;

public class PassTargeting : AbilityTargeting
{
    [SerializeField]
    private GameObject prefabTargeter;
    private GameObject targeter;

    private CastOnTarget callback;

    public override void ChooseTarget(CastOnTarget callback)
    {
        this.callback = callback;
        targeter = (GameObject)Instantiate(prefabTargeter, transform, true);
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
                callback.Invoke(targeter, targeter.transform.position);
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
