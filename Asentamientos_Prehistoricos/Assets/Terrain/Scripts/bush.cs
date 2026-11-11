using System.Diagnostics;
using System.Numerics;
using UnityEngine;

public class bush : MonoBehaviour
{
    public GameObject Sprite;
    public GameObject BushSprite;

    public Sprite[] sprites;
    private BushBehaviour BushC;
    public int fruitInterval = 100;

    public float decayingTime = 100;
    public float decayingRate = 1f;
    public int actualFruit;
    public float actualDecayingTime;
    public bool istime;
    public bool isdead;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void init(GameObject Sprite)
    {
        this.Sprite = Sprite;
        this.BushC = new BushBehaviour(this.fruitInterval, this.decayingTime, this.decayingRate);
    }
    void Start()
    {
        Sprite.transform.localScale = new UnityEngine.Vector2(10,10);
    }

    private void FixedUpdate() {
        this.BushC.updateBush();
    }
    // Update is called once per frame
    void Update()
    {
        actualFruit = this.BushC.actualFruit;
        actualDecayingTime = this.BushC.actualDecayingTime;
        istime = this.BushC.isFruitTime();
        isdead = this.BushC.isDyingTime();
        if (this.BushC.isFruitTime())
        {
            this.GetComponent<SpriteRenderer>().sprite = sprites[1];
            return;
        }

        if (this.BushC.isDyingTime())
        {
            this.GetComponent<SpriteRenderer> ().sprite = sprites[0];
        }
    }
}
