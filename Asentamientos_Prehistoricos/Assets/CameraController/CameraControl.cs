using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using NPCs;

public class CameraControl : MonoBehaviour
{
    public Camera cam;

    public float zoomStep = 3f;

    public float minFov = 8f;
    public float maxFov = 55f;

    public float mapSize = 100f;
    public float followSpeed = 6f;
    public float pickRadius = 2f;

    [Header("Villa")]
    public float housePickPadding = 0.15f;  // Margen extra alrededor de cada casa para el click

    private Transform target;   // Transform de la Cámara - Tiene: pos, rot, localScale.
    private Vector2 lastMouse;

    void Awake()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (!GlobalData.mapReady) return;

        // Mouse //

        Mouse m = Mouse.current; if (m == null) return;
        Vector2 mouse = m.position.ReadValue();

        // Zoom //

        float scroll = m.scroll.ReadValue().y;
        if (scroll != 0f)
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - Mathf.Sign(scroll) * zoomStep, minFov, maxFov);

        // Pan //

        bool dragging = m.rightButton.isPressed || m.middleButton.isPressed;
        bool justPressed = m.rightButton.wasPressedThisFrame || m.middleButton.wasPressedThisFrame;
        if (dragging)
        {
            target = null;
            if (!justPressed)
            {
                // Tamaño del Píxel
                float perPixel = 2f * Mathf.Abs(transform.position.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) / Screen.height;
                // Vector Dezplazamiento - Pan
                Vector2 d = (mouse - lastMouse) * perPixel;
                transform.position -= new Vector3(d.x, d.y, 0f);
            }
        }
        lastMouse = mouse;

        // Seguir Agente o Villa //

        // Un click sobre la UI (panel, botón) no cuenta como click sobre el mapa
        if (m.leftButton.wasPressedThisFrame && !PointerOverUI())
        {
            Vector2 world = ScreenToWorld(mouse);

            // Prioridad: casa > agente. Los ciudadanos caminan alrededor de las casas,
            // si el agente ganara siempre sería casi imposible clicar la casa.
            village hitVillage = PickVillage(world);
            if (hitVillage != null)
            {
                target = hitVillage.transform;
                SelectVillage(hitVillage);
            }
            else
            {
                target = PickNearest(world);
                SelectVillage(null);
            }
        }
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            target = null;
            SelectVillage(null);
        }

        if (target != null)
        {
            Vector3 goal = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, goal, followSpeed * Time.deltaTime); // Interpolación - Movimiento Suave - Ecuación Paramétrica - P = A + t(B - A)
        }

        // No salirse del mapa //

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, 0f, mapSize);
        p.y = Mathf.Clamp(p.y, 0f, mapSize);
        transform.position = p;
    }

    // Villa - Solo publica la selección; ReadInput muestra / oculta Canvas_Statistics
    void SelectVillage(village v)
    {
        GlobalData.selectedVillage = v;
    }

    bool PointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    // Coordenadas Universales
    Vector2 ScreenToWorld(Vector2 screen)
    {
        Vector3 w = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(transform.position.z)));
        return new Vector2(w.x, w.y);
    }

    // Villa más cercana cuyo click cae sobre (o casi sobre) alguna de sus casas
    village PickVillage(Vector2 world)
    {
        village best = null;
        float bestD = housePickPadding;

        foreach (village v in FindObjectsByType<village>(FindObjectsSortMode.None))
        {
            float d = v.DistanceToHouses(world);
            if (d <= bestD) { bestD = d; best = v; }
        }
        return best;
    }

    Transform PickNearest(Vector2 w)
    {
        Transform best = null;
        float bestD = pickRadius;

        foreach (Cow c in FindObjectsByType<Cow>(FindObjectsSortMode.None))
        {
            float d = Vector2.Distance(w, c.transform.position);
            if (d < bestD) { bestD = d; best = c.transform; }
        }
        foreach (Wanderer_man man in FindObjectsByType<Wanderer_man>(FindObjectsSortMode.None))
        {
            float d = Vector2.Distance(w, man.transform.position);
            if (d < bestD) { bestD = d; best = man.transform; }
        }
        return best;
    }
}
