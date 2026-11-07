using UnityEngine;
using System.Collections.Generic;

public static class GlobalData
{
    public static float tileSize = 0.4f;
    public static Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainValues = new Dictionary<int,Dictionary<int,(float height,float humidity,float temperature)>>();
}