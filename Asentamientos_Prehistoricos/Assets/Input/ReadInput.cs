using UnityEngine;

public class ReadInput : MonoBehaviour
{
    private string input;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ReadStringInput(string s)
    {
        input = s;
        if (int.TryParse(s, out int result))
            GlobalData.globalSeed = result;
        else
            Debug.LogWarning("El campo está vacío o no es un número válido.");

        // HAY QUE MATAR LA CAJA PARA QUE NO SALGA MÄS DESPUES DE 1er SETEADO
    }

}
