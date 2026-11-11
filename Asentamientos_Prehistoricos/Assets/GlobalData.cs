using UnityEngine;
using System.Collections.Generic;

public static class GlobalData
{
    public static float tileSize = 0.4f;
    public static Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainValues = new Dictionary<int,Dictionary<int,(float height,float humidity,float temperature)>>();

    public static float Map(float value, float low1, float max1, float low2, float max2)
    {
        return low2 + (max2 - low2) * (value - low1) / (max1 - low1);
    }
}