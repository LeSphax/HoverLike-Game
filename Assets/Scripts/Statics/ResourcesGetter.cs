using UnityEngine;
public class ResourcesGetter
{
    private static AudioClip timeslow;
    public static AudioClip TimeSlowSound
    {
        get
        {
            if (timeslow == null)
            {
                timeslow = Resources.Load<AudioClip>("Sounds/TimeSlow");
            }
            return timeslow;
        }
    }
    private static AudioClip teleport2;
    public static AudioClip Teleport2Sound
    {
        get
        {
            if (teleport2 == null)
            {
                teleport2 = Resources.Load<AudioClip>("Sounds/Teleport2");
            }
            return teleport2;
        }
    }
    private static AudioClip boost;
    public static AudioClip BoostSound
    {
        get
        {
            if (boost == null)
            {
                boost = Resources.Load<AudioClip>("Sounds/Boost");
            }
            return boost;
        }
    }
    private static AudioClip pass;
    public static AudioClip PassSound
    {
        get
        {
            if (pass == null)
            {
                pass = Resources.Load<AudioClip>("Sounds/Pass");
            }
            return pass;
        }
    }
    private static AudioClip softerror;
    public static AudioClip SoftErrorSound
    {
        get
        {
            if (softerror == null)
            {
                softerror = Resources.Load<AudioClip>("Sounds/SoftError");
            }
            return softerror;
        }
    }
    private static AudioClip landing;
    public static AudioClip LandingSound
    {
        get
        {
            if (landing == null)
            {
                landing = Resources.Load<AudioClip>("Sounds/Landing");
            }
            return landing;
        }
    }
    private static AudioClip jumping;
    public static AudioClip JumpingSound
    {
        get
        {
            if (jumping == null)
            {
                jumping = Resources.Load<AudioClip>("Sounds/Jumping");
            }
            return jumping;
        }
    }
    private static AudioClip but;
    public static AudioClip ButSound
    {
        get
        {
            if (but == null)
            {
                but = Resources.Load<AudioClip>("Sounds/But");
            }
            return but;
        }
    }
    private static GameObject settingspanel;
    public static GameObject SettingsPanelPrefab
    {
        get
        {
            if (settingspanel == null)
            {
                settingspanel = Resources.Load<GameObject>("Prefabs/SettingsPanel");
            }
            return settingspanel;
        }
    }
    private static GameObject tempaudiosource;
    public static GameObject TempAudioSourcePrefab
    {
        get
        {
            if (tempaudiosource == null)
            {
                tempaudiosource = Resources.Load<GameObject>("Prefabs/TempAudioSource");
            }
            return tempaudiosource;
        }
    }
    private static GameObject timeslowtargeter;
    public static GameObject TimeSlowTargeterPrefab
    {
        get
        {
            if (timeslowtargeter == null)
            {
                timeslowtargeter = Resources.Load<GameObject>("Prefabs/TimeSlowTargeter");
            }
            return timeslowtargeter;
        }
    }
    private static GameObject passtargeter;
    public static GameObject PassTargeterPrefab
    {
        get
        {
            if (passtargeter == null)
            {
                passtargeter = Resources.Load<GameObject>("Prefabs/PassTargeter");
            }
            return passtargeter;
        }
    }
    private static GameObject screenfader;
    public static GameObject ScreenFaderPrefab
    {
        get
        {
            if (screenfader == null)
            {
                screenfader = Resources.Load<GameObject>("Prefabs/ScreenFader");
            }
            return screenfader;
        }
    }
    private static GameObject tooltip;
    public static GameObject TooltipPrefab
    {
        get
        {
            if (tooltip == null)
            {
                tooltip = Resources.Load<GameObject>("Prefabs/Tooltip");
            }
            return tooltip;
        }
    }
    private static GameObject shadow;
    public static GameObject ShadowPrefab
    {
        get
        {
            if (shadow == null)
            {
                shadow = Resources.Load<GameObject>("Prefabs/Shadow");
            }
            return shadow;
        }
    }
    private static GameObject keytouse;
    public static GameObject KeyToUsePrefab
    {
        get
        {
            if (keytouse == null)
            {
                keytouse = Resources.Load<GameObject>("Prefabs/KeyToUse");
            }
            return keytouse;
        }
    }
    private static GameObject disabled;
    public static GameObject DisabledPrefab
    {
        get
        {
            if (disabled == null)
            {
                disabled = Resources.Load<GameObject>("Prefabs/Abilities/Disabled");
            }
            return disabled;
        }
    }
    private static GameObject cooldown;
    public static GameObject CooldownPrefab
    {
        get
        {
            if (cooldown == null)
            {
                cooldown = Resources.Load<GameObject>("Prefabs/Abilities/Cooldown");
            }
            return cooldown;
        }
    }
    private static GameObject ability;
    public static GameObject AbilityPrefab
    {
        get
        {
            if (ability == null)
            {
                ability = Resources.Load<GameObject>("Prefabs/Abilities/Ability");
            }
            return ability;
        }
    }
    private static GameObject moveuianimation;
    public static GameObject MoveUIAnimationPrefab
    {
        get
        {
            if (moveuianimation == null)
            {
                moveuianimation = Resources.Load<GameObject>("Prefabs/MoveUIAnimation");
            }
            return moveuianimation;
        }
    }
    private static Material field;
    public static Material FieldMaterial
    {
        get
        {
            if (field == null)
            {
                field = Resources.Load<Material>("Materials/Field");
            }
            return field;
        }
    }
    private static Material[] helmet;
    public static Material[] HelmetMaterials
    {
        get
        {
            if (helmet == null)
            {
                helmet = new Material[2];
                helmet[0] = Resources.Load<Material>("Materials/BlueHelmet");
                helmet[1] = Resources.Load<Material>("Materials/RedHelmet");
            }
            return helmet;
        }
    }
    private static Material[] skate;
    public static Material[] SkateMaterials
    {
        get
        {
            if (skate == null)
            {
                skate = new Material[2];
                skate[0] = Resources.Load<Material>("Materials/BlueSkate");
                skate[1] = Resources.Load<Material>("Materials/RedSkate");
            }
            return skate;
        }
    }
    public static void LoadAll()
    {
        var tempTimeSlowSound = TimeSlowSound;
        var tempTeleport2Sound = Teleport2Sound;
        var tempBoostSound = BoostSound;
        var tempPassSound = PassSound;
        var tempSoftErrorSound = SoftErrorSound;
        var tempLandingSound = LandingSound;
        var tempJumpingSound = JumpingSound;
        var tempButSound = ButSound;
        var tempSettingsPanelPrefab = SettingsPanelPrefab;
        var tempTempAudioSourcePrefab = TempAudioSourcePrefab;
        var tempTimeSlowTargeterPrefab = TimeSlowTargeterPrefab;
        var tempPassTargeterPrefab = PassTargeterPrefab;
        var tempScreenFaderPrefab = ScreenFaderPrefab;
        var tempTooltipPrefab = TooltipPrefab;
        var tempShadowPrefab = ShadowPrefab;
        var tempKeyToUsePrefab = KeyToUsePrefab;
        var tempDisabledPrefab = DisabledPrefab;
        var tempCooldownPrefab = CooldownPrefab;
        var tempAbilityPrefab = AbilityPrefab;
        var tempMoveUIAnimationPrefab = MoveUIAnimationPrefab;
        var tempFieldMaterial = FieldMaterial;
        var tempHelmetMaterials = HelmetMaterials;
        var tempSkateMaterials = SkateMaterials;
    }
}

