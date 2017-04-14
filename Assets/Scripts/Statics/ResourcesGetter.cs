using UnityEngine;

public class ResourcesGetter
{
    private const string MATERIALS_FOLDER = "Materials/";
    private const string AUDIO_FOLDER = "Audio/";
    private const string PREFABS_FOLDER = "Prefabs/";

    private static Material[] helmetMaterials;
    public static Material[] HelmetMaterials
    {
        get
        {
            if (helmetMaterials == null)
            {
                helmetMaterials = new Material[2];
                helmetMaterials[0] = Resources.Load<Material>(MATERIALS_FOLDER + "BlueHelmet");
                helmetMaterials[1] = Resources.Load<Material>(MATERIALS_FOLDER + "RedHelmet");
            }
            return helmetMaterials;
        }
    }

    private static Material[] skateMaterials;
    public static Material[] SkateMaterials
    {
        get
        {
            if (skateMaterials == null)
            {
                skateMaterials = new Material[2];
                skateMaterials[0] = Resources.Load<Material>(MATERIALS_FOLDER + "BlueSkate");
                skateMaterials[1] = Resources.Load<Material>(MATERIALS_FOLDER + "RedSkate");
            }
            return skateMaterials;
        }
    }

    internal static GameObject SettingsPanel()
    {
        return Resources.Load<GameObject>(PREFABS_FOLDER + "SettingsPanel");
    }

    public static AudioClip ErrorSound()
    {
        return Resources.Load<AudioClip>(AUDIO_FOLDER + "Soft error");
    }

    public static AudioClip BoostSound()
    {
        return Resources.Load<AudioClip>(AUDIO_FOLDER + "Boost");
    }

    public static AudioClip PassSound()
    {
        return Resources.Load<AudioClip>(AUDIO_FOLDER + "Pass");
    }

    public static AudioClip TeleportSound()
    {
        return Resources.Load<AudioClip>(AUDIO_FOLDER + "Teleport1 5sec");
    }

    public static AudioClip TimeSlowSound()
    {
        return Resources.Load<AudioClip>(AUDIO_FOLDER + "TimeSlow");
    }

    private static GameObject tempAudioSource;
    public static GameObject TempAudioSource
    {
        get
        {
            if (tempAudioSource == null)
            {
                tempAudioSource = GetTempAudioSource();
            }
            return tempAudioSource;
        }
    }

    private static GameObject GetTempAudioSource()
    {
        return Resources.Load<GameObject>(AUDIO_FOLDER + "TempAudioSource");
    }
}

