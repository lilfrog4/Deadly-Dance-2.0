using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class AppearAnimations : MonoBehaviour
{
    private float PlayerZcoord = -4.143f;
    private float InitialZcoord;
    private float PathLength;
    public bool AnimationInProcess;
    void Start()
    {
        
        InitialZcoord = transform.position.z;
        
        PathLength = Mathf.Abs(InitialZcoord - PlayerZcoord);
        StartCoroutine(ScaleUp());
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Debug.Log(PathLength);
    }

    void Update()
    {
        
    }

    // IEnumerator ScaleUp()
    // {
    //     float scale = 1f / 10f;
    //     transform.localScale /= 10f;
    //     while (scale < 1f)
    //     {
    //         scale *= 1.05f + Time.deltaTime;
    //         transform.localScale *= 1.05f + Time.deltaTime;
    //         yield return null;
    //     }
    //     transform.localScale = new Vector3(1f, 1f, 1f);
    // }
    IEnumerator ScaleUp()
    {
        AnimationInProcess = true;
        float DistanceTraveled = 0f;
        while (DistanceTraveled < 0.2f * PathLength)
        {
            DistanceTraveled = Mathf.Abs(transform.position.z - InitialZcoord);
            float coef = (DistanceTraveled / PathLength) / 0.2f;
            transform.localScale = new Vector3(coef, coef, coef);
            yield return null;
        }
        transform.localScale = new Vector3(1f, 1f, 1f);
        AnimationInProcess = false;
    }
}