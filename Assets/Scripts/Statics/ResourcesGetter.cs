using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesGetter
{
    private const string MATERIALS_FOLDER = "Materials/";

    public static Material RedMaterial()
    {
        return Resources.Load<Material>(MATERIALS_FOLDER+"Red");
    }

    public static Material BlueMaterial()
    {
        return Resources.Load<Material>(MATERIALS_FOLDER + "Blue");
    }

}
