using UnityEngine;

public class childtest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer GNote_Renderer = transform.Find("Model2").GetComponent<Renderer>();
        Renderer WNote_Renderer = transform.Find("Model").GetComponent<Renderer>();
        GNote_Renderer.material.color = Color.blue;
        WNote_Renderer.material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
