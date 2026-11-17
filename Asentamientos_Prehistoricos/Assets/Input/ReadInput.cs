using System.Collections;
using UnityEngine;
using TMPro;

public class ReadInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject map;
    public CanvasGroup uiGroup; 
    public float fadeTime = 0.6f;
    public GameObject[] waitForMap;

    [Header("Fondo")]
    public Transform background;
    public float backgroundNewX = -150f;
    public float backgroundNewY = -150f;

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
        inputField.ActivateInputField();
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
        Vector3 p = background.position;
        background.position = new Vector3(backgroundNewX, backgroundNewY, p.z); 
        StartCoroutine(FadeAndStart());
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
        
        map.SetActive(true);
        
        yield return new WaitUntil(() => GlobalData.mapReady); 
        
        foreach (GameObject g in waitForMap) g.SetActive(true);
    }
}