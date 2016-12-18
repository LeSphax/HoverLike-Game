using UnityEngine;
using System.Collections;

public class PlayerMesh : MonoBehaviour
{

    public Transform hand;
    public GameObject helmet;
    public GameObject skate;
    public GameObject body;

    public Material blueTeamHelmet;
    public Material redTeamHelmet;

    public GameObject ownerIndicator;

    public void SetTeam(Team team)
    {
        switch (team)
        {
            case Team.BLUE:
                helmet.GetComponent<Renderer>().material = blueTeamHelmet;
                break;
            case Team.RED:
                helmet.GetComponent<Renderer>().material = redTeamHelmet;
                break;
            default:
                Debug.LogError("This team hasn't an associated color " + team);
                return;
        }
        if (skate != null)
            foreach (Renderer renderer in skate.GetComponentsInChildren<Renderer>())
            {
                renderer.material.color = Colors.Teams[(int)team];
            }
    }

    public void SetOwner(bool isOwner)
    {
        ownerIndicator.SetActive(isOwner);
    }
}
