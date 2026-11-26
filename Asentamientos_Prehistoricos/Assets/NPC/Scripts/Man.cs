using System.Collections.Generic;
using System.Data;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace NPCs
{
    class Man:NPC
    {
        //Man stats
        private UnityEngine.Vector2 alignmentForce = UnityEngine.Vector2.zero;
        private UnityEngine.Vector2 separationForce = UnityEngine.Vector2.zero;
        private UnityEngine.Vector2 cohereForce = UnityEngine.Vector2.zero;
        public int nearMen = 0;
        public float separationRadio = 2;
        public int cohereMen = 0;
        public float cohereRadio = 4;
        //construct
        public Man(float posX, float posY, float maxVel)
        {
            this.wanderLookAhead = 40;
            this.wanderRadius = 10;
            this.maxSteeringForce = maxVel/2;
            this.escapeRatio = 0.1f;
            this.dangerRadio = 10;
            this.maxStamina = UnityEngine.Random.Range((int)(600-GlobalData.TimeF()*250),(int)(750-GlobalData.TimeF()*250));
            this.stamina = 0;

            this.pos = new UnityEngine.Vector2(posX, posY);
            this.maxVel = maxVel;
        }
        public void updateMan()
        {
            this.wanderNoise += 0.001f;

            if (this.cooldown)
            {
                this.stamina++;
                if(this.stamina >= this.maxStamina)
                {
                    this.cooldown = false;
                }
                return;
            }

            if (UnityEngine.Vector2.Distance(this.pos,this.danger) <= this.dangerRadio)
            {
                if (!this.escaping)
                {
                    this.dangerRadio = 5;
                    this.escaping = true;
                }
                this.escape(this.danger);
            }else{
                this.flocking();
            }

            this.vel += this.acc;
            if(this.vel.magnitude >= this.maxVel)
            {
                this.vel.Normalize();
                this.vel *= this.maxVel;
            }

            if((this.pos + this.vel).x<0 || (this.pos + this.vel).y<0 || (this.pos + this.vel).x>100 || (this.pos + this.vel).y > 100)
            {
                this.setDanger(new UnityEngine.Vector2(this.pos.x+this.vel.x,this.pos.y+this.vel.y));
                return;
            }

            if (this.onWater())
            {
                this.vel *= 0.8f;
            }
            if (this.onMountain())
            {
                this.vel *= 0.8f;
            }

            this.pos += this.vel;

            this.acc *= 0;

            if (UnityEngine.Vector2.Distance(this.pos,this.danger) > this.dangerRadio && this.escaping)
            {
                this.dangerRadio /= 2;
                this.wanderNoise = UnityEngine.Random.Range(0f,20f);
                this.escaping = false;
                wanderAng = 0;
                wanderNoise = UnityEngine.Random.Range(0f,2f);
            }

            this.stamina--;
            if(this.stamina <= 0) this.cooldown = true;
        }

        public void updatePos()
        {
            if (this.cooldown)
            {
                this.stamina++;
                if(this.stamina >= this.maxStamina)
                {
                    this.cooldown = false;
                }
                return;
            }
            
            this.vel += this.acc;
            if(this.vel.magnitude >= this.maxVel)
            {
                this.vel.Normalize();
                this.vel *= this.maxVel;
            }
            if (this.onWater())
            {
                this.vel *= 0.8f;
            }
            if (this.onMountain())
            {
                this.vel *= 0.8f;
            }

            if((this.pos + this.vel).x<0 || (this.pos + this.vel).y<0 || (this.pos + this.vel).x>100 || (this.pos + this.vel).y > 100)
            {
                this.setDanger(new UnityEngine.Vector2(this.pos.x+this.vel.x,this.pos.y+this.vel.y));
                return;
            }

            this.pos += this.vel;

            this.acc *= 0;
        }

        public void addNearMan()
        {
            this.nearMen++;
        }
        public void cohereWithMen(UnityEngine.Vector2 direction,UnityEngine.Vector2 vel)
        {
            if(vel.x == 0 && vel.y == 0) {
                return;
            }
            this.alignmentForce+=vel;
            if(UnityEngine.Vector2.Distance(this.pos,direction)>this.cohereRadio){
                this.cohereMen++;
                this.cohereForce+=direction;
            }

            if(UnityEngine.Vector2.Distance(this.pos,direction)<this.separationRadio){
                UnityEngine.Vector2 diff = this.pos - direction;
                if (diff.x == 0 || diff.y == 0)
                {
                    diff = new UnityEngine.Vector2(UnityEngine.Random.Range(-5f,5f),UnityEngine.Random.Range(-5f,5f));
                }
                diff.Normalize();
                diff /= UnityEngine.Vector2.Distance(this.pos,direction)==0?1:UnityEngine.Vector2.Distance(this.pos,direction);
                this.separationForce += diff;
            }
        }
        public override UnityEngine.Vector2 getVel()
        {
            if (this.onWater())
            {
                return this.vel * 2;
            }
            if (this.onMountain())
            {
                return this.vel * 2;
            }
            return this.vel;
        }

        public void popNearMan()
        {
            this.nearMen--;
        }

        private void separate()
        {
            this.separationForce /= this.nearMen;
            // this.separationForce.Normalize();
            this.separationForce *= this.maxSteeringForce*1.1f;
            this.addForce(this.separationForce);
        }

        private void cohere()
        {
            if(this.cohereMen<=0){
                return;
            }
            this.cohereForce /= this.cohereMen;
            this.cohereForce -= this.pos;
            this.cohereForce.Normalize();
            this.cohereForce *= this.maxSteeringForce;
            this.addForce(this.cohereForce);
            this.cohereMen = 0;
        }

        private void align()
        {
            this.alignmentForce /= this.nearMen;
            // this.alignmentForce.Normalize();
            this.alignmentForce *= this.maxSteeringForce*1.5f;
            // this.alignmentForce -= this.vel;
            this.addForce(this.alignmentForce);
        }

        public void flocking()
        {
            if(this.nearMen > 0){
                this.align();
                this.separate();
                this.cohere();
                if(this.acc.x == 0 && this.acc.y == 0)
                {
                    this.wander();
                }
            }
            else
            {
                this.wander();
            }
            this.alignmentForce *= 0;
            this.cohereForce *= 0;
            this.separationForce *= 0;
        }
        public bool onWater()
        {
            return this.TerrainVals[(int)this.pos.y][(int)this.pos.x].height <= GlobalData.Discriminator(Param.WaterUp);
        }
        public bool onMountain()
        {
            return this.TerrainVals[(int)this.pos.y][(int)this.pos.x].height >= GlobalData.Discriminator(Param.MountainDown);
        }

        public List<((float, float, float), UnityEngine.Vector2)> surroundingTerrain()
        {
            List<((float, float, float), UnityEngine.Vector2)> returningValues = new List<((float, float, float), UnityEngine.Vector2)>{};
            UnityEngine.Vector2 temp = this.getPos();
            int temp_x = (int)temp[0];
            int temp_y = (int)temp[1];

            if((temp_x-1) > 0 && (temp_y-1) > 0)
            {
                if((temp_x+1) < 99 && (temp_y+1) < 99)
                {
                    (float, float, float) terraintemp1 = this.TerrainVals[temp_y-1][temp_x-1];
                    UnityEngine.Vector2 posTemp1 = new UnityEngine.Vector2(temp_x-1,temp_y-1);
                    returningValues.Add((terraintemp1, posTemp1));

                    (float, float, float) terraintemp2 = this.TerrainVals[temp_y-1][temp_x];
                    UnityEngine.Vector2 posTemp2 = new UnityEngine.Vector2(temp_x,temp_y-1);
                    returningValues.Add((terraintemp2, posTemp2));

                    (float, float, float) terraintemp3 = this.TerrainVals[temp_y-1][temp_x+1];
                    UnityEngine.Vector2 posTemp3 = new UnityEngine.Vector2(temp_x+1,temp_y-1);
                    returningValues.Add((terraintemp3, posTemp3));

                    (float, float, float) terraintemp4 = this.TerrainVals[temp_y][temp_x-1];
                    UnityEngine.Vector2 posTemp4 = new UnityEngine.Vector2(temp_x-1,temp_y);
                    returningValues.Add((terraintemp4, posTemp4));

                    (float, float, float) terraintemp5 = this.TerrainVals[temp_y][temp_x+1];
                    UnityEngine.Vector2 posTemp5 = new UnityEngine.Vector2(temp_x+1,temp_y);
                    returningValues.Add((terraintemp5, posTemp5));

                    (float, float, float) terraintemp6 = this.TerrainVals[temp_y+1][temp_x-1];
                    UnityEngine.Vector2 posTemp6 = new UnityEngine.Vector2(temp_x-1,temp_y+1);
                    returningValues.Add((terraintemp6, posTemp6));

                    (float, float, float) terraintemp7 = this.TerrainVals[temp_y+1][temp_x];
                    UnityEngine.Vector2 posTemp7 = new UnityEngine.Vector2(temp_x,temp_y+1);
                    returningValues.Add((terraintemp7, posTemp7));

                    (float, float, float) terraintemp8 = this.TerrainVals[temp_y+1][temp_x+1];
                    UnityEngine.Vector2 posTemp8 = new UnityEngine.Vector2(temp_x+1,temp_y+1);
                    returningValues.Add((terraintemp8, posTemp8));
                }
            }
            else
            {
                returningValues.Add(((-1,-1,-1), new UnityEngine.Vector2(-1,-1)));
            }

            return returningValues;
        }
    }
}