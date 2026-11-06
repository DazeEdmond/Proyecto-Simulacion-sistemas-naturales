using System.Data;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

class CowBehaviour
{
    //attributes
    private UnityEngine.Vector2 pos;
    private UnityEngine.Vector2 vel = UnityEngine.Vector2.zero;
    private UnityEngine.Vector2 acc = UnityEngine.Vector2.zero;

    private float wanderLookAhead = 25;
    private UnityEngine.Vector2 futurePos = UnityEngine.Vector2.zero;
    private float wanderRadius = 10;
    private float wanderAng = 0;
    private float wanderNoise = UnityEngine.Random.Range(0f,2f);
    private float maxSteeringForce = 0.01f;
    private float maxVel = 0.01f;


    //construct
    public CowBehaviour(UnityEngine.Vector2 pos, UnityEngine.Vector2 vel, UnityEngine.Vector2 acc)
    {
        this.pos = pos;
        this.vel = vel;
        this.acc = acc;
    }
    //math map function beacuse c# does not have it
    private float Map(float value, float low1, float max1, float low2, float max2)
    {
        return low2 + (max2 - low2) * (value - low1) / (max1 - low1);
    }

    public void updateCow()
    {
        this.vel += this.acc;
        if(this.vel.magnitude >= this.maxVel)
        {
            this.vel.Normalize();
            this.vel *= this.maxVel;
        }
        this.pos += this.vel;

        this.acc *= 0;
    }

    public UnityEngine.Vector2 getPos()
    {
        return this.pos;
    }

    private void addForce(UnityEngine.Vector2 force)
    {
        this.acc += force;
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

    public void wander()
    {
        this.wanderAng = this.Map(Mathf.PerlinNoise1D(this.wanderNoise), 0f, 1f, -math.PI2*1.5f, math.PI2*1.5f);
        this.wanderNoise += 0.001f;

        this.futurePos *= 0;
        this.futurePos += this.vel;

        this.futurePos.Normalize();

        this.futurePos *= this.wanderLookAhead;

        float x = math.cos(this.wanderAng)*this.wanderRadius + this.futurePos[0] + this.pos[0];
        float y = math.sin(this.wanderAng)*this.wanderRadius + this.futurePos[1] + this.pos[1];

        UnityEngine.Vector2 Target = new UnityEngine.Vector2(x, y);

        this.seek(Target);
    }
}