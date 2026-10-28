using System.Data;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

class Man
{
    //attributes
    private UnityEngine.Vector2 pos;
    private UnityEngine.Vector2 vel = UnityEngine.Vector2.zero;
    private UnityEngine.Vector2 accel = UnityEngine.Vector2.zero;

    private float wanderer_look_ahead = 25;
    private UnityEngine.Vector2 future_pos = UnityEngine.Vector2.zero;
    private float wanderer_radius = 10;
    private float wanderer_ang = 0;
    private float wanderer_Noise = 0.01F;

    //public GameObject man_sprite;

    //construct
    public Man(float posX, float posY, GameObject GameMan)
    {
        this.pos = new UnityEngine.Vector2(posX, posY);
    }
    //math map function beacuse c# does not have it
    private float Map(float value, float low1, float max1, float low2, float max2)
    {
        return low2 + (max2 - low2) * (value - low1) / (max1 - low1);
    }

    public void updateMan()
    {
        this.vel += this.accel;
        if(this.vel.magnitude >= 0.025f)
        {
            this.vel.Normalize();
            this.vel *= 0.025f;
        }
        this.pos += this.vel;

        this.accel *= 0;
    }

    public UnityEngine.Vector2 getPos()
    {
        return this.pos;
    }


    private void addForce(UnityEngine.Vector2 force)
    {
        this.accel += force;
    }

    public void seek(UnityEngine.Vector2 target)
    {
        UnityEngine.Vector2 Desired = target - this.pos;
        Desired -= this.vel;
        if(Desired.magnitude >= 0.025f)
        {
            Desired.Normalize();
            Desired *= 0.025f;
        }

        this.addForce(Desired);
    }

    public void wanderer()
    {
        this.wanderer_ang = this.Map(Mathf.PerlinNoise1D(this.wanderer_Noise), 0f, 1f, -math.PI2, math.PI2);
        this.wanderer_Noise += 0.001f;

        this.future_pos *= 0;
        this.future_pos += this.vel;

        this.future_pos.Normalize();

        this.future_pos *= this.wanderer_look_ahead;

        float x = math.cos(this.wanderer_ang)*this.wanderer_radius + this.future_pos[0] + this.pos[0];
        float y = math.sin(this.wanderer_ang)*this.wanderer_radius + this.future_pos[1] + this.pos[1];

        UnityEngine.Vector2 Target = new UnityEngine.Vector2(x, y);

        this.seek(Target);
    }
}