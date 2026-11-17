using System.Collections.Generic;
using System.Data;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

class VillageBehaviour
{
    //attributes
    private UnityEngine.Vector2 pos;
    private List<GameObject> houses = new List<GameObject>{};
    private float Village_radius;
    private int house_limit;
    private int generation_time;
    private float actual_time;

    public int MaxFoodAmount = 5000;
    public int MaxWaterAmount = 5000;
    private int foodAmount = 3000;
    private int WaterAmount = 3000;

    public int eating_drinkingRatio = 2;
    public int eating_drinking_MaxCooldown = 30;
    private int eating_drinkingCooldown = 30;
    private int VillagersAmount;
    private UnityEngine.Vector2 closestForestPos;
    private UnityEngine.Vector2 closestWaterPos;

    private Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainVals = GlobalData.TerrainValues;
    //construct
    public VillageBehaviour(UnityEngine.Vector2 pos, float radius, int Villager_Amount, int time)
    {
        this.pos = pos;
        this.Village_radius = radius;
        this.VillagersAmount = Villager_Amount;
        this.house_limit = Villager_Amount/3;
        this.generation_time = time;
        this.actual_time = UnityEngine.Random.Range(-650,0);
    }

    public void updateVillage()
    {
        this.actual_time++;
        if(eating_drinkingCooldown <= 0)
        {
            this.foodAmount -= eating_drinkingRatio*VillagersAmount;
            this.WaterAmount -= eating_drinkingRatio*VillagersAmount;
            this.eating_drinkingCooldown = eating_drinking_MaxCooldown;
        }
        else
        {
            eating_drinkingCooldown--;
        }
            
    }

    public bool foodNeeded()
    {
        return foodAmount < MaxFoodAmount/2;
    }

    public bool WaterNeeded()
    {
        return WaterAmount < MaxWaterAmount/2;
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

    public void reset_time()
    {
        this.actual_time = 0;
    }

    public int GetFoodAmount()
    {
        return foodAmount;
    }

    public int getwaterAmount()
    {
        return WaterAmount;
    }

    public (float, float) isvalid(){
        int house_X =(int) System.Math.Floor(UnityEngine.Random.Range(this.pos[0]-this.Village_radius,this.pos[0]+this.Village_radius));
        int house_y =(int) System.Math.Floor(UnityEngine.Random.Range(this.pos[1]-this.Village_radius,this.pos[1]+this.Village_radius));
            if(house_X > 0 && house_X < 100 && house_y > 0 && house_y < 100)
            {
                float terrainheigt = TerrainVals[house_y][house_X].height;
                if (terrainheigt > 0.3f && terrainheigt < 0.65f)
                {
                    bool valid = true;
                    foreach(GameObject h in houses)
                    {
                        if((h.transform.position.x == house_X || h.transform.position.x + 1 == house_X || h.transform.position.x - 1 == house_X)
                        && h.transform.position.y == house_y)
                    {
                        valid = false;
                    }
                    }
                    if (valid)
                    {
                        return (house_X, house_y);
                    }
                }
            }
        return (-1f, -1f);
    }

    public bool is_House_Time()
    {   
        return this.actual_time >= this.generation_time && this.houses.Count < this.house_limit;
    }
}