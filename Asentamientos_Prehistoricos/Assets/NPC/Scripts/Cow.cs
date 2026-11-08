using System.Diagnostics;
using System.Numerics;
using UnityEngine;

public class Cow : MonoBehaviour
{
    public GameObject sprite;
    public GameObject cowSprite;
    private CowBehaviour CB;
    public UnityEngine.Vector2 defaultVel = new UnityEngine.Vector2(0.01f,0);

    public void init(GameObject sprite, UnityEngine.Vector2 pos, UnityEngine.Vector2 vel,int cowQuan)
    {
        this.sprite = sprite;
        this.CB = new CowBehaviour(pos, vel, UnityEngine.Vector2.zero, cowQuan);
    }

    void Start()
    {
        sprite.transform.localScale = new UnityEngine.Vector2(10,10);
        cowSprite = Instantiate(sprite, new UnityEngine.Vector2(0,0), UnityEngine.Quaternion.identity);
        cowSprite.GetComponent<SpriteRenderer> ().sortingOrder = 2;
    }

private void FixedUpdate() {
    CB.updateCow();
}

    // Update is called once per frame
    void Update()
    {
        if (CB.lookingRight())
        {
            cowSprite.transform.localScale = new UnityEngine.Vector2(-10,10);
        }
        else
        {
            cowSprite.transform.localScale = new UnityEngine.Vector2(10,10);
        }
        UnityEngine.Vector2 cowPos = CB.getPos();

        this.transform.position = new UnityEngine.Vector3(cowPos[0], cowPos[1], 0);
        cowSprite.transform.position = new UnityEngine.Vector3(cowPos[0], cowPos[1], 0);
    }

    public UnityEngine.Vector2 getPos()
    {
        return CB.getPos();
    }
    public UnityEngine.Vector2 getVel()
    {
        return CB.getVel();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Cow")
        {
            CB.addNearCow();
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(other.gameObject.tag == "Cow")
        {
            Cow cow = other.gameObject.GetComponent<Cow>();
            CB.cohereWithCow(cow.getPos(),cow.getVel());
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Cow")
        {
            CB.popNearCow();
        }
    }
}
