using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float delayBeforeMove = 0f;
    
    [Header("Настройки триггера")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool destroyOnTrigger = true;
    [SerializeField] private bool moveOnlyOnce = true;
    
    [Header("Настройки врага")]
    [SerializeField] private bool returnToStart = false;
    [SerializeField] private float waitAtTarget = 1f;
    
    [Header("Настройки вращения")]
    [SerializeField] private float targetRotationZ = 0f;
    
    [Header("Активация объекта после движения")]
    [SerializeField] private GameObject objectToActivate; // Объект для активации
    [SerializeField] private float activationDelay = 2f; // Задержка перед активацией (2 секунды)
    [SerializeField] private bool activateAfterMove = true; // Активировать после движения
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private bool isMoving = false;
    private bool hasMoved = false;
    private Collider2D myCollider;
    
    void Start()
    {
        Debug.Log("✅ EnemyMover Start() вызван");
        
        startPosition = transform.position;
        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, 0, targetRotationZ);
        
        Debug.Log($"🔄 Текущий поворот: {transform.eulerAngles.z}°");
        Debug.Log($"🎯 Целевой поворот: {targetRotationZ}°");
        
        if (targetPosition != null)
        {
            Debug.Log($"🎯 Целевая позиция: {targetPosition.position}");
        }
        
        // ★★★ ПРОВЕРКА ОБЪЕКТА ДЛЯ АКТИВАЦИИ ★★★
        if (objectToActivate != null)
        {
            Debug.Log($"📦 Объект для активации: {objectToActivate.name}, Сейчас активен: {objectToActivate.activeSelf}");
            // Убеждаемся, что объект выключен
            if (objectToActivate.activeSelf)
            {
                objectToActivate.SetActive(false);
                Debug.Log($"🔴 Объект {objectToActivate.name} деактивирован");
            }
        }
        else
        {
            Debug.Log("ℹ️ Объект для активации не назначен");
        }
        
        // Проверяем коллайдер
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.Log("⚠️ Нет коллайдера! Добавляю...");
            BoxCollider2D newCol = gameObject.AddComponent<BoxCollider2D>();
            newCol.isTrigger = true;
            newCol.size = new Vector2(1f, 1f);
            myCollider = newCol;
            Debug.Log("✅ Коллайдер добавлен");
        }
        else
        {
            Debug.Log($"✅ Коллайдер найден: {myCollider.GetType().Name}, IsTrigger = {myCollider.isTrigger}");
            if (!myCollider.isTrigger)
            {
                myCollider.isTrigger = true;
                Debug.Log("✅ Коллайдер теперь триггер");
            }
        }
        
        if (targetPosition == null)
        {
            Debug.LogError("❌ Target Position не назначен!");
        }
        
        Debug.Log("✅ Start() завершён");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"🔥 Триггер сработал! Объект: {other.gameObject.name}, Тег: {other.tag}");
        
        if (isMoving) 
        {
            Debug.Log("⏳ Уже движется");
            return;
        }
        
        if (moveOnlyOnce && hasMoved) 
        {
            Debug.Log("⏳ Уже двигался");
            return;
        }
        
        if (other.CompareTag(targetTag))
        {
            Debug.Log("🎯 Игрок активировал движение!");
            
            PlayerController.IsMovementBlocked = true;
            Debug.Log("🔒 Движение игрока заблокировано");
            
            StartCoroutine(MoveEnemyCoroutine());
            
            if (destroyOnTrigger)
            {
                if (myCollider != null) myCollider.enabled = false;
            }
        }
        else
        {
            Debug.Log($"❌ Не тот тег! Ожидался: {targetTag}, Получен: {other.tag}");
        }
    }
    
    IEnumerator MoveEnemyCoroutine()
    {
        Debug.Log("🚀 Корутина запущена!");
        isMoving = true;
        
        if (delayBeforeMove > 0)
        {
            Debug.Log($"⏳ Ожидание {delayBeforeMove} сек...");
            yield return new WaitForSeconds(delayBeforeMove);
        }
        
        if (targetPosition != null)
        {
            Debug.Log($"🎯 Двигаюсь к цели: {targetPosition.position}");
            yield return StartCoroutine(MoveAndRotateCoroutine(targetPosition.position, targetRotation));
            hasMoved = true;
            Debug.Log($"✅ Враг достиг цели! Позиция: {transform.position}, Поворот: {transform.eulerAngles.z}°");
            
            if (returnToStart)
            {
                Debug.Log($"⏳ Ожидание {waitAtTarget} сек перед возвратом...");
                yield return new WaitForSeconds(waitAtTarget);
                
                Debug.Log($"🔄 Возвращаюсь на старт: {startPosition}");
                yield return StartCoroutine(MoveAndRotateCoroutine(startPosition, startRotation));
                hasMoved = false;
                Debug.Log($"✅ Вернулся на старт! Позиция: {transform.position}, Поворот: {transform.eulerAngles.z}°");
            }
        }
        else
        {
            Debug.LogError("❌ Target Position не назначен!");
        }
        
        isMoving = false;
        
        PlayerController.IsMovementBlocked = false;
        Debug.Log("🔓 Движение игрока разблокировано");
        
        // ★★★ АКТИВАЦИЯ ОБЪЕКТА ЧЕРЕЗ 2 СЕКУНДЫ ★★★
        if (activateAfterMove && objectToActivate != null)
        {
            Debug.Log($"⏳ Ожидание {activationDelay} секунд перед активацией объекта...");
            yield return new WaitForSeconds(activationDelay);
            
            objectToActivate.SetActive(true);
            Debug.Log($"✅ Объект {objectToActivate.name} АКТИВИРОВАН!");
        }
        
        Debug.Log("🏁 Корутина завершена");
    }
    
    IEnumerator MoveAndRotateCoroutine(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / moveSpeed;
        float elapsed = 0f;
        
        if (duration < 0.01f) duration = 0.5f;
        
        Debug.Log($"📊 Движение из {startPos} в {targetPos}, {duration} сек");
        Debug.Log($"🔄 Поворот с {startRot.eulerAngles.z}° на {targetRot.eulerAngles.z}°");
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            
            yield return null;
        }
        
        transform.position = targetPos;
        transform.rotation = targetRot;
    }
    
    public void StartMoving()
    {
        Debug.Log("🎮 Вызван StartMoving() вручную");
        
        if (!isMoving && (!moveOnlyOnce || !hasMoved))
        {
            PlayerController.IsMovementBlocked = true;
            StartCoroutine(MoveEnemyCoroutine());
        }
    }
    
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
        
        if (targetPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPosition.position, 0.4f);
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawLine(transform.position, targetPosition.position);
        }
    }
}