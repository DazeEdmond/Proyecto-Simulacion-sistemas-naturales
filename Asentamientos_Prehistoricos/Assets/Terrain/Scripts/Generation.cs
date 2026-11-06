using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generation : MonoBehaviour
{
    public Tilemap tilemap;// = new GameObject("Tilemap").AddComponent<Grid>();

    //Tamaño del Tile (tamaño inicial)
    public int width = 108;
    public int height = 60;

    //Scale es granularidad del perlin nice
    public int scale = 5;
    public int forest_limit = 10;
    public float forest_radius = 10;
    public int forest_generation_time = 100;
    //Tamaño del tile para mostrar (usado para simular la distancia de vista de la camara)
    //Es el parametro que rige la posicion de los tiles en generate
    private float tileSize = GlobalData.tileSize;

    //Semilla para generacion
    private int offsetX;
    private int offsetY;

    //Semilla para la humedad
    private int HumidityOffsetX;
    private int HumidityOffsetY;

    //lista de bosques
    private List<Forest> forests = new List<Forest> {};

    //Tilemaps(ubicados en Assets/Terrain/Tiles)
    public TileBase WaterUpTile;
    public TileBase WaterTile;
    public TileBase WaterDownTile;
    public TileBase GrassUpTile;
    public TileBase GrassTile;
    public TileBase GrassDownTile;
    public TileBase MountainUpTile;
    public TileBase MountainTile;
    public TileBase MountainDownTile;

    //cow tile
    public GameObject Cow;
    public GameObject CowPrefab;

    //plant tile
    public GameObject Bush;
    public GameObject Tree;

    //Discriminantes(Determinan cuanto cual tile usaran dependiendo del valor de perlin nice)
    public float mountainUpDiscriminator = 1f;
    public float mountainDiscriminator = 0.95f;
    public float mountainDownDiscriminator = 0.9f;
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
        foreach (Forest f in forests)
        {
            f.updateForest();
            if (f.is_Tree_Time())
            {
                GameObject new_tree = Instantiate(Tree, new UnityEngine.Vector2(0,0), UnityEngine.Quaternion.identity);
                new_tree.transform.localScale = new UnityEngine.Vector2(10,10);
                new_tree.GetComponent<SpriteRenderer> ().sortingOrder = 1;
                f.add_Tree(new_tree);
                f.reset_time();
            }
        }
    }

    private void seed()
    {
        //Genera la semilla
        offsetX = Random.Range(0,9999);
        offsetY = Random.Range(0,9999);

        HumidityOffsetX = Random.Range(0,9999);
        HumidityOffsetY = Random.Range(0,9999);
    }

    private void Generate()
    {
        //Genera el perlin nice
        for (int y=(int)transform.position.y;y<transform.position.y+height;y++)
        {
            for (int x=(int)transform.position.x;x<transform.position.x+width;x++)
            {
                //Coordenadas del tile
                float xCoord = (float) y/height*scale + offsetY;
                float yCoord = (float) x/width*scale + offsetX;

                float XHumid = (float) x/width*scale + HumidityOffsetX;
                float YHumid = (float) y/height*scale + HumidityOffsetY;

                //Noise
                float noise = Mathf.PerlinNoise(xCoord,yCoord);

                float HumidNoise = Mathf.PerlinNoise(XHumid,YHumid);

                //decidir Tile a usar
                TileBase tile2Place;

                GameObject plant2Place;
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
                }else if (noise < mountainDownDiscriminator)
                {
                    tile2Place = MountainDownTile;
                }else if (noise < mountainDiscriminator)
                {
                    tile2Place = MountainTile;
                }else
                {
                    tile2Place = MountainUpTile;                    
                }

                if(HumidNoise > 0.3 && HumidNoise < 0.6 && (
                    tile2Place == GrassTile || tile2Place == GrassUpTile
                    )
                )
                {
                    int plant = Random.Range(0,999);
                    
                    int cowGen = Random.Range(0,999);
                    int cowQuan = Random.Range(1,4);
                    
                    if(plant <= 50)
                    {
                        plant2Place = Bush;

                    }else if(plant <= 75)
                    {
                        plant2Place = Tree;
                    }else
                    {
                    plant2Place = null;
                    }

                    if(cowGen <= 0.5)
                    {
                        for(int i=0;i<cowQuan;i++){
                            GameObject cow = Instantiate(CowPrefab, new UnityEngine.Vector2(x,y), UnityEngine.Quaternion.identity);
                            Cow COW = cow.GetComponent<Cow>();
                            COW.init(Cow, new UnityEngine.Vector2(x,y), new UnityEngine.Vector2(0.01f,0));
                        }
                    }

                }else
                {
                    plant2Place = null;
                }

                //Instancia el tile en la posicion x,y
                
                if(plant2Place != null)
                {
                    if(plant2Place == Bush)
                    {
                        plant2Place.transform.localScale = new UnityEngine.Vector2(10,10);
                        Instantiate(plant2Place, new UnityEngine.Vector2(x,y), UnityEngine.Quaternion.identity);
                        plant2Place.GetComponent<SpriteRenderer> ().sortingOrder = 1;
                    }
                    else
                    {
                        plant2Place.transform.localScale = new UnityEngine.Vector2(10,10);
                        GameObject new_tree = Instantiate(plant2Place, new UnityEngine.Vector2(x,y), UnityEngine.Quaternion.identity);
                        plant2Place.GetComponent<SpriteRenderer> ().sortingOrder = 1;
                        Forest new_forest = new Forest(x,y, forest_radius, forest_limit, forest_generation_time);
                        new_forest.add_First_Tree(new_tree);
                        forests.Add(new_forest);
                    }
                    
                }

                tilemap.SetTile(new Vector3Int(x,y,1), tile2Place);
            }
        }
    }
}
