using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace NPCs
{
    public class Wanderer_man : MonoBehaviour
    {
        //Atributes
        public GameObject Man_Sprite;
        public Bar HB;
        public Bar thirstB;
        public Bar hungerB;
        public GameObject hat;

        public Sprite[] hats;
        private GameObject inst_Hat;

        public GameObject flag;
        public Sprite[] flags;
        private GameObject inst_flag;
        public float pos_x;
        public float pos_y;
        private UnityEngine.Vector2 barScale = new Vector2(0.1f, 0.1f);

        //Life Atributes
        private enum State
        {
            Wander,
            SearchingWater,
            DrinkingWater,
            searchingFood,
        }
        [SerializeField] private State actualState = State.Wander;

        public enum Job
        {
            nomad,
            FoodGatherer,
            WaterGatherer,
            LogGatherer,
            citicen
        }
        [SerializeField] public Job actualJob;
        public bool chilling = false;
        public float maxVel = GlobalData.Get(Param.MaxSpeed);
        public float maxLife = GlobalData.Get(Param.MaxLife);
        public float life = GlobalData.Get(Param.MaxLife);
        public float lifeLoss = GlobalData.Get(Param.LifeLoss);
        public float maxThirst = 1000;
        public float thirst = 1000;
        public float thirstLoss = GlobalData.Get(Param.ThirstLoss);
        public UnityEngine.Vector2 nearestWater = new UnityEngine.Vector2(-1,-1);
        public float maxHunger = 2000;
        public float hunger = 2000;
        public float hungerLoss = GlobalData.Get(Param.HungerLoss);
        public UnityEngine.Vector2 nearestFood = new UnityEngine.Vector2(-1,-1);
        public Cow nearestCow = null;
        public tree nearestree = null;
        public bush nearestBush = null;
        public bool nearFood = false;
        public int damage = 20;
        public float attackRadio = 1;
        public float attackMaxCooldown = 100;
        public float attackCooldown = 0;

        public float desiredHeight;
        public float desiredHumidity;
        public float desiredTemp;
        public float perfectPlace;
        public int seekingCoolDown = 300;
        public int actualSeekingCoolDown = 300;
        public Vector2 seekingPos = new UnityEngine.Vector2(-1,-1);
        public GameObject village;
        public GameObject VillagePrefab;

        public float actualPerfect = -1;

        public Vector2 House_location;
        private village home;
        public int food2village = 0;
        public int water2village = 0;
        public int log2village = 0;
        public bool haslog = false;
        public bool hasWater = false;
        public bool hasfood = false;
        private int nomadGroup = 0;
        private bool cAsentamiento = false;
        private bool checkedNomads = false;
        private int wanderingCiticen = 1050;
        private Man wanderer_man;

        private GameObject SpawnedMan;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void init(Vector2 pos, int job, Vector2 House_location, village home)
        {
            Man_Sprite.transform.position = pos;
            this.pos_x = pos[0];
            this.pos_y = pos[1];
            this.House_location = House_location;
            this.home = home;
            this.actualJob = job switch
            {
                0 => Job.nomad,
                1 => Job.WaterGatherer,
                2 => Job.FoodGatherer,
                3 => Job.LogGatherer,
                4 => Job.citicen,
                _ => Job.nomad,
            };
        }
        void Start()
        {
            this.Man_Sprite.transform.localScale = new UnityEngine.Vector2(10,10);
            this.hat.transform.localScale = new UnityEngine.Vector2(10,10);
            this.flag.transform.localScale = new UnityEngine.Vector2(0,0);

            this.SpawnedMan = Instantiate(this.Man_Sprite, new UnityEngine.Vector2(this.pos_x, this.pos_y), UnityEngine.Quaternion.identity);

            this.inst_Hat = Instantiate(this.hat, new UnityEngine.Vector2(this.pos_x-0.1f, this.pos_y+0.3f), UnityEngine.Quaternion.identity);
            this.inst_Hat.GetComponent<SpriteRenderer> ().sortingOrder = 102-(int)this.pos_y;

            this.SpawnedMan.GetComponent<SpriteRenderer> ().sortingOrder = 102-(int)this.pos_y;

            this.inst_flag = Instantiate(this.flag, new UnityEngine.Vector2(this.pos_x, this.pos_y+1.6f), UnityEngine.Quaternion.identity);
            this.inst_flag.GetComponent<SpriteRenderer> ().sortingOrder = -1;

            if(actualJob == Job.citicen) this.inst_Hat.GetComponent<SpriteRenderer>().sprite = hats[0];
            if(actualJob == Job.nomad) this.inst_Hat.GetComponent<SpriteRenderer>().sprite = hats[1];
            if(actualJob == Job.WaterGatherer) this.inst_Hat.GetComponent<SpriteRenderer>().sprite = hats[2];
            if(actualJob == Job.FoodGatherer) this.inst_Hat.GetComponent<SpriteRenderer>().sprite = hats[3];
            if(actualJob == Job.LogGatherer) this.inst_Hat.GetComponent<SpriteRenderer>().sprite = hats[4];
            
            // Sliders
            maxLife = GlobalData.Get(Param.MaxLife);
            life = maxLife;
            lifeLoss = GlobalData.Get(Param.LifeLoss);
            thirstLoss = GlobalData.Get(Param.ThirstLoss);
            hungerLoss = GlobalData.Get(Param.HungerLoss);
            maxVel = GlobalData.Get(Param.MaxSpeed);

            this.wanderer_man = new Man(this.pos_x, this.pos_y, this.maxVel);
            this.HB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1,0);
            this.HB.transform.localScale = this.barScale;

            this.thirstB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.2f,0);
            this.thirstB.transform.localScale = this.barScale;

            this.hungerB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.4f,0);
            this.hungerB.transform.localScale = this.barScale;

            this.desiredHeight = Random.Range(GlobalData.Discriminator(Param.GrassDown), GlobalData.Discriminator(Param.MountainDown));
            this.desiredHumidity = Random.Range(0.3f, 0.6f);
            this.desiredTemp = Random.Range(10.0f, 30.0f);

            this.perfectPlace =((desiredHeight*30) + (desiredHumidity*30) + desiredTemp)/2;
        }

        private void FixedUpdate() {
            if (this.wanderer_man == null) return;
            
            int actualNomads = this.nomadGroup;
            this.nomadGroup = 0;

            if (this.checkedNomads)
            {
                this.KMS();
                return;
            }

            this.chilling = this.wanderer_man.isTired();

            this.nearFood = false;

            //Is Thirsty?
            if((this.actualState != State.SearchingWater || this.actualState != State.DrinkingWater)&&thirst <=maxThirst/2)
            {
                this.actualState = State.SearchingWater;
            }
            //Is Hungry
            if((this.actualState != State.searchingFood || this.actualState != State.SearchingWater) && hunger <=maxHunger/2)
            {
                this.actualState = State.searchingFood;
            }

            //STATES BEHAVIOURS
            if(this.actualState == State.SearchingWater)
            {
                this.inst_Hat.GetComponent<SpriteRenderer>().sprite = hats[2];
                if(this.nearestWater.x != -1 && this.nearestWater.y != -1)
                {
                    this.wanderer_man.seek(this.nearestWater);
                    this.wanderer_man.updatePos();
                    if (this.wanderer_man.onWater())
                    {
                        this.actualState = State.DrinkingWater;
                    }
                    return;
                }
            }

            if(this.actualState == State.searchingFood)
            {
                this.inst_Hat.GetComponent<SpriteRenderer>().sprite = hats[3];
                followFood();
                return;
            }

            if(this.actualState == State.DrinkingWater)
            {
                return;
            }

            if(this.actualJob == Job.nomad)
            {
                this.inst_Hat.GetComponent<SpriteRenderer>().sprite = hats[1];
                if(actualPerfect > 0 || actualSeekingCoolDown <= 0)
                {
                    List<((float, float, float), Vector2)> tempTerrain = wanderer_man.surroundingTerrain();
                    Vector2 ManPos = wanderer_man.getPos();
                    foreach (((float, float, float), Vector2) t in tempTerrain){
                        (float, float, float) values = t.Item1;
                        Vector2 pos = t.Item2;
                        float TempPlace =((values.Item1*40) + (values.Item2*40) + values.Item3)/3;
                        if(TempPlace >= this.perfectPlace*0.8f)
                        {
                            if(TempPlace >= actualPerfect && values.Item1 > GlobalData.Discriminator(Param.GrassDown) && values.Item1 < GlobalData.Discriminator(Param.MountainDown))
                            {
                                actualPerfect = TempPlace;
                                seekingPos = pos;
                            }
                        }
                    }
                    if(seekingPos[0] != -1)
                    {
                        this.wanderer_man.seek(seekingPos);
                        this.wanderer_man.updatePos();
                        if(wanderer_man.getPos() == seekingPos)
                        {
                            this.actualSeekingCoolDown = seekingCoolDown;
                            actualPerfect = -1;
                            seekingPos = new UnityEngine.Vector2(-1,-1);
                        }
                    }else
                    {
                        seekingPos = new UnityEngine.Vector2(-1,-1);
                        actualPerfect = -1;
                    }

                    if(actualPerfect >= perfectPlace*0.95f)
                    {
                        GameObject new_Village = Instantiate(VillagePrefab, ManPos, UnityEngine.Quaternion.identity);
                        village otherVillage = new_Village.GetComponent<village>();
                        otherVillage.init(village, ManPos,actualNomads+1);
                        this.cAsentamiento = true;
                    }
                    else
                    {
                        this.wanderer_man.updateMan();
                        this.actualSeekingCoolDown = seekingCoolDown;
                    }
                }
                if(actualSeekingCoolDown > 0)
                {
                    actualSeekingCoolDown --;
                }
            }

            if(actualJob == Job.LogGatherer)
            {
                if (!haslog)
                {
                    if (this.nearestree != null)
                    {
                        wanderer_man.seek(nearestree.transform.position);
                        this.wanderer_man.updatePos();
                        UnityEngine.Vector2 pos = this.wanderer_man.getPos();
                        if (UnityEngine.Vector2.Distance(pos,this.nearestree.transform.position)<this.attackRadio && this.attackCooldown >= this.attackMaxCooldown)
                        {
                            this.attackCooldown = 0;
                            this.log2village += this.nearestree.Chop();
                            this.nearestree = null;
                            haslog = true;
                        }
                    }
                    else
                    {
                        this.wanderer_man.wander();
                        this.wanderer_man.updatePos();
                    }
                }
                else
                {
                    this.inst_flag.GetComponent<SpriteRenderer> ().sortingOrder = 102-(int)this.pos_y;
                    this.inst_flag.GetComponent<SpriteRenderer>().sprite = flags[2];
                    this.inst_flag.transform.localScale = new Vector2(10,10);
                    wanderer_man.seek(House_location);
                    this.wanderer_man.updatePos();
                    if(UnityEngine.Vector2.Distance(wanderer_man.getPos(),this.House_location)<1)
                    {
                        home.addLog(log2village);
                        KMS();
                    }
                }
            }

            if(actualJob == Job.WaterGatherer)
            {
                if (!hasWater)
                {
                    if (this.nearestWater != null)
                    {
                        this.wanderer_man.seek(this.nearestWater);
                        this.wanderer_man.updatePos();
                        UnityEngine.Vector2 pos = this.wanderer_man.getPos();
                        if (UnityEngine.Vector2.Distance(pos,this.nearestWater)<this.attackRadio && this.attackCooldown >= this.attackMaxCooldown)
                        {
                            this.attackCooldown = 0;
                            this.water2village += 100;
                            hasWater = true;
                        }
                    }
                    else
                    {
                        this.wanderer_man.wander();
                        this.wanderer_man.updatePos();
                    }
                }
                else
                {
                    this.inst_flag.GetComponent<SpriteRenderer> ().sortingOrder = 102-(int)this.pos_y;
                    this.inst_flag.GetComponent<SpriteRenderer>().sprite = flags[0];
                    this.inst_flag.transform.localScale = new Vector2(10,10);
                    wanderer_man.seek(House_location);
                    this.wanderer_man.updatePos();
                    if(UnityEngine.Vector2.Distance(wanderer_man.getPos(),this.House_location)<1)
                    {
                        home.addwater(water2village);
                        KMS();
                    }
                }
            }
            if(actualJob == Job.FoodGatherer)
            {
                if (!hasfood)
                {
                    followFood();
                }
                else
                {
                    this.inst_flag.GetComponent<SpriteRenderer> ().sortingOrder = 102-(int)this.pos_y;
                    this.inst_flag.GetComponent<SpriteRenderer>().sprite = flags[1];
                    this.inst_flag.transform.localScale = new Vector2(10,10);
                    wanderer_man.seek(House_location);
                    this.wanderer_man.updatePos();
                    if(UnityEngine.Vector2.Distance(wanderer_man.getPos(),this.House_location)<1)
                    {
                        home.addFood(food2village);
                        KMS();
                    }
                }
            }
            if(actualJob == Job.citicen)
            {
                if(this.wanderingCiticen > 0)
                {
                    if(UnityEngine.Vector2.Distance(wanderer_man.getPos(),this.House_location)>GlobalData.Get(Param.VillageRadius))
                    {
                        wanderer_man.seek(House_location);
                        this.wanderer_man.updatePos();
                    }
                    else
                    {
                        this.wanderer_man.wander();
                        this.wanderer_man.updatePos();
                    }
                    this.wanderingCiticen--;
                }
                else
                {
                    wanderer_man.seek(House_location);
                    if(UnityEngine.Vector2.Distance(wanderer_man.getPos(),this.House_location)<1)
                    {
                        home.killCitizen();
                        KMS();
                    }
                }
            }
            
            this.nearestCow = null;
            this.nearestWater = new UnityEngine.Vector2(-1,-1);
            this.wanderer_man.updateMan();
        }
        // Update is called once per frame
        void Update()
        {
            if (this.wanderer_man == null) return;
            
            Vector2 temp = this.wanderer_man.getPos();
            this.transform.position = new Vector3(temp[0], temp[1], 0);
            this.pos_x = temp[0];
            this.pos_y = temp[1];
            this.SpawnedMan.transform.position = new Vector3(temp[0], temp[1], 0);

            if(this.attackCooldown <= attackMaxCooldown)
            {
                this.attackCooldown++;
            }

            this.HB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1,0);
            this.thirstB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.2f,0);
            this.hungerB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.4f,0);
            this.inst_flag.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.6f,0);

            this.life -= this.lifeLoss;
            if(this.actualState == State.DrinkingWater)
            {
                this.thirst+=this.thirstLoss*3;
                if(this.thirst >= this.maxThirst) this.actualState = State.Wander;
            }
            else
            {
                this.thirst -= this.thirstLoss;
            }
            this.hunger -= this.hungerLoss;

            this.HB.updateVar(this.life,this.maxLife);
            this.thirstB.updateVar(this.thirst,this.maxThirst);
            this.hungerB.updateVar(this.hunger,this.maxHunger);

            //Death moment
            if(this.life <= 0 || this.thirst <= 0 || this.hunger <= 0)
            {
                this.KMS();
            }
        
            if(this.wanderer_man.isTired()) return;
            
            //Layering
            this.inst_Hat.GetComponent<SpriteRenderer> ().sortingOrder = 102-(int)this.pos_y;
            this.SpawnedMan.GetComponent<SpriteRenderer> ().sortingOrder = 102-(int)this.pos_y;

            if (this.wanderer_man.lookingRight())
            {
                this.SpawnedMan.transform.localScale = new UnityEngine.Vector2(-10,10);

                this.inst_Hat.transform.localScale = new UnityEngine.Vector2(-10,10);
                this.inst_Hat.transform.position = new UnityEngine.Vector3(this.pos_x+0.1f, this.pos_y+0.3f,0);
            }
            else
            {
                this.SpawnedMan.transform.localScale = new UnityEngine.Vector2(10,10);

                this.inst_Hat.transform.localScale = new UnityEngine.Vector2(10,10);
                this.inst_Hat.transform.position = new UnityEngine.Vector3(this.pos_x-0.1f, this.pos_y+0.3f,0);
            }
        }

        public UnityEngine.Vector2 getPos()
        {
            return this.wanderer_man.getPos();
        }

        public UnityEngine.Vector2 getVel()
        {
            return this.wanderer_man.getVel();
        }

        public Job getActualJob()
        {
            return this.actualJob;
        }

        public void followFood()
        {
            if(this.nearestFood.x != -1 && this.nearestFood.y != -1)
            {
                this.wanderer_man.seek(this.nearestFood);
                this.wanderer_man.updatePos();
                UnityEngine.Vector2 pos = this.wanderer_man.getPos();
                if(this.nearestCow != null){
                    this.nearestFood = this.nearestCow.getPos();
                    if (UnityEngine.Vector2.Distance(pos,this.nearestFood)<this.attackRadio && this.attackCooldown >= this.attackMaxCooldown)
                    {
                        this.attackCooldown = 0;
                        if (!this.nearestCow.Hit(this.damage,this.wanderer_man.getPos()))
                        {
                            this.actualState = State.Wander;
                            if(actualJob != Job.FoodGatherer)
                            {
                                this.hunger = maxHunger;
                            }
                            else
                            {
                                food2village += 200;
                                hasfood = true;
                            }
                            this.nearestCow = null;
                        }
                        this.nearestFood = new UnityEngine.Vector2(-1,-1);
                    }
                    return;
                }
                if (this.nearestree != null)
                {
                    if(!this.nearestree.HasFruit()) {
                        this.nearestree = null;
                        this.nearestFood = new UnityEngine.Vector2(-1,-1);
                    }else{
                        if (UnityEngine.Vector2.Distance(pos,this.nearestFood)<this.attackRadio && this.attackCooldown >= this.attackMaxCooldown)
                        {
                            this.attackCooldown = 0;
                            if(actualJob != Job.FoodGatherer)
                            {
                                this.hunger += this.nearestree.Harvest();
                            }
                            else
                            {
                                food2village += this.nearestree.Harvest();
                                hasfood = true;
                            }
                            this.actualState = State.Wander;
                            this.nearestree = null;
                            this.nearestFood = new UnityEngine.Vector2(-1,-1);
                        }
                    }
                    return;
                }
                if (this.nearestBush != null)
                {
                    if(!this.nearestBush.HasFruit()) {
                        this.nearestBush = null;
                        this.nearestFood = new UnityEngine.Vector2(-1,-1);
                    }else{
                        if (UnityEngine.Vector2.Distance(pos,this.nearestFood)<this.attackRadio && this.attackCooldown >= this.attackMaxCooldown)
                        {
                            this.attackCooldown = 0;
                            if(actualJob != Job.FoodGatherer)
                            {
                                this.hunger += this.nearestBush.Harvest();
                            }
                            else
                            {
                                food2village += this.nearestBush.Harvest();
                                hasfood = true;
                            }
                            this.actualState = State.Wander;
                            this.nearestBush = null;
                            this.nearestFood = new UnityEngine.Vector2(-1,-1);
                        }
                    }
                }
                return;
            }
        }

        //PhysX events
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(this.wanderer_man == null) return;
            if(other.gameObject.tag == "man" && other.gameObject.GetComponent<Wanderer_man>().getActualJob() == this.actualJob && this.actualJob == Job.nomad)
            {
                this.wanderer_man.addNearMan();
            }
        }
        private void OnTriggerStay2D(Collider2D other) {
            if(other.gameObject.tag == "man" && other.gameObject.GetComponent<Wanderer_man>().getActualJob() == this.actualJob && this.actualJob == Job.nomad)
            {
                this.nomadGroup++;
                Wanderer_man man = other.gameObject.GetComponent<Wanderer_man>();
                if (this.cAsentamiento)
                {
                    this.checkedNomads = true;
                    man.KMS();
                    return;
                }
                wanderer_man.cohereWithMen(man.getPos(),man.getVel());
            }

            if((other.gameObject.tag == "water" && this.actualState == State.SearchingWater) || (other.gameObject.tag == "water" && this.actualJob == Job.WaterGatherer))
            {
                GameObject water = other.gameObject;
                if(UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), water.transform.position)<=UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), this.nearestWater))
                {
                    this.nearestWater = water.transform.position;
                }
                return;
            }

            if((other.gameObject.tag == "Cow" && this.actualState == State.searchingFood) || (other.gameObject.tag == "Cow" && this.actualJob == Job.FoodGatherer))
            {
                if (this.nearestree != null)
                {
                    if(actualJob != Job.LogGatherer){
                        this.nearestree = null;
                    }
                    this.nearestFood = new UnityEngine.Vector2(-1,-1);
                }
                if (this.nearestBush != null)
                {
                    this.nearestBush = null;
                    this.nearestFood = new UnityEngine.Vector2(-1,-1);
                }

                if (!this.nearFood)
                {
                    this.nearFood = true;
                    this.nearestFood = new UnityEngine.Vector2(-1,-1);
                }
                Cow cow = other.gameObject.GetComponent<Cow>();
                if(UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), cow.getPos())<=UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), this.nearestFood))
                {
                    this.nearestFood = cow.getPos();
                    this.nearestCow = cow;
                }
                return;
            }

            if((other.gameObject.tag == "tree" && this.actualState == State.searchingFood) || (other.gameObject.tag == "tree" && this.actualJob == Job.LogGatherer) || (other.gameObject.tag == "tree" && this.actualJob == Job.FoodGatherer))
            {
                if (this.nearestCow != null && actualJob != Job.LogGatherer) return;
                
                if (!this.nearFood)
                {
                    this.nearFood = true;
                    this.nearestFood = new UnityEngine.Vector2(-1,-1);
                }
                tree closestTree = other.gameObject.GetComponent<tree>();
                Vector3 treePos = other.gameObject.transform.position;
                if(this.actualJob == Job.LogGatherer)
                {
                    if(this.nearestree == null || (UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), treePos)<=UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), this.nearestree.gameObject.transform.position)))
                    {
                        this.nearestree = closestTree;
                    }
                }
                
                if(UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), treePos)<=UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), this.nearestFood))
                {
                    if (closestTree.HasFruit())
                    {
                        if (this.nearestBush != null)
                        {
                            this.nearestBush = null;
                        }
                        this.nearestree = closestTree;
                        this.nearestFood = treePos;
                    }
                }
                return;
            }
            if((other.gameObject.tag == "bush" && this.actualState == State.searchingFood) || (other.gameObject.tag == "bush" && this.actualJob == Job.FoodGatherer))
            {
                if (this.nearestCow != null) return;

                if (!this.nearFood)
                {
                    this.nearFood = true;
                    this.nearestFood = new UnityEngine.Vector2(-1,-1);
                }
                bush closestBush = other.gameObject.GetComponent<bush>();
                Vector3 bushPos = other.gameObject.transform.position;
                if(UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), bushPos)<=UnityEngine.Vector2.Distance(this.wanderer_man.getPos(), this.nearestFood))
                {
                    if (closestBush.HasFruit())
                    {
                        if (this.nearestree != null)
                        {
                            if(actualJob != Job.LogGatherer){
                                this.nearestree = null;
                            }
                            this.nearestFood = new UnityEngine.Vector2(-1,-1);
                        }
                        this.nearestBush = closestBush;
                        this.nearestFood = bushPos;
                    }
                }
                return;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.gameObject.tag == "man" && other.gameObject.GetComponent<Wanderer_man>().getActualJob() == this.actualJob && this.actualJob == Job.nomad)
            {
                this.wanderer_man.popNearMan();
            }
        }

        private void KMS()
        {
            Destroy(this.HB);
            Destroy(this.thirstB);
            Destroy(this.hungerB);
            Destroy(this.SpawnedMan);
            Destroy(this.inst_flag);
            Destroy(this.inst_Hat);
            Destroy(this.gameObject);
        }
    }
}