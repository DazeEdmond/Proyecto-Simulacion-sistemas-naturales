using Unity.Collections;
using UnityEngine;

public class village : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Sprite;
    private VillageBehaviour VillageB;
    public float Village_radius;
    public int generation_time;
    public int VillagersAmount;
    private UnityEngine.Vector2 closestForestPos;
    private UnityEngine.Vector2 closestWaterPos;

    public void init(GameObject Sprite, Vector2 pos)
    {
        this.Sprite = Sprite;
        this.VillageB = new VillageBehaviour(pos, Village_radius, VillagersAmount, generation_time);
    }
    void Start()
    {
        Sprite.transform.localScale = new UnityEngine.Vector2(20,15);
    }

    void FixedUpdate()
    {
        VillageB.updateVillage();
        (float, float) pos = VillageB.isvalid();
        if (VillageB.is_House_Time() && pos != (-1f,-1f))
        {
            GameObject new_House = Instantiate(Sprite, new UnityEngine.Vector2(pos.Item1,pos.Item2), UnityEngine.Quaternion.identity);
            new_House.transform.localScale = new UnityEngine.Vector2(20,15);
            new_House.GetComponent<SpriteRenderer> ().sortingOrder = 2;

            VillageB.add_House(new_House);
            VillageB.reset_time();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
