
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;

public class ReadInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject map;
    public Button resetButton;

    // Mapa de calor / humedad
    public Button heatButton;              
    public Button humidityButton;         
    public Button normalButton;         
    public GameObject heatGrid;           
    public TilemapRenderer terrainRenderer; 
    public TilemapRenderer heatRenderer;   

    public CanvasGroup uiGroup;
    public float fadeTime = 0.6f;
    public GameObject[] waitForMap;

    // Estadísticas Village
    public GameObject statsCanvas;
    public TMP_Text nManText;
    public TMP_Text foodText;
    public TMP_Text waterText;
    public Button forceButton;

    private string nManLabel, foodLabel, waterLabel; // Labels
    private int shownPeople = -1, shownFood = -1, shownWater = -1;  // Último valor pintado

    private bool starting = false;

    void Awake()
    {
        // Verificar que existen Label
        if (nManText != null) nManLabel = nManText.text.Trim();
        if (foodText != null) foodLabel = foodText.text.Trim();
        if (waterText != null) waterLabel = waterText.text.Trim();

        map.SetActive(false);
        foreach (GameObject g in waitForMap) g.SetActive(false);
    }

    void Start()
    {
        GlobalData.mapReady = false;
        inputField.onSubmit.AddListener(ReadStringInput);

        // Botones
        resetButton.onClick.AddListener(ResetGame);
        heatButton.onClick.AddListener(ToggleHeatMap);
        humidityButton.onClick.AddListener(ToggleHumidityMap);
        normalButton.onClick.AddListener(ToggleNormalMap);
        forceButton.onClick.AddListener(ForceMigration);

        // Estadísticas - Hasta seleccionar, mostrar.
        GlobalData.selectedVillage = null;
        if (statsCanvas != null) statsCanvas.SetActive(false);

        GlobalData.showHeat = false;
        GlobalData.showHumidity = false;

        // Orden Render
        terrainRenderer.sortingOrder = -2;
        heatRenderer.sortingOrder = -1;

        // GridH. Ver o no ver es la pregunta
        heatGrid.SetActive(true);
        heatRenderer.enabled = false;

        GlobalData.mapReady = true;
    }

    // Estadísticas
    void Update()
    {
        if (statsCanvas == null) return;

        village v = GlobalData.selectedVillage;
        bool show = v != null;

        if (statsCanvas.activeSelf != show)
        {
            statsCanvas.SetActive(show);
            shownPeople = shownFood = shownWater = -1; // Repintar encima
        }
        if (show) RefreshStats(v);
    }

    void RefreshStats(village v)
    {
        int people = v.ActualVillagers;
        int food = Mathf.RoundToInt(v.foodAmount);
        int water = Mathf.RoundToInt(v.WaterAmount);

        if (nManText != null && people != shownPeople)
        {
            nManText.text = nManLabel + " " + people;
            shownPeople = people;
        }
        if (foodText != null && food != shownFood)
        {
            foodText.text = foodLabel + " " + food + " / " + v.MaxFood;
            shownFood = food;
        }
        if (waterText != null && water != shownWater)
        {
            waterText.text = waterLabel + " " + water + " / " + v.MaxWater;
            shownWater = water;
        }

        if (forceButton != null) forceButton.interactable = v.CanMigrate;
    }

    // Force Migration Button
    void ForceMigration()
    {
        village v = GlobalData.selectedVillage;
        if (v != null) v.ForceMigration();
    }

    // Normal Button
    void ToggleNormalMap()
    {
        GlobalData.showHeat = false;
        GlobalData.showHumidity = false;
        ApplyOverlay();
    }


    // Heat Button
    void ToggleHeatMap()
    {
        GlobalData.showHeat = !GlobalData.showHeat;
        GlobalData.showHumidity = false;
        ApplyOverlay();
    }

    // Humidity Button 
    void ToggleHumidityMap()
    {
        GlobalData.showHumidity = !GlobalData.showHumidity;
        GlobalData.showHeat = false;
        ApplyOverlay();
    }

    // Repintar
    void ApplyOverlay()
    {
        bool show = GlobalData.showHeat || GlobalData.showHumidity;
        if (show) map.GetComponent<NPCs.Generation>().PaintOverlay();
        heatRenderer.enabled = show;
    }

    // Seed - Robuzto
    public void ReadStringInput(string s)
    {
        if (starting) return;

        if (!int.TryParse(s, out int result))
        {
            Debug.LogWarning("El campo está vacío o no es un número válido.");
            inputField.ActivateInputField();
            return;
        }

        GlobalData.globalSeed = result;
        starting = true;
        StartCoroutine(FadeAndStart());
    }

    // Reset Button - Función 
    void ResetGame()
    {
        GlobalData.skipIntro = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Transición
    IEnumerator FadeAndStart()
    {
        // Desactiva las interacciones con la interfaz
        uiGroup.interactable = false;
        uiGroup.blocksRaycasts = false;

        // Reduce progresivamente la opacidad de la interfaz
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            uiGroup.alpha = 1f - Mathf.Clamp01(t / fadeTime);
            yield return null;
        }

        // Oculta la interfaz y activa el mapa
        uiGroup.alpha = 0f;
        uiGroup.gameObject.SetActive(false);

        map.SetActive(true);

        // Espera a que finalice la generación del mapa
        yield return new WaitUntil(() => GlobalData.mapReady);

        // Activa los elementos que dependen del mapa
        foreach (GameObject g in waitForMap) g.SetActive(true);
    }
}
