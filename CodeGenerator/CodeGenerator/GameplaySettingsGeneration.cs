using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GameplaySettingsGeneration
{

    private static Dictionary<string, string> typeToUIComponent;
    private static Dictionary<string, string> TypeToUIComponent
    {
        get
        {
            if (typeToUIComponent == null)
            {
                typeToUIComponent = new Dictionary<string, string>();
                typeToUIComponent.Add("bool", "Toggle");
                typeToUIComponent.Add("float", "InputField");
                typeToUIComponent.Add("string", "InputField");
                typeToUIComponent.Add("int", "Dropdown");
            }
            return typeToUIComponent;
        }
    }

    private static Dictionary<string, string> uiComponentToContent;
    private static Dictionary<string, string> UIComponentToContent
    {
        get
        {
            if (uiComponentToContent == null)
            {
                uiComponentToContent = new Dictionary<string, string>();
                uiComponentToContent.Add("Toggle", ".isOn");
                uiComponentToContent.Add("InputField", ".text");
                uiComponentToContent.Add("Dropdown", ".value");
            }
            return uiComponentToContent;
        }
    }
    private static List<string> typeWithParser;
    private static List<string> TypesWithParser
    {
        get
        {
            if (typeWithParser == null)
            {
                typeWithParser = new List<string>();
                typeWithParser.Add("float");
            }
            return typeWithParser;
        }
    }

    public class Field
    {
        public string type;
        public string name;
        public string realValue;

        public Field(string type, string name, string realValue)
        {
            this.type = type;
            this.name = name;
            this.realValue = realValue;
        }

        public string UIComponentName
        {
            get
            {
                return name + TypeToUIComponent[type];
            }
        }

        public string UIComponent
        {
            get
            {
                return TypeToUIComponent[type];
            }
        }

        public string UIComponentContent
        {
            get
            {
                return UIComponentName + UIComponentToContent[TypeToUIComponent[type]];
            }
        }

        public string NewValue
        {
            get
            {
                string newValue;
                if (TypesWithParser.Contains(type))
                    newValue = "Parse" + type.ToUpperFirstChar() + "("+ UIComponentContent+ ", " + PropertyName + ")";
                else
                    newValue = UIComponentContent;
                if (type == "int")
                    newValue = newValue + " + 1";
                return newValue;
            }
        }

        public string CurrentValue
        {
            get
            {
                string currentValue = PropertyName;
                if (type == "int")
                    currentValue = PropertyName + " - 1";
                else if (TypesWithParser.Contains(type))
                    currentValue += "+\"\"";
                return currentValue;
            }
        }



        public string PropertyName
        {
            get
            {
                return name.ToUpperFirstChar();
            }
        }

    }

    private static string AbilityCooldown(string abilityName)
    {
        return "ResourcesGetter." + abilityName + "Prefab.GetComponent<Ability>().cooldownDuration";
    }

    public static readonly string[] Abilities = { "Move", "Dash", "Jump", "Pass", "Steal", "Block", "TimeSlow", "Teleport" };

    public static void Main()
    {
        List<string> gameplayContent =new List<string>
        {
            "bool","continuouMovement","MoveInput.ContinuousMovement",
            "float","attackerAcceleration","AttackerMovementStrategy.Acceleration",
            "float","attackerMaxSpeed","AttackerMovementStrategy.MaxVelocity",
            "float","goalieSpeed","GoalieMovementStrategy.Speed",
            "float","shootPowerLevel","BallMovementView.ShootPowerLevel",
            "float","brakeProportion","AttackerMovementStrategy.BrakeProportion",
            "float","timeSlowProportion","TimeSlowApplier.TimeSlowProportion"

        };
        foreach(string ability in Abilities)
        {
            gameplayContent.Add("float");
            gameplayContent.Add(ability.ToLowerFirstChar() + "Cooldown");
            gameplayContent.Add(AbilityCooldown(ability));
        }
        List<string> roomContent = new List<string>
        {
            "int","numberPlayers","MatchPanel.MaxNumberPlayers",
            "string","password","MatchPanel.Password",
        };
        List<Field> gameplayFields = GetFields(gameplayContent);
        List<Field> roomFields = GetFields(roomContent);
        CreateClass("GameplaySettings", gameplayFields);
        CreateClass("RoomSettings", roomFields);
    }

    private static List<Field> GetFields(List<string> content)
    {
        List<Field> fields = new List<Field>();
        for (int i = 0; i < content.Count; i += 3)
        {
            Field field = new Field(content[i], content[i + 1], content[i + 2]);
            fields.Add(field);
        }

        return fields;
    }

    static void CreateClass(string className, List<Field> fields)
    {
        StringBuilder dc = new StringBuilder(4096);

        dc.AppendLine("//*************************************************************");
        dc.AppendLine("//THIS CLASS WAS GENERATED, USE THE CORRESPONDING GENERATOR TO CHANGE IT (CodeGenerator folder)");
        dc.AppendLine("//*************************************************************");
        dc.AppendLine("using System;");
        dc.AppendLine("using UnityEngine;");
        dc.AppendLine("using UnityEngine.UI;");
        dc.AppendLine("using TimeSlow;");

        dc.AppendLine("public class " + className + " : SlideBall.MonoBehaviour {");
        dc.AppendLine("public GameObject panel;");

        foreach (Field mField in fields)
        {
            //dc.Append("[FileHelpers.FieldQuoted('\"', QuoteMode.OptionalForBoth)]");
            dc.AppendLine("public " + mField.UIComponent + " " + mField.UIComponentName + ";");
        }
        Start(dc);
        foreach (string type in TypesWithParser)
        {
            Parser(dc, type);
        }
        Fields(fields, dc);
        SettingsAsked(dc);

        Save(fields, dc);

        SetSettings(fields, dc);
        Show(dc);
        Reset(fields, dc);
        dc.AppendLine("}");

        StreamWriter file = new StreamWriter(@"../../../../Assets/Scripts/Settings/" + className + ".cs");
        file.WriteLine(dc.ToString()); // "sb" is the StringBuilder
        file.Close();
    }

    private static void SetSettings(List<Field> fields, StringBuilder dc)
    {
        dc.AppendLine("[MyRPC]");
        dc.Append("public void SetSettings(");
        foreach (Field mField in fields)
        {
            dc.Append(mField.type + " " + mField.name + ", ");
        }
        dc.Remove(dc.Length - 2, 2);
        dc.AppendLine(")");
        dc.AppendLine("{");
        foreach (Field mField in fields)
        {
            dc.AppendLine(mField.PropertyName + " = " + mField.name + ";");
            dc.AppendLine(mField.UIComponentContent + " = " + mField.CurrentValue + ";");

        }
        dc.AppendLine("}");
    }

    private static void Save(List<Field> fields, StringBuilder dc)
    {
        dc.AppendLine("public void Save()");
        dc.AppendLine("{");
        dc.Append("   View.RPC(\"SetSettings\", RPCTargets.All");
        foreach (Field mField in fields)
        {
            dc.Append(", " + mField.NewValue);
        }
        dc.AppendLine(");");
        dc.AppendLine("}");
    }

    private static void SettingsAsked(StringBuilder dc)
    {
        dc.AppendLine("[MyRPC]");
        dc.AppendLine("private void SettingsAsked()");
        dc.AppendLine("{");
        dc.AppendLine("    Save();");
        dc.AppendLine(" }");
    }

    private static void Fields(List<Field> fields, StringBuilder dc)
    {
        foreach (Field mField in fields)
        {
            dc.AppendLine("public " + mField.type + " " + mField.PropertyName);
            dc.AppendLine("{");
            dc.AppendLine("get");
            dc.AppendLine("{");
            dc.AppendLine("return " + mField.realValue + ";");
            dc.AppendLine("}");
            dc.AppendLine("set");
            dc.AppendLine("{");
            dc.AppendLine(mField.realValue + " = value;");
            dc.AppendLine("}");
            dc.AppendLine("}");
        }
    }

    private static void Parser(StringBuilder dc, string type)
    {
        dc.AppendLine("private " + type + " Parse" + type.ToUpperFirstChar() + "(string value, " + type + " defaultValue)");
        dc.AppendLine("{");
        dc.AppendLine("   try { return " + type + ".Parse(value); }");
        dc.AppendLine("    catch (Exception)");
        dc.AppendLine("   {");
        dc.AppendLine("       Debug.LogWarning(\"Couldn't parse \" + value);");
        dc.AppendLine("       return defaultValue;");
        dc.AppendLine("    }");
        dc.AppendLine("}");
    }

    private static void Start(StringBuilder dc)
    {
        dc.AppendLine("private void Start()");
        dc.AppendLine("{");
        dc.AppendLine("Reset();");
        dc.AppendLine("}");
    }

    private static void Reset(List<Field> fields, StringBuilder dc)
    {
        dc.AppendLine("private void Reset()");
        dc.AppendLine("{");
        dc.AppendLine("if (!MyComponents.NetworkManagement.IsServer)");
        dc.AppendLine("  View.RPC(\"SettingsAsked\", RPCTargets.Server);");
        foreach (Field mField in fields)
        {
            dc.AppendLine(mField.UIComponentContent + " = " + mField.CurrentValue + ";");
            //dc.Append(mField.UIComponentName + ".onValueChanged.RemoveAllListeners();");
            //dc.Append(mField.UIComponentName + ".onValueChanged.AddListener(value => " + mField.PropertyName + " = ");
          //dc.AppendLine(mField.NewValue + ");");
        }
        dc.AppendLine("}");
    }

    private static void Show(StringBuilder dc)
    {
        dc.AppendLine("public void Show(bool show)");
        dc.AppendLine("{");
        dc.AppendLine("    if (show){ Reset();}");
        dc.AppendLine("    panel.SetActive(show);");
        dc.AppendLine("}");
    }


}


