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

    public float Max_Man_generation_cooldown = 1200;
    public float Man_generation_cooldown = 0;

    public int Man_Log_generation_cooldown = 0;
    public float Man_Food_generation_cooldown = 0;
    public float Man_Water_generation_cooldown = 0;

    public float foodAmount;
    public float WaterAmount;
    public int ActualVillagers;


    public void init(GameObject Sprite, Vector2 pos, int nomads)
    {
        this.Sprite = Sprite;
        this.Sprite.GetComponent<SpriteRenderer>().sortingOrder = 2;
        this.VillagersAmount = nomads;
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
            if (VillageB.hasLog())
            {
                GameObject new_House = Instantiate(Sprite, new UnityEngine.Vector2(pos.Item1,pos.Item2), UnityEngine.Quaternion.identity);
                new_House.transform.localScale = new UnityEngine.Vector2(20,15);
                new_House.GetComponent<SpriteRenderer> ().sortingOrder = 2;

                VillageB.add_House(new_House);
                VillageB.reset_time();
            }
            else
            {
                if(Man_Log_generation_cooldown <= 0)
                {
                    GameObject NewFood_Man = Instantiate(ManPrefab, VillageB.getPos(), UnityEngine.Quaternion.identity);
                    NewFood_Man.GetComponent<Wanderer_man>().init(VillageB.getPos(), 3, VillageB.getPos(), this);
                    Man_Log_generation_cooldown = (int)Max_Man_generation_cooldown;
                }
            }
        }

        if (VillageB.foodNeeded())
        {
            if(Man_Food_generation_cooldown <= 0)
            {
                GameObject NewFood_Man = Instantiate(ManPrefab, VillageB.getPos(), UnityEngine.Quaternion.identity);
                NewFood_Man.GetComponent<Wanderer_man>().init(VillageB.getPos(),2, VillageB.getPos(), this);
                Man_Food_generation_cooldown = (Max_Man_generation_cooldown*(VillageB.GetFoodAmount()/5000))+180;
            }
        }

        if (VillageB.WaterNeeded())
        {
            if(Man_Water_generation_cooldown <= 0)
            {
                GameObject NewWater_Man = Instantiate(ManPrefab, VillageB.getPos(), UnityEngine.Quaternion.identity);
                NewWater_Man.GetComponent<Wanderer_man>().init(VillageB.getPos(),1, VillageB.getPos(), this);
                Man_Water_generation_cooldown = (Max_Man_generation_cooldown*(VillageB.GetFoodAmount()/5000))+180;
            }
        }

        if (VillageB.OverPopulation())
        {
            int nomad_amount = VillageB.divide()/2;
            for(int i = 0; i < nomad_amount; i++)
            {
                GameObject NewNomad_Man = Instantiate(ManPrefab, VillageB.getPos(), UnityEngine.Quaternion.identity);
                NewNomad_Man.GetComponent<Wanderer_man>().init(VillageB.getPos(),0, new Vector2(-1,-1), null);
            }
        }

        if(Man_Water_generation_cooldown > 0) Man_Water_generation_cooldown--;
        if(Man_Food_generation_cooldown > 0) Man_Food_generation_cooldown--;
        if(Man_Log_generation_cooldown > 0) Man_Log_generation_cooldown--;

        this.foodAmount = VillageB.GetFoodAmount();
        this.WaterAmount = VillageB.getwaterAmount();
        ActualVillagers = VillageB.getVillagerAmount();
    }

    public void addFood(int nutrition)
    {
        VillageB.addFood(nutrition);
    }

    public void addwater(int water)
    {
        VillageB.addWater(water);
    }

    public void addLog(int log)
    {
        VillageB.addLog(log);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
