using UnityEngine;

class TreeBehaviour
{
    public int fruitInterval;
    public int nutritionValue;
    public int fruitTimer;
    public bool hasFruit = false;

    public TreeBehaviour(int fruitInterval, int nutritionValue)
    {
        this.fruitInterval = Mathf.Max(1, fruitInterval);
        this.nutritionValue = nutritionValue;

        this.fruitTimer = Random.Range(0, this.fruitInterval);
    }

    public void updateTree()
    {
        if (this.hasFruit) return;

        this.fruitTimer++;
        if (this.fruitTimer >= this.fruitInterval)
        {
            this.hasFruit = true;
            this.fruitTimer = 0;
        }
    }

    public int TryHarvest()
    {
        if (!this.hasFruit) return 0;

        this.hasFruit = false;
        this.fruitTimer = 0;
        return this.nutritionValue;
    }
}
