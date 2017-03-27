using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesGetter
{
    private const string MATERIALS_FOLDER = "Materials/";
    private const string AUDIO_FOLDER = "Audio/";

    public static Material PlayerMaterial()
    {
        return Resources.Load<Material>(MATERIALS_FOLDER + "Player");
    }

    public static Material OutLineMaterial()
    {
        return Resources.Load<Material>(MATERIALS_FOLDER + "Outline");
    }

    public static AudioClip ErrorSound()
    {
        return Resources.Load<AudioClip>(AUDIO_FOLDER + "Soft error");
    }

}
