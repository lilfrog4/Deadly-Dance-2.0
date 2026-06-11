using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PunchVisualScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        float timer = 0f;
        while (timer < 0.15f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(transform.gameObject);
    }
}