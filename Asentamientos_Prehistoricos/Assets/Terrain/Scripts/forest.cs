using System.Collections.Generic;
using UnityEngine;

class Forest
{
    //attributes
    private Vector2 pos;
    private float forest_radius;
    private int tree_limit;
    private List<tree> trees = new List<tree>();
    private int generation_time;
    private int actual_time;
    private bool hadTrees = false;

    //construct
    public Forest(float posX, float posY, float radius, int tree_limit, int time)
    {
        this.pos = new Vector2(posX, posY);
        this.forest_radius = radius;
        this.tree_limit = tree_limit;
        this.generation_time = time;
        // Desfase inicial para que los bosques no crezcan todos a la vez
        this.actual_time = Random.Range(-time, 0);
    }

    public Vector2 getPos()
    {
        return this.pos;
    }

    public int TreeCount
    {
        get { return this.trees.Count; }
    }

    // Un bosque que tuvo árboles y se quedó sin ninguno ya no puede regenerarse
    public bool IsExtinct
    {
        get { return this.hadTrees && this.trees.Count == 0; }
    }

    public void updateForest()
    {
        this.actual_time++;
    }

    public void reset_time()
    {
        this.actual_time = 0;
    }

    public void add_Tree(tree t)
    {
        if (t == null || this.trees.Count >= this.tree_limit) return;

        this.trees.Add(t);
        t.Removed += OnTreeRemoved;
        this.hadTrees = true;
    }

    // El árbol avisa cuando se destruye (talado o eliminado)
    private void OnTreeRemoved(tree t)
    {
        this.trees.RemoveAll(x => x == null || ReferenceEquals(x, t));
    }

    public bool is_Tree_Time()
    {
        return this.trees.Count > 0
            && this.trees.Count < this.tree_limit
            && this.actual_time >= this.generation_time;
    }

    // Busca una posición para un árbol nuevo:
    //  - cerca de un árbol existente (distancia gaussiana)
    //  - dentro del radio del bosque
    //  - con altura y humedad adecuadas
    //  - sin encimarse con otros árboles
    public bool TryGetNewTreePosition(out Vector2 position)
    {
        position = Vector2.zero;
        if (this.trees.Count == 0) return false;

        tree parent = this.trees[Random.Range(0, this.trees.Count)];
        if (parent == null) return false;

        Vector2 origin = parent.transform.position;
        float sigma = this.forest_radius / 2f;

        float x = GlobalData.RandomGaussian(origin.x, sigma);
        float y = GlobalData.RandomGaussian(origin.y, sigma);
        Vector2 candidate = new Vector2(x, y);

        if (Vector2.Distance(candidate, this.pos) > this.forest_radius) return false;
        if (!GlobalData.IsValidTreeSpot(x, y)) return false;

        foreach (tree t in this.trees)
        {
            if (t != null && Vector2.Distance(candidate, t.transform.position) < GlobalData.treeMinSpacing)
            {
                return false;
            }
        }

        position = candidate;
        return true;
    }

    // Elimina un árbol al azar del bosque
    public void deleteRandom()
    {
        if (this.trees.Count == 0) return;

        tree victim = this.trees[Random.Range(0, this.trees.Count)];
        if (victim != null) victim.DestroySelf();
    }
}
