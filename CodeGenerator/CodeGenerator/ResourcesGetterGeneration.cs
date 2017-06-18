using System;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

public class ResourcesGetterGeneration
{

    private const string MATERIALS_FOLDER = "Materials/";
    private const string AUDIO_FOLDER = "Audio/";
    private const string PREFABS_FOLDER = "Prefabs/";
    private static Dictionary<string, string> typeToFolders;
    private static Dictionary<string, string> TypeToFolders
    {
        get
        {
            if (typeToFolders == null)
            {
                typeToFolders = new Dictionary<string, string>();
                typeToFolders.Add("AudioClip", "Sounds/");
                typeToFolders.Add("Material", "Materials/");
                typeToFolders.Add("GameObject", "Prefabs/");
            }
            return typeToFolders;
        }
    }

    public class Field
    {
        public string type;
        public string name;

        public string DisplayName
        {
            get
            {
                string append = IsArray ? FirstFolder.Substring(0, FirstFolder.Length - 1) : FirstFolder.Substring(0, FirstFolder.Length - 2);
                return name + append;
            }
        }
        private string FirstFolder
        {
            get
            {
                return TypeToFolders.ContainsKey(TypeWithoutArray) ? TypeToFolders[TypeWithoutArray] : "";
            }
        }
        public string Folder
        {
            get
            {
                return FirstFolder + additionalFolders;
            }
        }
        public string TypeWithoutArray
        {
            get
            {
                if (!IsArray)
                {
                    return type;
                }
                else
                    return type.Substring(0, type.Length - 2);
            }
        }

        public bool IsArray
        {
            get
            {
                return type.Substring(type.Length - 2, 2) == "[]";
            }

        }
        public List<string> fileNames = new List<string>();

        private string additionalFolders = "";

        public Field(string type, string name)
        {
            if (type.Contains("/"))
            {
                int pos = type.IndexOf('/');
                additionalFolders = type.Substring(pos + 1, type.Length - pos - 1) + "/";
                this.type = type.Substring(0, pos);
                Debug.WriteLine(type + "    " + additionalFolders);
            }
            else
                this.type = type;
            this.name = name;
        }
    }

    public static void Main()
    {
        List<string> content = new List<string>
        {
            "AudioClip","TimeSlow",
            "AudioClip","Teleport2",
            "AudioClip","Boost",
            "AudioClip","Pass",
            "AudioClip","SoftError",
            "AudioClip","Landing",
            "AudioClip","Jumping",
            "AudioClip","But",
            "AudioClip","JoinRoom",
            "GameObject","SettingsPanel",
            "GameObject","TempAudioSource",
            "GameObject","TimeSlowTargeter",
            "GameObject","PassTargeter",
            "GameObject","ScreenFader",
            "GameObject","Tooltip",
            "GameObject","Shadow",
            "GameObject","KeyToUse",
            "GameObject","PlayerInfo",


            "GameObject/Abilities","Disabled",
            "GameObject/Abilities","Cooldown",
            "GameObject/Abilities","Ability",
            

            "GameObject","MoveUIAnimation",
            "Material","Field",
            
            "Material[]","Helmet", "2", "BlueHelmet", "RedHelmet",
            "Material[]","Skate", "2", "BlueSkate", "RedSkate"
        };
        foreach(string ability in GameplaySettingsGeneration.Abilities)
        {
            content.Add("GameObject/Abilities");
            content.Add(ability);
        }
        List<Field> fields = new List<Field>();
        for (int i = 0; i < content.Count; i += 2)
        {
            Field field = new Field(content[i], content[i + 1]);

            if (field.IsArray)
            {
                int nbItems = int.Parse(content[i + 2]);
                for (int y = 3; y < nbItems + 3; y++)
                {
                    field.fileNames.Add(content[i + y]);
                }
                i += nbItems + 1;
            }
            fields.Add(field);
        }
        CreateClass(fields);
    }


    static void CreateClass(List<Field> fields)
    {
        StringBuilder dc = new StringBuilder(4096);
        StringBuilder loadAllMethod = new StringBuilder(512);

        loadAllMethod.AppendLine("public static void LoadAll(){");
        dc.AppendLine("//*************************************************************");
        dc.AppendLine("//THIS CLASS WAS GENERATED, USE THE CORRESPONDING GENERATOR TO CHANGE IT (CodeGenerator folder)");
        dc.AppendLine("//*************************************************************");
        dc.AppendLine("using UnityEngine;");
        dc.Append("public class ResourcesGetter");
        dc.AppendLine("{");
        foreach (Field mField in fields)
        {
            //dc.Append("[FileHelpers.FieldQuoted('\"', QuoteMode.OptionalForBoth)]");
            dc.AppendLine("private static " + mField.type + " " + mField.DisplayName.ToLowerFirstChar() + ";");

            dc.AppendLine("public static " + mField.type + " " + mField.DisplayName + "{");

            dc.AppendLine("get {");
            dc.AppendLine("if (" + mField.DisplayName.ToLowerFirstChar() + " == null ){");
            if (mField.IsArray)
            {
                dc.AppendLine(mField.DisplayName.ToLowerFirstChar() + "= new " + mField.TypeWithoutArray + "[" + mField.fileNames.Count + "];");
                for (int i = 0; i < mField.fileNames.Count; i++)
                {
                    dc.AppendLine(mField.DisplayName.ToLowerFirstChar() + "[" + i + "]= Resources.Load<" + mField.TypeWithoutArray + ">(\"" + mField.Folder + mField.fileNames[i] + "\");");
                }
            }
            else
                dc.AppendLine(mField.DisplayName.ToLowerFirstChar() + "= Resources.Load<" + mField.type + ">(\"" + mField.Folder + mField.name + "\");");
            dc.AppendLine("}");
            dc.AppendLine("return " + mField.DisplayName.ToLowerFirstChar() + ";");
            dc.AppendLine("}}");

            loadAllMethod.AppendLine("var temp"+ mField.DisplayName+ " = " + mField.DisplayName + ";");
        }
        loadAllMethod.AppendLine("}");
        dc.Append(loadAllMethod);

        dc.AppendLine("}");
        StreamWriter file = new StreamWriter(@"../../../../Assets/Scripts/Statics/ResourcesGetter.cs");
        file.WriteLine(dc.ToString()); // "sb" is the StringBuilder
        file.Close();
    }
}