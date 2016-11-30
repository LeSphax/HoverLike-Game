﻿using UnityEngine;
using System.Collections.Generic;
using PlayerManagement;
using System;
using UnityEngine.UI;

public class AbilitiesFactory : MonoBehaviour
{

    private GameObject prefabAbility;
    private GameObject PrefabAbility
    {
        get
        {
            if (prefabAbility == null)
                prefabAbility = Resources.Load<GameObject>(Paths.ABILITIES + "Ability");
            return prefabAbility;
        }
    }
    private List<GameObject> abilityGOs = new List<GameObject>();

    public void RecreateAbilities()
    {
        foreach (GameObject ability in abilityGOs)
        {
            Destroy(ability);
        }
        abilityGOs.Clear();
        foreach (string ability in Players.MyPlayer.MyAvatarSettings.abilities)
        {
            GameObject layout = Instantiate(PrefabAbility);
            layout.transform.SetParent(transform, false);
            abilityGOs.Add(layout);
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
