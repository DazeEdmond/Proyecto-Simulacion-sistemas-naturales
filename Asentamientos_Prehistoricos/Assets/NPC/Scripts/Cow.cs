using System.Numerics;
using UnityEngine;

public class Cow : MonoBehaviour
{
    public GameObject sprite;
    public GameObject cowSprite;
    private CowBehaviour CB;
    public UnityEngine.Vector2 defaultVel = new UnityEngine.Vector2(0.01f,0);

    public void init(GameObject sprite, UnityEngine.Vector2 pos, UnityEngine.Vector2 vel)
    {
        this.sprite = sprite;
        this.CB = new CowBehaviour(pos, vel, UnityEngine.Vector2.zero);
    }

    void Start()
    {
        sprite.transform.localScale = new UnityEngine.Vector2(10,10);
        cowSprite = Instantiate(sprite, new UnityEngine.Vector2(0,0), UnityEngine.Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        CB.updateCow();
        CB.wander();
        UnityEngine.Vector2 cowPos = CB.getPos();

        cowSprite.transform.position = new UnityEngine.Vector3(cowPos[0], cowPos[1], 0);
    }
}
