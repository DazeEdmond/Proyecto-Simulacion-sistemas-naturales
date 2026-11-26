using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingSlider : MonoBehaviour
{
    // Argumentos
    public Param param;
    public string title;
    public bool wholeNumbers;
    public Slider slider;
    public TMP_Text label;

    // Constructor
    void Start()
    {
        slider.wholeNumbers = wholeNumbers;
        slider.value = GlobalData.Get(param);
        slider.onValueChanged.AddListener(OnChange); // Si cambia, cambiar
        OnChange(slider.value); // Primer valor
    }

    // Métodos
    void OnChange(float v)
    {
        GlobalData.Set(param, v);
        label.text = title + ": " + v.ToString("0.##");
    }
}