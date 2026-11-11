using UnityEngine;

public class tree : MonoBehaviour
{
    public GameObject Sprite;
    public GameObject BushSprite;

    public Sprite[] sprites;
    private TreeBehaviour TreeB;
    public int fruitInterval = 100;

    public float decayingTime = 1300;
    public float decayingRate = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void init(GameObject Sprite)
    {
        this.Sprite = Sprite;
        this.TreeB = new TreeBehaviour(this.fruitInterval, this.decayingTime, this.decayingRate);
    }
    void Start()
    {
        Sprite.transform.localScale = new UnityEngine.Vector2(10,10);
    }

    private void FixedUpdate() {
        this.TreeB.updateTree();
    }
    // Update is called once per frame
    void Update()
    {
        if (this.TreeB.isFruitTime())
        {
            this.GetComponent<SpriteRenderer>().sprite = sprites[1];
            return;
        }

        if (this.TreeB.isDyingTime())
        {
            this.GetComponent<SpriteRenderer> ().sprite = sprites[0];
        }
    }

    public void DestroySelf()
    {
        Destroy(this);
    }
}
