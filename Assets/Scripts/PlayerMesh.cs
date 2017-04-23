using UnityEngine;

public class PlayerMesh : MonoBehaviour
{

    public Transform hand;
    public GameObject helmet;
    public GameObject skate;
    public GameObject body;

    private Material[] helmetMaterials;
    private Material[] HelmetMaterials
    {
        get
        {
            if (helmetMaterials == null)
                helmetMaterials = ResourcesGetter.HelmetMaterials;
            return helmetMaterials;
        }
    }
    private Material[] skateMaterials;
    private Material[] SkateMaterials
    {
        get
        {
            if (skateMaterials == null)
                skateMaterials = ResourcesGetter.SkateMaterials;
            return skateMaterials;
        }
    }

    public GameObject ownerIndicator;

    public void SetTeam(Team team)
    {
        helmet.GetComponent<Renderer>().material = HelmetMaterials[(int)team];
        if (skate != null)
            skate.GetComponent<Renderer>().material = SkateMaterials[(int)team];
    }

    public void SetOwner(bool isOwner)
    {
        ownerIndicator.SetActive(isOwner);
    }
}
