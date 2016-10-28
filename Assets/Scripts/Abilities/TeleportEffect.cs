using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffect : AbilityEffect
{

    private List<GameObject> effects;

    public float TimeBeforeTeleportation;

    private GameObject target;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        this.target = target;
        GameObject effectPrefab = Resources.Load<GameObject>("Effects/Teleportation");
        effects = new List<GameObject>();
        GameObject effectAtPlayer = (GameObject)Instantiate(effectPrefab, target.transform,false);
        GameObject effectAtSpawningPoint = (GameObject)Instantiate(effectPrefab, position, Quaternion.identity);

        effects.Add(effectAtPlayer);
        effects.Add(effectAtSpawningPoint);
        Invoke("Teleport", TimeBeforeTeleportation);
    }

    private void Teleport()
    {
        Debug.Log("Teleport");
        target.transform.position = Players.MyPlayer.SpawningPoint;
        foreach(GameObject effect in effects)
        {
            Destroy(effect);
        }
        effects = null;
    }
}
