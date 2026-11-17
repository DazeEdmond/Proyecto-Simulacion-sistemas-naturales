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
        { Param.ForestGeneration, 900 },    { Param.MaxLife,        10000 },  
        { Param.LifeLoss,   0.1f },         { Param.VillageRadius,  5 }, 
        { Param.ThirstLoss, 0.1f },         { Param.HungerLoss, 0.1f },
        { Param.MaxSpeed,   0.025f },        { Param.FruitTime,  600 },
    }; // Por defecto
    public static float Get(Param p) { return settings[p]; }
    public static void Set(Param p, float v) { settings[p] = v; }
    
    // Man - GlobalData - Espera generación de Mapa
    public static bool mapReady = false;

    // Seed - GlobalData
    public static int globalSeed = 0;

    // Tamaño del mapa (lo escribe Generation.Start)
    public static int mapWidth = 100;
    public static int mapHeight = 100;

    // ---------------- Vegetación ----------------
    // Valor nutricional fijo
    public const int TreeNutrition = 1;
    public const int BushNutrition = 2;

    // Condiciones donde pueden nacer árboles nuevos (modificables luego desde sliders)
    public static float treeMinHeight = 0.3f;
    public static float treeMaxHeight = 0.65f;
    public static float treeMinHumidity = 0.3f;
    public static float treeMaxHumidity = 0.6f;
    // Distancia mínima entre árboles de un mismo bosque
    public static float treeMinSpacing = 0.5f;

    public static float Map(float value, float low1, float max1, float low2, float max2)
    {
        return low2 + (max2 - low2) * (value - low1) / (max1 - low1);
    }

    // Distribución gaussiana 
    public static float RandomGaussian(float mean, float stdDev)
    {
        float u1 = 1f - UnityEngine.Random.value; // (0, 1], evita Log(0)
        float u2 = UnityEngine.Random.value;
        float z = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
        return mean + z * stdDev;
    }

    // Lectura segura de los valores del terreno en una posición del mundo
    public static bool TryGetTerrain(float x, float y, out (float height, float humidity, float temperature) values)
    {
        values = default;
        if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight) return false;
        if (!TerrainValues.TryGetValue((int)y, out var row)) return false;
        return row.TryGetValue((int)x, out values);
    }

    // ¿Puede crecer un árbol en esta posición según altura y humedad?
    public static bool IsValidTreeSpot(float x, float y)
    {
        if (!TryGetTerrain(x, y, out var t)) return false;
        return t.height > treeMinHeight && t.height < treeMaxHeight
            && t.humidity > treeMinHumidity && t.humidity < treeMaxHumidity;
    }
}
