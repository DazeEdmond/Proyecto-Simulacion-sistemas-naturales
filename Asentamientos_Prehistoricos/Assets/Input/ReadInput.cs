
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ReadInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject map;
    public Button resetButton;

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
        resetButton.onClick.AddListener(ResetGame);
        GlobalData.mapReady = true;
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

    // Transición - :o 
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
