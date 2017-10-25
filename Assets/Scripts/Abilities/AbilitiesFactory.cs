using PlayerManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesFactory : MonoBehaviour
{

    public Dictionary<string, GameObject> abilityGOs = new Dictionary<string, GameObject>();

    public void RecreateAbilities()
    {
        foreach (GameObject ability in abilityGOs.Values)
        {
            Destroy(ability);
        }
        abilityGOs.Clear();
        foreach (string ability in Players.MyPlayer.MyAvatarSettings.abilities)
        {
            GameObject layout = Instantiate(ResourcesGetter.AbilityPrefab);
            layout.transform.SetParent(transform, false);
            abilityGOs.Add(ability, layout);
            layout.name = ability;
            //
            string abilityPath = Paths.ABILITIES + ability;
            GameObject prefab = Resources.Load<GameObject>(abilityPath);
            //Check if the ability has an icon
            if (prefab.GetComponent<Image>() == null)
            {
                //Avoiding empty space for abilities without icons
                Destroy(layout.GetComponent<LayoutElement>());
            }
            if (prefab != null) { 
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(layout.transform, false);
            }
            else 
            {
                Debug.LogError("The ability " + ability + " isn't present in the Resources folder at " + abilityPath);
            }

        }
    }
}
