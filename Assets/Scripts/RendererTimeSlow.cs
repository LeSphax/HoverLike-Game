using UnityEngine;
using System.Collections;
using TimeSlow;
using AbilitiesManagement;

public class RendererTimeSlow : MonoBehaviour
{

    private void Awake()
    {
        transform.localScale = Vector3.one * TimeSlowTargeting.DIAMETER_TIME_SLOW_ZONE;
        gameObject.AddComponent<DestroyAfterTimeout>().timeout = TimeSlowPersistentEffect.DURATION;
        AbilitiesManager.visualEffects.Add(gameObject);
    }

    private void OnDestroy()
    {
        AbilitiesManager.visualEffects.Remove(gameObject);
    }
}
