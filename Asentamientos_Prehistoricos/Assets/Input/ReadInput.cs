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

    public CanvasGroup uiGroup;         // CanvasGroup de Slide_3_Canvas
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

    void ResetGame()
    {
        GlobalData.skipIntro = true;   // al recargar, SlideFlow arranca en el slide 2
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator FadeAndStart()
    {
        uiGroup.interactable = false;
        uiGroup.blocksRaycasts = false;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            uiGroup.alpha = 1f - Mathf.Clamp01(t / fadeTime);
            yield return null;
        }

        uiGroup.alpha = 0f;
        uiGroup.gameObject.SetActive(false);   // oculta Slide 3 (y su fondo)

        map.SetActive(true);

        yield return new WaitUntil(() => GlobalData.mapReady);

        foreach (GameObject g in waitForMap) g.SetActive(true);
    }
}