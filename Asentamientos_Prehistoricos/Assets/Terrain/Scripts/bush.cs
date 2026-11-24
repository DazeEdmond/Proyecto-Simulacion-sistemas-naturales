using UnityEngine;

public class bush : MonoBehaviour
{
    // Se mantiene por compatibilidad con el prefab (ya no se escala)
    public GameObject Sprite;

    // sprites[0] = sin fruta, sprites[1] = con fruta
    public Sprite[] sprites;

    public int fruitInterval;                        // da fruta con más frecuencia que el árbol
    public int nutritionValue = GlobalData.BushNutrition;  // arbusto = 2

    private BushBehaviour BushC;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.spriteRenderer.sortingOrder = 102-(int)this.gameObject.transform.position.y;
    }

    public void init(GameObject Sprite)
    {
        this.Sprite = Sprite;
        this.BushC = new BushBehaviour(this.fruitInterval, this.nutritionValue);
        fruitInterval = (int) GlobalData.Get(Param.FruitTime)/2;
        RefreshSprite();
    }

    private void FixedUpdate()
    {
        if (this.BushC == null) return;
        this.BushC.updateBush();
    }

    void Update()
    {
        if (this.BushC == null) return;
        RefreshSprite();
    }

    private void RefreshSprite()
    {
        if (this.spriteRenderer == null || this.sprites == null || this.sprites.Length < 2) return;

        UnityEngine.Sprite wanted = this.BushC.hasFruit ? this.sprites[1] : this.sprites[0];
        if (this.spriteRenderer.sprite != wanted)
        {
            this.spriteRenderer.sprite = wanted;
        }
        this.spriteRenderer.sortingOrder = 102-(int)this.gameObject.transform.position.y;
    }

    // ---------- API para los humanos ----------

    public bool HasFruit()
    {
        return this.BushC != null && this.BushC.hasFruit;
    }

    // Devuelve el valor nutricional (2) y reinicia el ciclo; 0 si no había fruta
    public int Harvest()
    {
        if (this.BushC == null) return 0;
        int value = this.BushC.TryHarvest();
        if (value > 0) RefreshSprite();
        return value;
    }
}
