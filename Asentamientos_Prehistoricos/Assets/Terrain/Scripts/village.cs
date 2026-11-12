using System.Collections.Generic;
using System.Data;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

class Village
{
    //attributes
    private UnityEngine.Vector2 pos;
    private float Village_radius;
    private int house_limit;
    private List<GameObject> houses = new List<GameObject>{};
    private int generation_time;
    private float actual_time;

    private int foodAmount = 500;
    private int WaterAmount = 500;
    private int VillagersAmount;
    private UnityEngine.Vector2 closestForestPos;
    private UnityEngine.Vector2 closestWaterPos;

    private Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainVals = GlobalData.TerrainValues;
    //construct
    public Village(float posX, float posY, float radius, int Villager_Amount, int time)
    {
        this.pos = new UnityEngine.Vector2(posX, posY);
        this.Village_radius = radius;
        this.VillagersAmount = Villager_Amount;
        this.house_limit = Villager_Amount/3;
        this.generation_time = time;
        this.actual_time = UnityEngine.Random.Range(-100,0);
    }

    public void updateVillage()
    {
        this.actual_time++;
    }

    public UnityEngine.Vector2 getPos()
    {
        return this.pos;
    }

    public void add_House(GameObject house)
    {
        if(this.houses.Count < this.house_limit)
        {
            this.houses.Add(house);
        }
    }

    // public void add_First_Tree(tree tree)
    // {
    //     if(this.trees.Count < this.tree_limit)
    //     {
    //         this.trees.Add(tree);
    //     }
    // }

    public void reset_time()
    {
        this.actual_time = 0;
    }

    public (float, float) isvalid(){
        float house_X = UnityEngine.Random.Range(this.pos[0]-this.Village_radius,this.pos[0]+this.Village_radius);
        float house_y = UnityEngine.Random.Range(this.pos[1]-this.Village_radius,this.pos[1]+this.Village_radius);
            if(house_X > 0 && house_X < 100 && house_y > 0 && house_y < 100)
            {
                float terrainheigt = TerrainVals[(int)house_y][(int)house_X].height;
                if (terrainheigt > 0.3f && terrainheigt < 0.65f)
                {
                    return (house_X, house_y);
                }
            }
        return (-1f, -1f);
    }

    public bool is_House_Time()
    {   
        return this.actual_time >= this.generation_time && this.houses.Count < this.house_limit;
    }
}