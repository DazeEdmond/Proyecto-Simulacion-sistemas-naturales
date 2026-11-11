using System.Collections.Generic;
using System.Data;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

class Forest
{
    //attributes
    private UnityEngine.Vector2 pos;
    private float forest_radius;
    private int tree_limit;
    private List<tree> trees = new List<tree>{};
    private int generation_time;
    private int actual_time;

    private Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainVals = GlobalData.TerrainValues;
    //construct
    public Forest(float posX, float posY, float radius, int tree_limit, int time)
    {
        this.pos = new UnityEngine.Vector2(posX, posY);
        this.forest_radius = radius;
        this.tree_limit = tree_limit;
        this.generation_time = time;
        this.actual_time = UnityEngine.Random.Range(-100,0);
    }
    //math map function beacuse c# does not have it
    private float Map(float value, float low1, float max1, float low2, float max2)
    {
        return low2 + (max2 - low2) * (value - low1) / (max1 - low1);
    }

    public void updateForest()
    {
        this.actual_time++;
    }

    public UnityEngine.Vector2 getPos()
    {
        return this.pos;
    }

    public void add_Tree(tree tree)
    {
        if(this.trees.Count < this.tree_limit)
        {
            this.trees.Add(tree);
        }
    }

    public void add_First_Tree(tree tree)
    {
        if(this.trees.Count < this.tree_limit)
        {
            this.trees.Add(tree);
        }
    }

    public void reset_time()
    {
        this.actual_time = 0;
    }

    public (float, float) isvalid(){
        float tree_X = UnityEngine.Random.Range(this.pos[0]-this.forest_radius,this.pos[0]+this.forest_radius);
        float tree_y = UnityEngine.Random.Range(this.pos[1]-this.forest_radius,this.pos[1]+this.forest_radius);
            if(tree_X > 0 && tree_X < 100 && tree_y > 0 && tree_y < 100)
            {
                float terrainheigt = TerrainVals[(int)tree_y][(int)tree_X].height;
                if (terrainheigt > 0.45f && terrainheigt < 0.65f)
                {
                    return (tree_X, tree_y);
                }
            }
        return (-1f, -1f);
    }

    public void deleteRandom()
    {
        if(this.trees.Count > 0)
        {
            int randomPos = (int)UnityEngine.Random.Range(0, this.trees.Count-1);
            tree deleteThis = this.trees[randomPos];
            this.trees.RemoveAt(randomPos);
            deleteThis.DestroySelf();
        }
    }
    // 

    public bool is_Tree_Time()
    {   
        return this.actual_time >= this.generation_time && this.trees.Count < this.tree_limit;
    }
}