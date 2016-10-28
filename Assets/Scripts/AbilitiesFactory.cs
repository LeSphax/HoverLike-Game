using UnityEngine;
using System.Collections.Generic;
using PlayerManagement;

public class AbilitiesFactory : MonoBehaviour {

    private List<GameObject> abilityGOs = new List<GameObject>();

	public void RecreateAbilities()
    {
        foreach(GameObject ability in abilityGOs)
        {
            Destroy(ability);
        }
        abilityGOs.Clear();
        foreach ( string ability in Players.MyPlayer.MyAvatarSettings.abilities)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Abilities/" + ability);
            GameObject go = Instantiate(prefab);
            go.transform.SetParent(transform, false);
            abilityGOs.Add(go);
        }
    }
}
