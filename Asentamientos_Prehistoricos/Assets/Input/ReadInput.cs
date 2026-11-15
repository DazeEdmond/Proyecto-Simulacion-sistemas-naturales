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
        Debug.Log(input); // Make sure it's ok
    }

}
