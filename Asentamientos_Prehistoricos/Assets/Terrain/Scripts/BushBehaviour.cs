using UnityEngine;

class BushBehaviour
{
    public int fruitInterval;
    public int nutritionValue;
    public int fruitTimer;
    public bool hasFruit = false;

    public BushBehaviour(int fruitInterval, int nutritionValue)
    {
        this.fruitInterval = Mathf.Max(1, fruitInterval);
        this.nutritionValue = nutritionValue;
        // Desfasa los arbustos para que no den fruta todos al mismo tiempo
        this.fruitTimer = Random.Range(0, this.fruitInterval);
    }

    // Se llama en FixedUpdate
    public void updateBush()
    {
        if (this.hasFruit) return;

        this.fruitTimer++;
        if (this.fruitTimer >= this.fruitInterval)
        {
            this.hasFruit = true;
            this.fruitTimer = 0;
        }
    }

    // Devuelve el valor nutricional y reinicia el ciclo (0 si no había fruta)
    public int TryHarvest()
    {
        UnityEngine.Debug.Log("Fuck on");
        if (!this.hasFruit) return 0;

        this.hasFruit = false;
        this.fruitTimer = 0;
        return this.nutritionValue;
    }
}
