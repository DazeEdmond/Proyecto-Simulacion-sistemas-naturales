using UnityEngine;

public class tree : MonoBehaviour
{
    // Se mantiene por compatibilidad con el prefab (ya no se destruye ni se escala)
    public GameObject Sprite;

    // sprites[0] = sin fruta, sprites[1] = con fruta
    public Sprite[] sprites;

    public int fruitInterval;
    public int nutritionValue = GlobalData.TreeNutrition; // árbol = 1
    public int woodAmount = 1;                            // madera que da al talarlo

    // El bosque se suscribe para saber cuándo muere un árbol
    public event System.Action<tree> Removed;

    private TreeBehaviour TreeB;
    private SpriteRenderer spriteRenderer;
    private bool removed = false;

    void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.spriteRenderer.sortingOrder = 102-(int)this.gameObject.transform.position.y;
    }

    public void init(GameObject Sprite)
    {
        this.Sprite = Sprite;
        this.TreeB = new TreeBehaviour(this.fruitInterval, this.nutritionValue);
        fruitInterval = (int) GlobalData.Get(Param.FruitTime);
        RefreshSprite();
    }

    private void FixedUpdate()
    {
        if (this.TreeB == null) return;
        this.TreeB.updateTree();
    }

    void Update()
    {
        if (this.TreeB == null) return;
        RefreshSprite();
    }

    private void RefreshSprite()
    {
        if (this.spriteRenderer == null || this.sprites == null || this.sprites.Length < 2) return;

        UnityEngine.Sprite wanted = this.TreeB.hasFruit ? this.sprites[1] : this.sprites[0];
        if (this.spriteRenderer.sprite != wanted)
        {
            this.spriteRenderer.sprite = wanted;
        }
        this.spriteRenderer.sortingOrder = 102-(int)this.gameObject.transform.position.y;
    }

    // ---------- API para los humanos ----------

    public bool HasFruit()
    {
        return this.TreeB != null && this.TreeB.hasFruit;
    }

    // Devuelve el valor nutricional (1) y reinicia el ciclo; 0 si no había fruta
    public int Harvest()
    {
        if (this.TreeB == null) return 0;
        int value = this.TreeB.TryHarvest();
        if (value > 0) RefreshSprite();
        return value;
    }

    // Tala el árbol: desaparece y devuelve la madera obtenida
    public int Chop()
    {
        if (this.removed) return 0;
        DestroySelf();
        return this.woodAmount;
    }

    public void DestroySelf()
    {
        if (this.removed) return;
        this.removed = true;
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        this.removed = true;
        Removed?.Invoke(this);
        Removed = null;
    }
}
