
using System;
using System.Collections.Generic;


public class AvatarSettings
{
    public string MESH_NAME;
    public float acceleration;
    public float maxSpeed;
    public float catchColliderRadius;
    public float catchColliderZPos;
    public float catchColliderHeight;
    public string[] abilities;

    private static Dictionary<AvatarSettingsTypes, AvatarSettings> data;
    public static Dictionary<AvatarSettingsTypes,AvatarSettings> Data
    {
        get
        {
            if (data == null)
            {
                CreateSettings();
            }
            return data;
        }
    }

    public enum AvatarSettingsTypes
    {
        NONE,
        GOALIE,
        ATTACKER,
    }

    private static string GetFileName(AvatarSettingsTypes type)
    {
        switch (type)
        {
            case AvatarSettingsTypes.GOALIE:
                return "GoalieSettings";
            case AvatarSettingsTypes.ATTACKER:
                return "AttackerSettings";
            default:
                throw new UnhandledSwitchCaseException(type);
        }
    }

    private static void CreateSettings()
    {
        data = new Dictionary<AvatarSettingsTypes, AvatarSettings>();
        foreach (AvatarSettingsTypes type in Enum.GetValues(typeof(AvatarSettingsTypes)))
        {
            if (type != AvatarSettingsTypes.NONE)
            {
                AvatarSettings a = new AvatarSettings();
                Dictionary<string, string> settings = Settings.GetSettings(GetFileName(type));
                a.MESH_NAME = settings["MESH_NAME"];
                a.acceleration = float.Parse(settings["acceleration"]);
                a.maxSpeed = float.Parse(settings["maxSpeed"]);
                a.catchColliderRadius = float.Parse(settings["catchColliderRadius"]);
                a.catchColliderZPos = float.Parse(settings["catchColliderZPos"]);
                a.catchColliderHeight = float.Parse(settings["catchColliderHeight"]);
                a.abilities = Settings.ParseTable(settings["abilities"]);
                data.Add(type, a);
            }
        }
    }
}