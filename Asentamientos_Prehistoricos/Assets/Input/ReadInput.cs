
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

    private bool starting = false;

    void Awake()
    {
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
