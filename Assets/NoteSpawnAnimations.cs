using System.Collections;
using UnityEngine;

public class NoteSpawnAnimations : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ScaleUp());
    }

    void Update()
    {
        
    }

    IEnumerator ScaleUp()
    {
        float scale = 1f / 50f;
        transform.localScale /= 50f;
        while (scale < 1f)
        {
            scale *= 1.1f + Time.deltaTime;
            transform.localScale *= 1.1f + Time.deltaTime;
            yield return null;
        }
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}