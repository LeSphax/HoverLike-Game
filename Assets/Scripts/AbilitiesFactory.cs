using UnityEngine;
using System.Collections.Generic;
using PlayerManagement;
using System;

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
            string abilityPath = "Abilities/" + ability;
            GameObject prefab = Resources.Load<GameObject>(abilityPath);
            try
            {
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(transform, false);
                abilityGOs.Add(go);
            }
            catch (ArgumentException)
            {
                Debug.LogError("The ability " + ability + " isn't present in the Resources folder at " + abilityPath);
            }
           
        }
    }
}
