using System;
using System.Collections.Generic;
using UnityEngine;

public static class LayersGetter
{

    public const int GOALIE_0 = 8;
    public const int GOALIE_1 = 11;
    public const int ATTACKER = 14;
    public const int ATTACKER_0 = 15;
    public const int ATTACKER_1 = 16;
    public static int[] players = new int[5] { GOALIE_0, GOALIE_1, ATTACKER, ATTACKER_0, ATTACKER_1 };
    public static int[] attackers = new int[3] { ATTACKER_0, ATTACKER_1, ATTACKER };

    public const int BALL = 9;

    public static LayerMask BallMask()
    {
        return 1 << BALL;
    }

    public static LayerMask PlayersMask()
    {
        return CombineLayers(players);
    }

    internal static bool IsPlayer(int layer)
    {
        return IsInArray(layer,players);
    }

    internal static bool IsAttacker(int layer)
    {
        return IsInArray(layer, attackers);
    }

    private static bool IsInArray(int value, int[ ] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
                return true;
        }
        return false;
    }

    public static LayerMask AttackersMask()
    {
        return CombineLayers(attackers);
    }

    private static LayerMask CombineLayers(int[] layers)
    {
        LayerMask layerMask = 0;
        foreach (int layer in layers)
            layerMask |= 1 << layer;
        return layerMask;
    }
}
