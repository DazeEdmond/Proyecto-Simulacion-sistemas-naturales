using NPCs;
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
    public GameObject ManPrefab;

    public int Max_Man_generation_cooldown = 1200;
    public int Man_generation_cooldown = 0;

    public int foodAmount;
    public int WaterAmount;


    public void init(GameObject Sprite, Vector2 pos)
    {
        this.Sprite = Sprite;
        this.Sprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
        this.VillageB = new VillageBehaviour(pos, GlobalData.Get(Param.VillageRadius), VillagersAmount, generation_time); // Slider Value
    }
    void Start()
    {
        Sprite.transform.localScale = new UnityEngine.Vector2(20,15);
        Sprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
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

        if (VillageB.foodNeeded())
        {
            if(Man_generation_cooldown <= 0)
            {
                GameObject NewFood_Man = Instantiate(ManPrefab, VillageB.getPos(), UnityEngine.Quaternion.identity);
                // NewFood_Man.transform.localScale = new UnityEngine.Vector2(10,10);
                // NewFood_Man.GetComponent<SpriteRenderer> ().sortingOrder = 2;
                NewFood_Man.GetComponent<Wanderer_man>().init(VillageB.getPos());
                Man_generation_cooldown = Max_Man_generation_cooldown;
            }
        }

        if (VillageB.WaterNeeded())
        {
            if(Man_generation_cooldown <= 0)
            {
                GameObject NewWater_Man = Instantiate(ManPrefab, VillageB.getPos(), UnityEngine.Quaternion.identity);
                // NewWater_Man.transform.localScale = new UnityEngine.Vector2(10,10);
                // NewWater_Man.GetComponent<SpriteRenderer> ().sortingOrder = 2;
                NewWater_Man.GetComponent<Wanderer_man>().init(VillageB.getPos());
                Man_generation_cooldown = Max_Man_generation_cooldown;
            }
        }

        if(Man_generation_cooldown > 0) Man_generation_cooldown--;

        this.foodAmount = VillageB.GetFoodAmount();
        this.WaterAmount = VillageB.getwaterAmount();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
