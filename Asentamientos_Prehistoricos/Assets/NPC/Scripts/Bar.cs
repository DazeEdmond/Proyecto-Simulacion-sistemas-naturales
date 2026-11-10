using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image bar;
    private float maxVal;
    private void Start() {
        bar.fillAmount = 1;
    }
    public void updateVar(float val,float maxVal)
    {
        bar.fillAmount = val/maxVal;
    }
}
