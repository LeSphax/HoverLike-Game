using Byn.Net;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesFactory : SlideBall.MonoBehaviour
{
    [HideInInspector]
    private ConnectionId playerId = ConnectionId.INVALID;
    public ConnectionId PlayerId
    {
        get
        {
            return playerId;
        }
        set
        {
            playerId = value;
            if (playerId != MyComponents.MyPlayer.id)
                GetComponent<Image>().enabled = false;
        }
    }

    public Dictionary<string, GameObject> abilityGOs = new Dictionary<string, GameObject>();

    public void RecreateAbilities()
    {
        if (playerId == ConnectionId.INVALID)
        {
            playerId = MyComponents.MyPlayer.id;
        }

        foreach (GameObject ability in abilityGOs.Values)
        {
            Destroy(ability);
        }
        abilityGOs.Clear();
        foreach (string ability in MyComponents.Players.players[playerId].MyAvatarSettings.abilities)
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
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(layout.transform, false);
                go.GetComponent<Ability>().PlayerId = playerId;
            }
            else
            {
                Debug.LogError("The ability " + ability + " isn't present in the Resources folder at " + abilityPath);
            }

        }
    }
}
