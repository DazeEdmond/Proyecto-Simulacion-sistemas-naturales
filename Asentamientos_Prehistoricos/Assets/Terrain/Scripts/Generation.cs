using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NPCs
{
    public class Generation : MonoBehaviour
    {
        public Tilemap tilemap;// = new GameObject("Tilemap").AddComponent<Grid>();

        //Tamaño del Tile (tamaño inicial)
        public int width = 108;
        public int height = 60;

        //Scale es granularidad del perlin nice
        public int scale = 5;
        public int forest_limit;
        public float forest_radius;
        public int forest_generation_time;
        //Tope global de árboles para que la simulación no se sature
        public int max_trees = 1000;

        private Dictionary<int,Dictionary<int, (float height,float humidity,float temperature)>> TerrainVals = GlobalData.TerrainValues;
        //Semilla para generacion
        private int offsetX;
        private int offsetY;

        //Semilla para la humedad
        private int HumidityOffsetX;
        private int HumidityOffsetY;

        //forest list
        private List<Forest> forests = new List<Forest> {};
        //Village list
        //private List<Village> villages = new List<Village>{};

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
        public GameObject waterCollider;

        //plant tile
        public GameObject Bush;
        public GameObject BushPrefab;
        public GameObject Tree;
        public GameObject TreePrefab;

        public GameObject ManPrefab;
        public bool isManCreated = false;

        //Discriminantes(Determinan cuanto cual tile usaran dependiendo del valor de perlin nice)
        public float mountainUpDiscriminator;
        public float mountainDiscriminator;
        public float mountainDownDiscriminator;
        public float grassUpDiscriminator;
        public float grassDiscriminator;
        public float grassDownDiscriminator;
        public float waterUpDiscriminator;
        public float waterDiscriminator;
        public float waterDownDiscriminator;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            forest_limit = (int) GlobalData.Get(Param.ForestLimit);
            forest_radius = GlobalData.Get(Param.ForestRadius);
            forest_generation_time = (int) GlobalData.Get(Param.ForestGeneration);
            TerrainVals.Clear();
            GlobalData.mapWidth = width;
            GlobalData.mapHeight = height;
            seed();

            waterDownDiscriminator    = GlobalData.Discriminator(Param.WaterDown);
            waterDiscriminator        = GlobalData.Discriminator(Param.Water);
            waterUpDiscriminator      = GlobalData.Discriminator(Param.WaterUp);
            grassDownDiscriminator    = GlobalData.Discriminator(Param.GrassDown);
            grassDiscriminator        = GlobalData.Discriminator(Param.Grass);
            grassUpDiscriminator      = GlobalData.Discriminator(Param.GrassUp);
            mountainDownDiscriminator = GlobalData.Discriminator(Param.MountainDown);
            mountainDiscriminator     = GlobalData.Discriminator(Param.Mountain);
            mountainUpDiscriminator   = GlobalData.Discriminator(Param.MountainUp);

            Generate();
        }

        // El crecimiento de bosques va en FixedUpdate para ir al mismo ritmo que la fruta
        void FixedUpdate()
        {
            // Quita bosques extintos (sin árboles no vuelven a crecer) y cuenta los árboles totales
            int totalTrees = 0;
            for (int i = forests.Count - 1; i >= 0; i--)
            {
                if (forests[i].IsExtinct)
                {
                    forests.RemoveAt(i);
                    continue;
                }
                totalTrees += forests[i].TreeCount;
            }

            foreach (Forest f in forests)
            {
                f.updateForest();
                if (!f.is_Tree_Time()) continue;
                if (totalTrees >= max_trees) continue;

                UnityEngine.Vector2 newPos;
                if (!f.TryGetNewTreePosition(out newPos)) continue;

                GameObject new_tree = Instantiate(TreePrefab, new UnityEngine.Vector2(newPos.x,newPos.y), UnityEngine.Quaternion.identity);
                tree otherTree = new_tree.GetComponent<tree>();
                otherTree.init(Tree);
                f.add_Tree(otherTree);
                f.reset_time();
                totalTrees++;
            }
        }

        private void seed()
        {
            if (GlobalData.globalSeed > 0) {
                int s = GlobalData.globalSeed;

                // Forma determinista de generar la Semilla
                offsetX = s % 9999;
                offsetY = (s * 2) % 9999; 

                HumidityOffsetX = (s + 200) % 9999;
                HumidityOffsetY = (s + 500) % 9999; 

                // Misma semilla = misma vegetación y animales iniciales
                Random.InitState(s);
            } else {
                //Genera la semilla de forma aleatoria xd
                offsetX = Random.Range(0,9999);
                offsetY = Random.Range(0,9999);

                HumidityOffsetX = Random.Range(0,9999);
                HumidityOffsetY = Random.Range(0,9999);
            }   
        }

        private void Generate()
        {
            //Genera el perlin nice
            for (int y=(int)transform.position.y;y<transform.position.y+height;y++)
            {
                TerrainVals.Add(y,new Dictionary<int,(float height,float humidity,float temperature)>());
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

                    float temp = GlobalData.Map((noise*-30) + (HumidNoise*30), -30, 30, 0, 40);

                    TerrainVals[y].Add(x,(noise,HumidNoise,temp));

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

                    if(tile2Place == WaterDownTile || tile2Place == WaterTile || tile2Place == WaterUpTile)
                    {
                        GameObject water = Instantiate(waterCollider, new UnityEngine.Vector2(x+0.5f,y+0.5f), UnityEngine.Quaternion.identity);
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
                                COW.init(Cow, new UnityEngine.Vector2(x,y), new UnityEngine.Vector2(0.1f,0),cowQuan-1);
                            }
                        }

                        if (!isManCreated)
                        {
                            int ManGen = Random.Range(0,999);
                            if(ManGen <= 3)
                            {
                                GameObject New_Man = Instantiate(ManPrefab, new UnityEngine.Vector2(x,y), UnityEngine.Quaternion.identity);
                                New_Man.GetComponent<Wanderer_man>().init(new UnityEngine.Vector2(x,y), 0, new UnityEngine.Vector2(-1,-1));
                                isManCreated = true;
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
                            GameObject new_bush = Instantiate(BushPrefab, new UnityEngine.Vector2(x,y), UnityEngine.Quaternion.identity);
                            bush otherBush = new_bush.GetComponent<bush>();
                            otherBush.init(Bush);
                        }
                        else
                        {
                            GameObject new_tree = Instantiate(TreePrefab, new UnityEngine.Vector2(x,y), UnityEngine.Quaternion.identity);
                            tree otherTree = new_tree.GetComponent<tree>();
                            otherTree.init(Tree);
                            Forest new_forest = new Forest(x,y, forest_radius, forest_limit, forest_generation_time);
                            new_forest.add_Tree(otherTree);
                            forests.Add(new_forest);
                        }
                        
                    }

                    tilemap.SetTile(new Vector3Int(x,y,1), tile2Place);
                }
            }
        }
    }
}
