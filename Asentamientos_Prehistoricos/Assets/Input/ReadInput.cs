using System.Collections;
using UnityEngine;
using TMPro;

public class ReadInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject map;
    public CanvasGroup uiGroup;      // el CanvasGroup del Canvas
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
        inputField.onSubmit.AddListener(ReadStringInput);
        inputField.ActivateInputField();
    }

    public void ReadStringInput(string s)
    {
        if (starting) return;   // evita doble Enter durante el fade

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
        uiGroup.gameObject.SetActive(false);   // oculta todo el Canvas
        map.SetActive(true);                            // se genera el mapa
        yield return new WaitUntil(() => GlobalData.mapReady);  // espera a que termine
        foreach (GameObject g in waitForMap) g.SetActive(true); // ahora aparecen
    }
}