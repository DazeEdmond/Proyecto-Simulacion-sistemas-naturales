using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    public void OnClick()
    {
        GlobalData.skipIntro = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}