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

}
