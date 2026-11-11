using System.Data;
using System.Numerics;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace NPCs
{
    public class NPC
    {
        protected Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainVals = GlobalData.TerrainValues;
        protected UnityEngine.Vector2 pos;
        protected UnityEngine.Vector2 vel = UnityEngine.Vector2.zero;
        protected float maxVel;
        protected UnityEngine.Vector2 acc = UnityEngine.Vector2.zero;
        public int maxStamina;
        public int stamina = 0;
        protected float wanderLookAhead;
        protected UnityEngine.Vector2 futurePos = UnityEngine.Vector2.zero;
        protected float wanderRadius;
        protected float wanderAng = 0;
        protected float wanderNoise = UnityEngine.Random.Range(0f,2f);
        protected float maxSteeringForce;
        protected bool escaping = false;
        protected bool cooldown = true;
        protected float escapeRatio;
        protected int dangerRadio;
        protected UnityEngine.Vector2 danger = new UnityEngine.Vector2(0,0);

        public UnityEngine.Vector2 getPos()
        {
            return this.pos;
        }
        public UnityEngine.Vector2 getVel()
        {
            return this.vel;
        }
        protected void addForce(UnityEngine.Vector2 force)
        {
            this.acc += force;
        }
        public bool lookingRight()
        {
            return this.vel.x < 0;
        }

        protected void escape(UnityEngine.Vector2 danger)
        {
            UnityEngine.Vector2 escape = danger-this.pos;
            escape.Normalize();
            escape*=-escapeRatio;

            this.addForce(escape);
        } 
        public void setDanger(UnityEngine.Vector3 danger)
        {
            this.danger = danger;
            this.dangerRadio = 5;
            this.escaping = true;
            this.wanderAng += (float)System.Math.PI;
            this.acc *= 0;
            this.vel *= -1;
        }
        public void wander()
        {
            this.wanderAng += GlobalData.Map(Mathf.PerlinNoise1D(this.wanderNoise), 0f, 1f, -math.PI2, math.PI2);

            this.futurePos *= 0;
            this.futurePos += this.vel;

            this.futurePos.Normalize();

            this.futurePos *= this.wanderLookAhead;

            float x = math.cos(this.wanderAng)*this.wanderRadius + this.futurePos[0] + this.pos[0];
            float y = math.sin(this.wanderAng)*this.wanderRadius + this.futurePos[1] + this.pos[1];

            UnityEngine.Vector2 Target = new UnityEngine.Vector2(x, y);

            this.seek(Target);
        }
        public void seek(UnityEngine.Vector2 target)
        {
            UnityEngine.Vector2 Desired = target - this.pos;
            Desired -= this.vel;
            if(Desired.magnitude >= this.maxSteeringForce)
            {
                Desired.Normalize();
                Desired *= this.maxSteeringForce;
            }

            this.addForce(Desired);
        }
    }   
}
