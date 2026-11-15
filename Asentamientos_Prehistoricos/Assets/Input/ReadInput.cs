using UnityEngine;
using TMPro;

public class ReadInput : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject map;

    void Awake()
    {
        map.SetActive(false);
    }

    void Start()
    {
        inputField.onSubmit.AddListener(ReadStringInput);
        inputField.ActivateInputField();        
    }

    void Update()
    {
        
    }
    public void ReadStringInput(string s)
    {
        if (!int.TryParse(s, out int result)){
            Debug.LogWarning("El campo está vacío o no es un número válido.");
            inputField.ActivateInputField();
            return;
        }

        GlobalData.globalSeed = result;
        inputField.gameObject.SetActive(false);
        map.SetActive(true);
    }

}
