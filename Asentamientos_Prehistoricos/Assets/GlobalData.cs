using UnityEngine;
using System.Collections.Generic;

// Enumeration - Sliders existentes
public enum Param { ForestLimit, ForestRadius, ForestGeneration, MaxLife, LifeLoss,
                    VillageRadius, ThirstLoss, HungerLoss, MaxSpeed, FruitTime,
                    WaterDown, Water, WaterUp, GrassDown, Grass, GrassUp,
                    MountainDown, Mountain, MountainUp }

public static class GlobalData
{
    public static Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainValues = new Dictionary<int,Dictionary<int,(float height,float humidity,float temperature)>>();

    // Sliders - Dic<TipoSlider, ValorSlider>
    static Dictionary<Param,float> settings = new Dictionary<Param,float> {
        // Agentes
        { Param.ForestLimit,      25 },     { Param.ForestRadius,   4 }, 
        { Param.ForestGeneration, 900 },    { Param.MaxLife,        10000 },  
        { Param.LifeLoss,   0.1f },         { Param.VillageRadius,  10 }, 
        { Param.ThirstLoss, 0.1f },         { Param.HungerLoss, 0.1f },
        { Param.MaxSpeed,   0.18f },        { Param.FruitTime,  100 },
        
        // Terreno
        { Param.WaterDown, 0.02f },    { Param.Water, 0.12f },     { Param.WaterUp, 0.23f },
        { Param.GrassDown, 0.3f },    { Param.Grass, 0.46f },     { Param.GrassUp, 0.68f },
        { Param.MountainDown, 0.76f }, { Param.Mountain, 0.83f },  { Param.MountainUp, 1f },
    };

    // Sliders - Métodos getset 
    public static float Get(Param p) { return settings[p]; }
    public static void Set(Param p, float v) { settings[p] = v; }

    // Sliders - Terreno acotado - Método Josué
    public static float Discriminator(Param p)
    {
        // int k = (int)p - (int)Param.WaterDown;
        // return k * 0.1f + Get(p);
        return Get(p);
    }

    // GUI - Saltar primer slide
    public static bool skipIntro = false;
    
    // GUI - Espera generación de Mapa
    public static bool mapReady = false;

    // Seed - Si es positivo, Valor determinístico
    public static int globalSeed = 0;

    // Tamaño del mapa
    public static int mapWidth = 100;
    public static int mapHeight = 100;

    // ---------------- Vegetación ----------------
    // Valor nutricional fijo
    public const int TreeNutrition = 1;
    public const int BushNutrition = 2;

    // Condiciones donde pueden nacer árboles nuevos (modificables luego desde sliders)
    public static float treeMinHeight = GlobalData.Discriminator(Param.GrassDown);
    public static float treeMaxHeight = GlobalData.Discriminator(Param.GrassUp);
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
