using UnityEngine;

public class Wanderer_man : MonoBehaviour
{
    public GameObject Man_Sprite;

    public float pos_x = 54F;
    public float pos_y = 30F;
    private Man wanderer_man;

    private GameObject SpawnedMan;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Man_Sprite.transform.localScale = new UnityEngine.Vector2(10,10);
        SpawnedMan = Instantiate(Man_Sprite, new UnityEngine.Vector2(pos_x, pos_y), UnityEngine.Quaternion.identity);

        SpawnedMan.GetComponent<SpriteRenderer> ().sortingOrder = 2;
        
        this.wanderer_man = new Man(pos_x, pos_y, Man_Sprite);
    }

    // Update is called once per frame
    void Update()
    {
        this.wanderer_man.updateMan();
        this.wanderer_man.wanderer();
        Vector2 temp = this.wanderer_man.getPos();

        SpawnedMan.transform.position = new Vector3(temp[0], temp[1], 0);
    }

}
