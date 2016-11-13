using PlayerManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class EnumExtensions
{


    public static void SetFlag(this PlayerFlags a, PlayerFlags b)
    {
        a = a | b;
    }

    public static void UnsetFlag(this PlayerFlags a, PlayerFlags b)
    {
        a = a & (~b);
    }

    // Works with "None" as well
    public static bool HasFlag(this PlayerFlags a, PlayerFlags b)
    {
        return (a & b) == b;
    }

    public static void ToogleFlag(PlayerFlags a, PlayerFlags b)
    {
        a = a ^ b;
    }

    public static void SetFlag(this InputFlag a, InputFlag b)
    {
        Debug.LogError(a + "   " + (a | b));
        a = a | b;
        Debug.LogError(a);

    }

    public static void UnsetFlag(this InputFlag a, InputFlag b)
    {
        a = a & (~b);
    }

    // Works with "None" as well
    public static bool HasFlag(this InputFlag a, InputFlag b)
    {
        return (a & b) == b;
    }

    public static void ToogleFlag(InputFlag a, InputFlag b)
    {
        a = a ^ b;
    }

}
