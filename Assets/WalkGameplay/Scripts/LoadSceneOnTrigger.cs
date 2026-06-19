using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnTrigger : MonoBehaviour
{
    [Header("Настройки загрузки")]
    [SerializeField] private string sceneToLoad = "Level2"; // Имя сцены для загрузки
    [SerializeField] private float delayBeforeLoad = 0f; // Задержка перед загрузкой
    [SerializeField] private bool loadByIndex = false; // Загружать по индексу?
    [SerializeField] private int sceneIndex = 1; // Индекс сцены (если loadByIndex = true)
    
    [Header("Позиция игрока в новой сцене")]
    [SerializeField] private Vector2 playerPosition = new Vector2(0, 0); // Координаты игрока
    [SerializeField] private bool setPlayerPosition = true; // Устанавливать позицию?
    
    [Header("Настройки триггера")]
    [SerializeField] private string targetTag = "Player"; // Тег объекта, который активирует триггер
    [SerializeField] private bool destroyOnTrigger = true; // Уничтожить триггер после использования
    [SerializeField] private bool loadOnlyOnce = true; // Загружать только один раз
    
    private bool isLoaded = false;
    
    // Статическая переменная для передачи позиции между сценами
    private static Vector2? nextPlayerPosition = null;
    private static string nextSceneName = "";
    private static int nextSceneIndex = -1;
    private static bool isLoading = false;
    
    void Start()
    {
        // Проверяем, что коллайдер настроен как триггер
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("Коллайдер на объекте " + gameObject.name + " не является триггером! Включите Is Trigger.");
        }
        
        // Проверяем, нужно ли установить позицию игрока (при загрузке сцены)
        if (setPlayerPosition && nextPlayerPosition.HasValue)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = nextPlayerPosition.Value;
                Debug.Log($"Позиция игрока установлена: {nextPlayerPosition.Value}");
                nextPlayerPosition = null; // Сбрасываем
            }
            else
            {
                Debug.LogWarning("Игрок с тегом 'Player' не найден в сцене!");
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что триггер ещё не сработал
        if (isLoaded && loadOnlyOnce) return;
        
        // Проверяем тег объекта, который вошёл в триггер
        if (other.CompareTag(targetTag))
        {
            isLoaded = true;
            Debug.Log($"Триггер '{gameObject.name}' активирован! Загрузка сцены...");
            
            // Сохраняем позицию для следующей сцены
            if (setPlayerPosition)
            {
                nextPlayerPosition = playerPosition;
                Debug.Log($"Сохранена позиция для новой сцены: {playerPosition}");
            }
            
            // Загружаем сцену с задержкой или без
            if (delayBeforeLoad > 0)
            {
                Invoke(nameof(LoadScene), delayBeforeLoad);
            }
            else
            {
                LoadScene();
            }
            
            // Уничтожаем триггер, если нужно
            if (destroyOnTrigger)
            {
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }
        }
    }
    
    void LoadScene()
    {
        if (loadByIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    // Сброс сохранённой позиции (можно вызвать из другого скрипта)
    public static void ResetPosition()
    {
        nextPlayerPosition = null;
    }
    
    // Проверка, есть ли сохранённая позиция
    public static bool HasPosition()
    {
        return nextPlayerPosition.HasValue;
    }
    
    // Получить сохранённую позицию
    public static Vector2 GetPosition()
    {
        return nextPlayerPosition ?? Vector2.zero;
    }
    
    // Очистить позицию
    public static void ClearPosition()
    {
        nextPlayerPosition = null;
    }
    
    // Визуализация триггера в редакторе
    void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
        
        // Визуализируем позицию, куда попадёт игрок
        if (setPlayerPosition)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(playerPosition, 0.3f);
            
            // Рисуем линию от триггера к позиции
            if (col != null)
            {
                Gizmos.color = new Color(1, 1, 0, 0.3f);
                Gizmos.DrawLine(col.bounds.center, playerPosition);
            }
        }
    }
}