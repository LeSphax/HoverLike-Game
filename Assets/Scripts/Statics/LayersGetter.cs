using System;
using System.Collections.Generic;
using UnityEngine;

public static class LayersGetter
{
    public static int[] players = new int[3] { 8, 11, 14 };

    public static int ballLayer = 9;

    public static LayerMask BallMask()
    {
        return 1 << ballLayer;
    }
}
