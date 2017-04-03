using UnityEngine;

public class ResourcesGetter
{
    private const string MATERIALS_FOLDER = "Materials/";
    private const string AUDIO_FOLDER = "Audio/";
    private const string PREFABS_FOLDER = "Prefabs/";

    public static Material PlayerMaterial()
    {
        return Resources.Load<Material>(MATERIALS_FOLDER + "Player");
    }

    public static Material OutLineMaterial()
    {
        return Resources.Load<Material>(MATERIALS_FOLDER + "Outline");
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

