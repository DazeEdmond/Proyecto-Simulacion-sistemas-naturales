using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Generation : MonoBehaviour
{
    //Tamaño del Tile (tamaño inicial)
    public int width = 108;
    public int height = 60;

    //Scale es granularidad del perlin nice
    public int scale = 5;
    //Tamaño del tile para mostrar (usado para simular la distancia de vista de la camara)
    //Es el parametro que rige la posicion de los tiles en generate
    private float tileSize = GlobalData.tileSize;

    //Semilla para generacion
    private int offsetX;
    private int offsetY;

    //Tilemaps(ubicados en Assets/Terrain/Tiles)
    public GameObject WaterUpTile;
    public GameObject WaterTile;
    public GameObject WaterDownTile;
    public GameObject GrassUpTile;
    public GameObject GrassTile;
    public GameObject GrassDownTile;
    public GameObject MountainTile;

    //Discriminantes(Determinan cuanto cual tile usaran dependiendo del valor de perlin nice)
    public float mountainDiscriminator = 1f;
    public float grassUpDiscriminator = 0.8f;
    public float grassDiscriminator = 0.6f;
    public float grassDownDiscriminator = 0.5f;
    public float waterUpDiscriminator = 0.25f;
    public float waterDiscriminator = 0.2f;
    public float waterDownDiscriminator = 0.1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Generar el mapa al iniciar el script
        seed();
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void seed()
    {
        //Genera la semilla
        offsetX = Random.Range(0,9999);
        offsetY = Random.Range(0,9999);
    }

    private void Generate()
    {
        //Genera el perlin nice
        for (float y=transform.position.y;y<height;y+=tileSize)
        {
            for (float x=transform.position.x;x<width;x+=tileSize)
            {
                //Coordenadas del tile
                float xCoord = (float) y/height*scale + offsetY;
                float yCoord = (float) x/width*scale + offsetX;

                //Noise
                float noise = Mathf.PerlinNoise(xCoord,yCoord);

                //decidir Tile a usar
                GameObject tile2Place;
                if (noise < waterDownDiscriminator)
                {
                    tile2Place = WaterDownTile;
                }else if (noise < waterDiscriminator)
                {
                    tile2Place = WaterTile;
                }else if (noise < waterUpDiscriminator)
                {
                    tile2Place = WaterUpTile;
                }else if (noise < grassDownDiscriminator)
                {
                    tile2Place = GrassDownTile;
                }else if (noise < grassDiscriminator)
                {
                    tile2Place = GrassTile;
                }else if (noise < grassUpDiscriminator)
                {
                    tile2Place = GrassUpTile;
                }else
                {
                    tile2Place = MountainTile;                    
                }
                //Instancia el tile en la posicion x,y
                tile2Place.transform.localScale = new UnityEngine.Vector2(tileSize, tileSize);
                Instantiate(tile2Place,new UnityEngine.Vector2(x,y),UnityEngine.Quaternion.identity);
            }
        }
    }
}
