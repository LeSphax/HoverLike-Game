using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesGetter
{
    private const string MATERIALS_FOLDER = "Materials/";

    public static Material PlayerMaterial()
    {
        return Resources.Load<Material>(MATERIALS_FOLDER + "Player");
    }

    public static Material OutLineMaterial()
    {
        return Resources.Load<Material>(MATERIALS_FOLDER + "ToonLitOutline");
    }

}
