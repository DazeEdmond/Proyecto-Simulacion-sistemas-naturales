using UnityEngine;
using System.Collections.Generic;

public enum Param { ForestLimit, ForestRadius, ForestGeneration, MaxLife, LifeLoss,
                    VillageRadius, ThirstLoss, HungerLoss, MaxSpeed, FruitTime }

public static class GlobalData
{
    public static float tileSize = 0.4f;
    public static Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainValues = new Dictionary<int,Dictionary<int,(float height,float humidity,float temperature)>>();

    // Sliders - GlobalData
    static Dictionary<Param,float> settings = new Dictionary<Param,float> {
        { Param.ForestLimit,      25 },     { Param.ForestRadius,   4 }, 
        { Param.ForestGeneration, 100 },    { Param.MaxLife,        10000 },  
        { Param.LifeLoss,   0.1f },         { Param.VillageRadius,  10 }, 
        { Param.ThirstLoss, 0.1f },         { Param.HungerLoss, 0.1f },
        { Param.MaxSpeed,   0.18f },        { Param.FruitTime,  100 },
    }; // Por defecto
    public static float Get(Param p) { return settings[p]; }
    public static void Set(Param p, float v) { settings[p] = v; }
    
    // Man - GlobalData - Espera generación de Mapa
    public static bool mapReady = false;

    // Seed - GlobalData
    public static int globalSeed = 0;

    public static float Map(float value, float low1, float max1, float low2, float max2)
    {
        return low2 + (max2 - low2) * (value - low1) / (max1 - low1);
    }
}