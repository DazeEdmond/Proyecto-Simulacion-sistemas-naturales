using System.Data;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

class CowBehaviour
{
    //attributes
    private UnityEngine.Vector2 pos;
    private UnityEngine.Vector2 vel = UnityEngine.Vector2.zero;
    private UnityEngine.Vector2 acc = UnityEngine.Vector2.zero;

    private float wanderLookAhead = 30;
    private UnityEngine.Vector2 futurePos = UnityEngine.Vector2.zero;
    private float wanderRadius = 10;
    private float wanderAng = 0;
    private float wanderNoise = UnityEngine.Random.Range(0f,2f);
    private float maxSteeringForce = 0.17f;
    private float maxVel = 0.2f;
    private bool escaping = false;
    private float escapeRatio = 0.15f;
    private int dangerRadio = 10;
    private UnityEngine.Vector2 danger = new UnityEngine.Vector2(0,0);
    public int nearCows = 0;
    public float cowSeparationRadio = 2;
    public UnityEngine.Vector2 alignmentForce = new UnityEngine.Vector2(0,0);
    public UnityEngine.Vector2 cohereForce = new UnityEngine.Vector2(0,0);
    public UnityEngine.Vector2 separationForce = new UnityEngine.Vector2(0,0);

    public int maxCowStamina = UnityEngine.Random.Range(300,500);
    public int cowStamina = 0;
    public bool cooldown = true;
    private Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainVals = GlobalData.TerrainValues;


    //construct
    public CowBehaviour(UnityEngine.Vector2 pos, UnityEngine.Vector2 vel, UnityEngine.Vector2 acc, int cowQuan)
    {
        this.pos = pos;
        this.vel = vel;
        this.acc = acc;
        this.nearCows = 0;
        this.dangerRadio /= 2;
    }
    //math map function beacuse c# does not have it
    private float Map(float value, float low1, float max1, float low2, float max2)
    {
        return low2 + (max2 - low2) * (value - low1) / (max1 - low1);
    }

    public void updateCow()
    {
        this.wanderNoise += 0.001f;

        if (this.cooldown)
        {
            this.cowStamina++;
            if(this.cowStamina >= this.maxCowStamina)
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
            this.danger = new UnityEngine.Vector2(this.pos.x+this.vel.x,this.pos.y+this.vel.y);
            this.dangerRadio = 5;
            this.escaping = true;
            this.wanderAng += (float)System.Math.PI;
            this.acc *= 0;
            this.vel *= -1;
            return;
        }
        if (TerrainVals[(int)System.Math.Floor((this.pos + this.vel).y)][(int)System.Math.Floor((this.pos + this.vel).x)].height <= 0.25f)
        {
            this.danger = new UnityEngine.Vector2(this.pos.x+this.vel.x,this.pos.y+this.vel.y);
            this.dangerRadio = 5;
            this.escaping = true;
            this.wanderAng += (float)System.Math.PI;
            this.acc *= 0;
            this.vel *= -1;
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

        this.cowStamina--;
        if(this.cowStamina <= 0) this.cooldown = true;

    }

    public UnityEngine.Vector2 getPos()
    {
        return this.pos;
    }
    public UnityEngine.Vector2 getVel()
    {
        return this.vel;
    }
    

    private void addForce(UnityEngine.Vector2 force)
    {
        this.acc += force;
    }

    private void seek(UnityEngine.Vector2 target)
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

    public bool lookingRight()
    {
        return this.vel.x < 0;
    }

    private void escape(UnityEngine.Vector2 danger)
    {
        UnityEngine.Vector2 escape = danger-this.pos;
        escape.Normalize();
        escape*=-escapeRatio;

        this.addForce(escape);
    } 
    public void wander()
    {
        this.wanderAng += this.Map(Mathf.PerlinNoise1D(this.wanderNoise), 0f, 1f, -math.PI2, math.PI2);

        this.futurePos *= 0;
        this.futurePos += this.vel;

        this.futurePos.Normalize();

        this.futurePos *= this.wanderLookAhead;

        float x = math.cos(this.wanderAng)*this.wanderRadius + this.futurePos[0] + this.pos[0];
        float y = math.sin(this.wanderAng)*this.wanderRadius + this.futurePos[1] + this.pos[1];

        UnityEngine.Vector2 Target = new UnityEngine.Vector2(x, y);

        this.seek(Target);
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

}