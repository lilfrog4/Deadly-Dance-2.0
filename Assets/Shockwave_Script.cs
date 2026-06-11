using System.Collections;
using UnityEngine;

public class Shockwave_Script : MonoBehaviour
{
    public float ExplodeTime;
    private float InitialXScale;
    private float PeakXScale;
    private Color WaveColor;
    private float timer = 0f;
    void Start()
    {
        WaveColor = transform.GetComponentInChildren<SpriteRenderer>().color;
        InitialXScale = transform.localScale.x;
        PeakXScale = InitialXScale * 10f;
        StartCoroutine(Explode());
    }

    void Update()
    {
        transform.GetComponentInChildren<SpriteRenderer>().color = new Color(WaveColor.r, WaveColor.g, WaveColor.b, 1f - (timer / ExplodeTime));
    }

    IEnumerator Explode()
    {
        timer = 0f;
        while (timer < ExplodeTime / 2f)
        {
            transform.localScale = new Vector3(InitialXScale + (timer / (ExplodeTime / 2f) * (PeakXScale - InitialXScale)), transform.localScale.y, transform.localScale.z);
            yield return null;
            timer += Time.deltaTime;
        }
        while (timer < ExplodeTime)
        {
            yield return null;
            transform.localScale = new Vector3(PeakXScale - ((timer - ExplodeTime / 2f) / (ExplodeTime / 2f) * (PeakXScale - InitialXScale)), transform.localScale.y, transform.localScale.z);
            timer += Time.deltaTime;
        }
        Destroy(gameObject);
    }
}