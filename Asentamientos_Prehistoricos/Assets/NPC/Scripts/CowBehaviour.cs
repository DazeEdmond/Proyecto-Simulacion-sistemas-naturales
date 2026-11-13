using System.Data;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace NPCs
{
    class CowBehaviour:NPC
    {
        //attributes
        public int life = 100;
        public int nearCows = 0;
        public float cowSeparationRadio = 2;
        public UnityEngine.Vector2 alignmentForce = new UnityEngine.Vector2(0,0);
        public UnityEngine.Vector2 cohereForce = new UnityEngine.Vector2(0,0);
        public UnityEngine.Vector2 separationForce = new UnityEngine.Vector2(0,0);

        //construct
        public CowBehaviour(UnityEngine.Vector2 pos, UnityEngine.Vector2 vel, UnityEngine.Vector2 acc, int cowQuan)
        {
            this.wanderLookAhead = 30;
            this.wanderRadius = 10;
            this.maxSteeringForce = 0.1f;
            this.maxVel = 0.15f;
            this.escapeRatio = 0.2f;
            this.dangerRadio = 10;
            this.maxStamina = UnityEngine.Random.Range(300,500);
            this.stamina = 0;
            
            this.pos = pos;
            this.vel = vel;
            this.acc = acc;
            this.nearCows = 0;
            this.dangerRadio /= 2;
        }

        public void updateCow()
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
            if (TerrainVals[(int)System.Math.Floor((this.pos + this.vel).y)][(int)System.Math.Floor((this.pos + this.vel).x)].height <= 0.25f)
            {
                this.setDanger(new UnityEngine.Vector2(this.pos.x+this.vel.x,this.pos.y+this.vel.y));
                return;
            }
            this.pos += this.vel;

            if (UnityEngine.Vector2.Distance(this.pos,this.danger) > this.dangerRadio && this.escaping)
            {
                this.dangerRadio /= 2;
                this.wanderNoise = UnityEngine.Random.Range(0f,20f);
                this.escaping = false;
                wanderAng = 0;
                wanderNoise = UnityEngine.Random.Range(0f,2f);
            }
            this.acc *= 0;

            this.stamina--;
            if(this.stamina <= 0) this.cooldown = true;

        }

        public void addNearCow()
        {
            this.nearCows++;
        }
        public void cohereWithCow(UnityEngine.Vector2 direction,UnityEngine.Vector2 vel)
        {
            this.cohereForce+=direction;
            this.alignmentForce+=vel;

            if(UnityEngine.Vector2.Distance(this.pos,direction)<this.cowSeparationRadio){
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

        public void popNearCow()
        {
            this.nearCows--;
        }

        private void separate()
        {
            this.separationForce /= this.nearCows;
            this.separationForce.Normalize();
            this.separationForce *= 0.1f;
            this.addForce(this.separationForce);
        }

        private void cohere()
        {
            this.cohereForce /= this.nearCows;
            this.cohereForce -= this.pos;
            this.cohereForce.Normalize();
            this.cohereForce *= 0.1f;
            this.addForce(this.cohereForce);
        }

        private void align()
        {
            this.alignmentForce /= this.nearCows;
            this.alignmentForce.Normalize();
            this.alignmentForce *= this.maxSteeringForce;
            this.alignmentForce -= this.vel;
            this.addForce(this.alignmentForce);
        }

        public void flocking()
        {
            if(this.nearCows > 0){
                this.align();
                this.separate();
                this.cohere();
            }
            else
            {
                this.wander();
            }
            this.alignmentForce *= 0;
            this.cohereForce *= 0;
            this.separationForce *= 0;
        }
        public void staminaShot()
        {
            this.stamina = this.maxStamina;
        }
        public bool isDeath()
        {
            return this.life<=0;
        }
    }
}