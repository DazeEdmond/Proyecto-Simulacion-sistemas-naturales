using UnityEngine;
using NPCs;

// Coloca un borde a cada lado del mapa, en coordenadas del MUNDO.
// Se mueven y hacen zoom junto con el mapa (no quedan fijos a la pantalla).
public class MapBorders : MonoBehaviour
{
    public Generation generation;   // El objeto que tiene el script Generation (da width, height y origen)
    public Sprite leftSprite;       // IMG/Bordes/1.png
    public Sprite rightSprite;      // IMG/Bordes/2.png
    public float gap = 0f;          // Separación entre el mapa y el borde (unidades de mundo)

    void Start()
    {
        float x0 = generation.transform.position.x;
        float y0 = generation.transform.position.y;
        float w  = generation.width;
        float h  = generation.height;

        // El borde mide lo mismo que el mapa de alto; el ancho sale de la proporción del sprite
        float spriteH = leftSprite.bounds.size.y;
        float scale   = h / spriteH;

        CreateBorder("Border_Left", leftSprite, scale,
            x0 - gap - leftSprite.bounds.size.x * scale * 0.5f, y0 + h * 0.5f);

        CreateBorder("Border_Right", rightSprite, scale,
            x0 + w + gap + rightSprite.bounds.size.x * scale * 0.5f, y0 + h * 0.5f);
    }

    void CreateBorder(string name, Sprite sprite, float scale, float x, float y)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.transform.position = new Vector3(x, y, 0f);
        go.transform.localScale = new Vector3(scale, scale, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
    }
}
