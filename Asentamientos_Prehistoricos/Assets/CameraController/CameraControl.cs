using UnityEngine;
using UnityEngine.InputSystem;
using NPCs;

public class CameraControl : MonoBehaviour
{
    public Camera cam;
    public float zoomStep = 3f;      // grados de FOV por cada paso de rueda
    public float minFov = 8f;
    public float maxFov = 55f;       // con z=-100 se ve el mapa completo de 100x100
    public float mapSize = 100f;
    public float followSpeed = 6f;
    public float pickRadius = 2f;    // qué tan cerca debe ser el click de un Man/vaca

    private Transform target;
    private Vector2 lastMouse;

    void Awake()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (!GlobalData.mapReady) return;   // no hace nada mientras está el menú
        Mouse m = Mouse.current;
        if (m == null) return;

        Vector2 mouse = m.position.ReadValue();

        // Zoom
        float scroll = m.scroll.ReadValue().y;
        if (scroll != 0f)
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - Mathf.Sign(scroll) * zoomStep, minFov, maxFov);

        // Pan
        bool dragging = m.rightButton.isPressed || m.middleButton.isPressed;
        bool justPressed = m.rightButton.wasPressedThisFrame || m.middleButton.wasPressedThisFrame;
        if (dragging)
        {
            target = null;                                   // arrastrar cancela el seguimiento
            if (!justPressed)
            {
                float perPixel = 2f * Mathf.Abs(transform.position.z)
                               * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) / Screen.height;
                Vector2 d = (mouse - lastMouse) * perPixel;
                transform.position -= new Vector3(d.x, d.y, 0f);
            }
        }
        lastMouse = mouse;

        // Seguir
        if (m.leftButton.wasPressedThisFrame) target = PickNearest(mouse);
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) target = null;

        if (target != null)   // si el Man/vaca muere, Unity lo detecta como null
        {
            Vector3 goal = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, goal, followSpeed * Time.deltaTime);
        }

        // No salirse del mapa
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, 0f, mapSize);
        p.y = Mathf.Clamp(p.y, 0f, mapSize);
        transform.position = p;
    }

    Transform PickNearest(Vector2 screen)
    {
        Vector3 w = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(transform.position.z)));
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