using UnityEngine;
using System.Collections;

public class FadeInWithTrigger : MonoBehaviour
{
    [Header("Настройки объекта")]
    [SerializeField] private GameObject objectToFade;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private bool startDisabled = true;
    
    [Header("Настройки триггера")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool destroyOnTrigger = true;
    [SerializeField] private bool fadeOnlyOnce = true;
    
    [Header("Настройки после появления")]
    [SerializeField] private bool deactivateAfterFade = false;
    [SerializeField] private float stayActiveTime = 2f;
    
    [Header("Активация объекта через 2 секунды")]
    [SerializeField] private GameObject objectToActivate; // Объект для активации
    [SerializeField] private float activationDelay = 2f; // Задержка (2 секунды)
    [SerializeField] private bool activateAfterFade = true; // Активировать после появления
    
    private SpriteRenderer spriteRenderer;
    private CanvasGroup canvasGroup;
    private bool isFading = false;
    private bool hasFaded = false;
    private Collider2D myCollider;
    
    void Start()
    {
        if (objectToFade != null)
        {
            spriteRenderer = objectToFade.GetComponent<SpriteRenderer>();
            canvasGroup = objectToFade.GetComponent<CanvasGroup>();
            
            if (startDisabled)
            {
                objectToFade.SetActive(false);
                Debug.Log($"🔴 Объект {objectToFade.name} деактивирован");
            }
            else
            {
                SetAlpha(0f);
                Debug.Log($"🔴 Объект {objectToFade.name} прозрачный (alpha = 0)");
            }
        }
        else
        {
            Debug.LogError("❌ Object To Fade не назначен!");
        }
        
        // ★★★ ПРОВЕРКА ОБЪЕКТА ДЛЯ АКТИВАЦИИ ★★★
        if (objectToActivate != null)
        {
            // Убеждаемся, что объект выключен
            if (objectToActivate.activeSelf)
            {
                objectToActivate.SetActive(false);
                Debug.Log($"🔴 Объект {objectToActivate.name} деактивирован");
            }
            Debug.Log($"📦 Объект для активации: {objectToActivate.name}");
        }
        else
        {
            Debug.Log("ℹ️ Объект для активации не назначен");
        }
        
        myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            if (!myCollider.isTrigger)
            {
                myCollider.isTrigger = true;
                Debug.Log("✅ Коллайдер теперь триггер");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Нет коллайдера на триггере! Добавьте BoxCollider2D с Is Trigger");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🔥 Триггер сработал! Объект: {other.gameObject.name}, Тег: {other.tag}");
        
        if (isFading) 
        {
            Debug.Log("⏳ Уже исчезает/появляется");
            return;
        }
        
        if (fadeOnlyOnce && hasFaded) 
        {
            Debug.Log("⏳ Уже сработал");
            return;
        }
        
        if (other.CompareTag(targetTag))
        {
            Debug.Log($"🎯 Игрок активировал появление объекта!");
            
            // Блокируем движение игрока
            PlayerController.IsMovementBlocked = true;
            Debug.Log("🔒 Движение игрока заблокировано");
            
            StartCoroutine(FadeInCoroutine());
            
            if (destroyOnTrigger && myCollider != null)
            {
                myCollider.enabled = false;
                Debug.Log("🔒 Триггер отключён");
            }
        }
        else
        {
            Debug.Log($"❌ Не тот тег! Ожидался: {targetTag}, Получен: {other.tag}");
        }
    }
    
    IEnumerator FadeInCoroutine()
    {
        isFading = true;
        
        if (objectToFade == null)
        {
            Debug.LogError("❌ Объект для появления не назначен!");
            isFading = false;
            PlayerController.IsMovementBlocked = false;
            yield break;
        }
        
        // Активируем объект для появления
        if (!objectToFade.activeSelf)
        {
            objectToFade.SetActive(true);
            Debug.Log($"✅ Объект {objectToFade.name} активирован");
        }
        
        // Устанавливаем начальную прозрачность (0)
        SetAlpha(0f);
        Debug.Log($"🎨 Начинаем появление: alpha 0 → 1 за {fadeDuration} сек");
        
        // Плавное появление
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            SetAlpha(alpha);
            
            if (Mathf.Floor(elapsed) > Mathf.Floor(elapsed - Time.deltaTime))
            {
                Debug.Log($"🎨 Прогресс: {elapsed:F1} сек, alpha: {alpha:F2}");
            }
            
            yield return null;
        }
        
        // Фиксируем конечную прозрачность (1)
        SetAlpha(1f);
        hasFaded = true;
        Debug.Log($"✅ Объект {objectToFade.name} полностью появился! (alpha = 1)");
        
        // ★★★ АКТИВАЦИЯ ОБЪЕКТА ЧЕРЕЗ 2 СЕКУНДЫ ★★★
        if (activateAfterFade && objectToActivate != null)
        {
            Debug.Log($"⏳ Ожидание {activationDelay} секунд перед активацией объекта...");
            yield return new WaitForSeconds(activationDelay);
            
            objectToActivate.SetActive(true);
            Debug.Log($"✅ Объект {objectToActivate.name} АКТИВИРОВАН!");
        }
        
        // Разблокируем движение игрока
        PlayerController.IsMovementBlocked = false;
        Debug.Log("🔓 Движение игрока разблокировано");
        
        // Если нужно отключить после появления
        if (deactivateAfterFade)
        {
            Debug.Log($"⏳ Ожидание {stayActiveTime} сек перед отключением...");
            yield return new WaitForSeconds(stayActiveTime);
            
            SetAlpha(0f);
            objectToFade.SetActive(false);
            Debug.Log($"🔴 Объект {objectToFade.name} деактивирован");
        }
        
        isFading = false;
        Debug.Log("🏁 Появление завершено");
    }
    
    void SetAlpha(float alpha)
    {
        if (objectToFade == null) return;
        
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
        
        if (spriteRenderer == null && canvasGroup == null)
        {
            Debug.LogWarning($"⚠️ На объекте {objectToFade.name} нет SpriteRenderer или CanvasGroup!");
        }
    }
    
    public void StartFade()
    {
        if (!isFading && (!fadeOnlyOnce || !hasFaded))
        {
            Debug.Log($"🎮 Запуск появления по команде!");
            PlayerController.IsMovementBlocked = true;
            StartCoroutine(FadeInCoroutine());
        }
        else
        {
            if (isFading) Debug.Log("⏳ Уже выполняется");
            if (fadeOnlyOnce && hasFaded) Debug.Log("⏳ Уже сработал");
        }
    }
    
    public void ResetFade()
    {
        hasFaded = false;
        isFading = false;
        PlayerController.IsMovementBlocked = false;
        
        if (objectToFade != null)
        {
            SetAlpha(0f);
            if (startDisabled) objectToFade.SetActive(false);
        }
        
        if (myCollider != null) myCollider.enabled = true;
        
        Debug.Log($"🔄 Состояние сброшено для {gameObject.name}");
    }
}