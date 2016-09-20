using UnityEngine;

public delegate void CastOnTarget(GameObject gameObject, Vector3 target);

public abstract class AbilityTargeting : MonoBehaviour
{

    public abstract void ChooseTarget(CastOnTarget callback);
}