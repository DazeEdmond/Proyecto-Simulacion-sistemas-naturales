using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SlideFlow : MonoBehaviour
{
    public GameObject[] slides;
    public TMP_InputField seedField;
    int current;

    void Awake()
    {
        current = GlobalData.skipIntro ? 1 : 0;
        GlobalData.skipIntro = false;
        Show(current);
    }

    void Update()
    {
        bool isLast = current >= slides.Length - 1;
        Keyboard k = Keyboard.current;
        if (!isLast && k != null)
        {
            if (k.spaceKey.wasPressedThisFrame || k.rightArrowKey.wasPressedThisFrame)
                Show(current + 1);
            else if (k.leftArrowKey.wasPressedThisFrame && current > 0)
                Show(current - 1);
        }
    }

    void Show(int i)
    {
        current = i;
        for (int n = 0; n < slides.Length; n++) slides[n].SetActive(n == i);
        if (i == slides.Length - 1) seedField.ActivateInputField();
    }
}