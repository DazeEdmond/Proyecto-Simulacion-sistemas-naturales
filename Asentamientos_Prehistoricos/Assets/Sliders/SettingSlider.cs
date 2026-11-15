using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingSlider : MonoBehaviour
{
    public Param param;
    public string title;
    public bool wholeNumbers;
    public Slider slider;
    public TMP_Text label;

    void Start()
    {
        slider.wholeNumbers = wholeNumbers;
        slider.value = GlobalData.Get(param); // Por defecto
        slider.onValueChanged.AddListener(OnChange);
        OnChange(slider.value);
    }

    void OnChange(float v)
    {
        GlobalData.Set(param, v);
        label.text = title + ": " + v.ToString("0.##");
    }
}