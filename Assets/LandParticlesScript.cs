using UnityEngine;
using System.Collections;

public class LandParticlesScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayedDestroy());
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(0.12f);
        Destroy(transform.gameObject);
    }
}
