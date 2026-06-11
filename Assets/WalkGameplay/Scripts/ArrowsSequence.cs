using UnityEngine;
using System.Collections;

public class ArrowsSequence : MonoBehaviour
{
    [SerializeField] private GameObject green;
    [SerializeField] private GameObject pink;
    [SerializeField] private GameObject red;
    
    private void Start()
    {
        StartCoroutine(PlaySequence());
    }
    
    private IEnumerator PlaySequence()
    {
        while (true)
        {
            // Выключаем все
            green.SetActive(false);
            pink.SetActive(false);
            red.SetActive(false);
            
            // Зелёная
            green.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            
            // Розовая
            pink.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            
            // Красная
            red.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            
            // Выключаем все
            green.SetActive(false);
            pink.SetActive(false);
            red.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }
}