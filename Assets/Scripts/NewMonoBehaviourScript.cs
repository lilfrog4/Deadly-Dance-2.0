using UnityEngine;
using TMPro;
using System.Collections;

public class CanvasTimer : MonoBehaviour
{
    public GameObject canvasToActivate; // Сюда перетащите ваш Canvas
    public float activationTime = 3f;   // Время активации в секундах
    
    // Настройки для Typewriter (можно настроить в инспекторе)
    public TMP_Text textComponent;      // Перетащите сюда ваш TextMeshPro с Canvas
    public float typewriterDelay = 0.05f; // Задержка между символами

    void Start()
    {
        // Запускаем корутину с таймером
        StartCoroutine(ActivateCanvasAfterTime());
    }

    IEnumerator ActivateCanvasAfterTime()
    {
        // Ждем указанное количество секунд (реальное время, не зависит от пауз)
        yield return new WaitForSecondsRealtime(activationTime);
        
        // Активируем Canvas
        if (canvasToActivate != null)
        {
            canvasToActivate.SetActive(true);
            Debug.Log($"Canvas активирован через {activationTime} секунд!");
            
            // Запускаем эффект печати текста на активированном Canvas
            if (textComponent != null)
            {
                StartCoroutine(ShowText());
            }
            else
            {
                Debug.LogError("Ошибка: textComponent не назначен в инспекторе!");
            }
        }
        else
        {
            Debug.LogError("Ошибка: canvasToActivate не назначен в инспекторе!");
        }
    }
    
    IEnumerator ShowText()
    {
        string fullText = textComponent.text;     // Сохраняем полный текст
        textComponent.text = "";                   // Очищаем поле
        
        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i);  // Показываем i символов
            yield return new WaitForSeconds(typewriterDelay); // Ждём
        }
    }
}
