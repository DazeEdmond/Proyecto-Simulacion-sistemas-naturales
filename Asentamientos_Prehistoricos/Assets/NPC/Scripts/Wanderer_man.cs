using UnityEngine;

namespace NPCs
{
    public class Wanderer_man : MonoBehaviour
    {
        //Atributes
        public GameObject Man_Sprite;
        public Bar HB;
        public Bar thirstB;
        public Bar hungerB;
        public float pos_x = 54F;
        public float pos_y = 30F;
        //Life Atributes
        private enum State
        {
            Wander,
            SearchingWater,
            DrinkingWater,
            searchingFood,
        }
        [SerializeField] private State actualState = State.Wander;
        public float maxVel = 0.18F;
        public float maxLife = 10000;
        public float life = 10000;
        public float lifeLoss = 0.1F;
        public float maxThirst = 1000;
        public float thirst = 1000;
        public float thirstLoss = 0.1F;
        public UnityEngine.Vector2 nearestWater = new UnityEngine.Vector2(-1,-1);
        public float maxHunger = 2000;
        public float hunger = 2000;
        public float hungerLoss = 0.1F;
        public UnityEngine.Vector2 nearestFood = new UnityEngine.Vector2(-1,-1);
        public Cow nearestCow = null;
        public bool nearFood = false;
        public int damage = 20;
        public float attackRadio = 1;
        public float attackMaxCooldown = 100;
        public float attackCooldown = 0;

        private Man wanderer_man;

        private GameObject SpawnedMan;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            this.Man_Sprite.transform.localScale = new UnityEngine.Vector2(10,10);
            this.SpawnedMan = Instantiate(this.Man_Sprite, new UnityEngine.Vector2(this.pos_x, this.pos_y), UnityEngine.Quaternion.identity);

            this.SpawnedMan.GetComponent<SpriteRenderer> ().sortingOrder = 2;
            
            this.wanderer_man = new Man(this.pos_x, this.pos_y, this.maxVel);
            this.HB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1,0);
            this.thirstB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.2f,0);
            this.hungerB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.4f,0);
        }

        private void FixedUpdate() {
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

            if(this.actualState == State.DrinkingWater)
            {
                return;
            }

            if(this.actualState == State.searchingFood)
            {
                if(this.nearestFood.x != -1 && this.nearestFood.y != -1)
                {
                    this.wanderer_man.seek(this.nearestFood);
                    this.wanderer_man.updatePos();
                    UnityEngine.Vector2 pos = this.wanderer_man.getPos();
                    if (UnityEngine.Vector2.Distance(pos,this.nearestFood)<this.attackRadio && this.attackCooldown >= this.attackMaxCooldown)
                    {
                        this.attackCooldown = 0;
                        if (!this.nearestCow.Hit(this.damage,this.wanderer_man.getPos()))
                        {
                            this.actualState = State.Wander;
                            this.hunger = maxHunger;
                            this.nearestCow = null;
                        }
                        this.nearestFood = new UnityEngine.Vector2(-1,-1);
                    }
                    return;
                }
            }

            if(this.actualState == State.DrinkingWater)
            {
                return;
            }

            this.wanderer_man.updateMan();
            this.nearestCow = null;
            this.nearestWater = new UnityEngine.Vector2(-1,-1);
        }
        // Update is called once per frame
        void Update()
        {
            if(this.attackCooldown <= attackMaxCooldown)
            {
                this.attackCooldown++;
            }
            if (this.wanderer_man.lookingRight())
            {
                this.SpawnedMan.transform.localScale = new UnityEngine.Vector2(-10,10);
            }
            else
            {
                this.SpawnedMan.transform.localScale = new UnityEngine.Vector2(10,10);
            }

            Vector2 temp = this.wanderer_man.getPos();
            this.transform.position = new Vector3(temp[0], temp[1], 0);
            this.pos_x = temp[0];
            this.pos_y = temp[1];
            this.SpawnedMan.transform.position = new Vector3(temp[0], temp[1], 0);
            this.HB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1,0);
            this.thirstB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.2f,0);
            this.hungerB.transform.position = new UnityEngine.Vector3(this.pos_x, this.pos_y+1.4f,0);

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
                Destroy(this.SpawnedMan);
                Destroy(this.gameObject);
            }
        }

        //PhysX events
        private void OnTriggerStay2D(Collider2D other) {
            if(other.gameObject.tag == "water")
            {
                GameObject water = other.gameObject;
                if(UnityEngine.Vector3.Distance(new UnityEngine.Vector3(this.pos_x, this.pos_y, 0), water.transform.position)<=UnityEngine.Vector2.Distance(new UnityEngine.Vector2(this.pos_x, this.pos_y), this.nearestWater))
                {
                    this.nearestWater = water.transform.position;
                }
            }

            if(other.gameObject.tag == "Cow")
            {
                if (!this.nearFood)
                {
                    this.nearFood = true;
                    this.nearestFood = new UnityEngine.Vector2(-1,-1);
                }
                Cow cow = other.gameObject.GetComponent<Cow>();
                if(UnityEngine.Vector3.Distance(new UnityEngine.Vector3(this.pos_x, this.pos_y, 0), cow.getPos())<=UnityEngine.Vector2.Distance(new UnityEngine.Vector2(this.pos_x, this.pos_y), this.nearestFood))
                {
                    this.nearestFood = cow.getPos();
                    this.nearestCow = cow;
                }
            }
        }
    }
}