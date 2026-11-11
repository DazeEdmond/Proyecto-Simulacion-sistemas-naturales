using UnityEngine;

class TreeBehaviour
{
    public int fruitInterval = 100;

    public float decayingTime = 100;
    public float decayingRate = 1f;
    public float actualDecayingTime = 0;
    public int actualFruit = Random.Range(-100,0);

    public TreeBehaviour(int fruitInterval, float decayingTime, float decayingRate)
    {
        this.fruitInterval = fruitInterval;
        this.decayingTime = decayingTime;
        this.decayingRate = decayingRate;
    }

    public void updateTree()
    {
        if(this.actualFruit >= this.fruitInterval)
        {
            this.actualDecayingTime = decayingTime;
            this.actualFruit = 0;
        }
        if(this.actualDecayingTime > -1f)
        {
            this.actualDecayingTime -= this.decayingRate;
        }
        else
        {
            this.actualFruit++;
        }
    }

    public bool isFruitTime()
    {
        return this.actualFruit >= (this.fruitInterval*0.90);
    }

    public bool isDyingTime()
    {
        return this.actualDecayingTime < 0;
    }

}