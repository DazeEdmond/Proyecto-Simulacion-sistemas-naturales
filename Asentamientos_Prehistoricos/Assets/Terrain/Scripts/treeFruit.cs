using UnityEngine;

public class treeFruit : MonoBehaviour
{
    private Animator treeanim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        treeanim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (treeanim.GetBool("hasFruit"))
            {
                treeanim.SetBool("hasFruit", false);
            }
            else
            {
                treeanim.SetBool("hasFruit", true);
            }
        }
    }
}
