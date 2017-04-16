using UnityEngine;

public class PlayerMesh : MonoBehaviour
{

    public Transform hand;
    public GameObject helmet;
    public GameObject skate;
    public GameObject body;

    private Material[] helmetMaterials
    {
        get
        {
            return ResourcesGetter.HelmetMaterials;
        }
    }
    private Material[] skateMaterials
    {
        get
        {
            return ResourcesGetter.SkateMaterials;
        }
    }

    public GameObject ownerIndicator;

    public void SetTeam(Team team)
    {
        helmet.GetComponent<Renderer>().material = helmetMaterials[(int)team];
        if (skate != null)
            skate.GetComponent<Renderer>().material = skateMaterials[(int)team];
    }

    public void SetOwner(bool isOwner)
    {
        ownerIndicator.SetActive(isOwner);
    }
}
