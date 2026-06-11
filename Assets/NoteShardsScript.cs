using System.Collections;
using UnityEngine;

public class NoteShardsScript : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        float timer = 0f;
        while (timer < 0.12f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(transform.gameObject);
    }

}
