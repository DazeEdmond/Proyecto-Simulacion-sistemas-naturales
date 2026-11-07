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
    private List<GameObject> trees = new List<GameObject>{};
    private int generation_time;
    private int actual_time = 0;
    //construct
    public Forest(float posX, float posY, float radius, int tree_limit, int time)
    {
        this.pos = new UnityEngine.Vector2(posX, posY);
        this.forest_radius = radius;
        this.tree_limit = tree_limit;
        this.generation_time = time;
        this.actual_time = UnityEngine.Random.Range(0,time);
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

    public void add_Tree(GameObject tree)
    {
        if(this.trees.Count < this.tree_limit)
        {
            float tree_X = UnityEngine.Random.Range(this.pos[0]-this.forest_radius,this.pos[0]+this.forest_radius);
            float tree_y = UnityEngine.Random.Range(this.pos[1]-this.forest_radius,this.pos[1]+this.forest_radius);
            tree.transform.position = new UnityEngine.Vector3(tree_X, tree_y, 0);
            this.trees.Add(tree);
        }
    }

    public void add_First_Tree(GameObject tree)
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

    public bool is_Tree_Time()
    {   
        return this.actual_time >= this.generation_time;
    }
}