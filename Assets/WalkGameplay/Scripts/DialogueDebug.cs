using UnityEngine;

public class DialogueDebug : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private GameObject testObject; // Объект, который запускает диалог (триггер или кнопка)
    
    [Header("Настройки")]
    [SerializeField] private bool autoCheck = true;
    
    private void Start()
    {
        if (dialogueSystem == null)
            dialogueSystem = FindObjectOfType<DialogueSystem>();
        
        if (autoCheck)
            InvokeRepeating("CheckDialogue", 0.5f, 1f);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CheckDialogue();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("=== ПРИНУДИТЕЛЬНЫЙ ЗАПУСК ДИАЛОГА ===");
            if (testObject != null)
                Debug.Log($"Попытка запустить диалог для: {testObject.name}");
            else
                Debug.LogWarning("Test Object не назначен в инспекторе!");
        }
    }
    
    private void CheckDialogue()
    {
        Debug.Log("========== ДИАГНОСТИКА ДИАЛОГОВ ==========");
        
        // Проверяем наличие диалогов
        if (dialogueSystem == null)
        {
            Debug.LogError("DialogueSystem не найден в сцене!");
            return;
        }
        
        // Получаем приватные поля через рефлексию
        var dialoguesField = dialogueSystem.GetType().GetField("dialogues", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (dialoguesField == null)
        {
            Debug.LogError("Не удалось получить массив dialogues через рефлексию!");
            return;
        }
        
        var dialogues = dialoguesField.GetValue(dialogueSystem) as System.Array;
        
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogWarning("Нет зарегистрированных диалогов!");
            return;
        }
        
        Debug.Log($"Найдено диалогов: {dialogues.Length}");
        
        // Выводим каждый диалог
        foreach (var d in dialogues)
        {
            var nameField = d.GetType().GetField("name");
            var phrasesField = d.GetType().GetField("phrases");
            var colorsField = d.GetType().GetField("phraseColors");
            var usesField = d.GetType().GetField("uses");
            
            string name = nameField?.GetValue(d) as string ?? "unknown";
            string[] phrases = phrasesField?.GetValue(d) as string[];
            Color[] colors = colorsField?.GetValue(d) as Color[];
            int uses = usesField != null ? (int)usesField.GetValue(d) : 0;
            
            Debug.Log($"--- Диалог: {name} ---");
            Debug.Log($"  Использований: {uses}");
            
            if (phrases == null || phrases.Length == 0)
            {
                Debug.LogWarning($"  Фразы: ОТСУТСТВУЮТ!");
            }
            else
            {
                Debug.Log($"  Фраз: {phrases.Length}");
                for (int i = 0; i < phrases.Length; i++)
                {
                    Debug.Log($"    [{i}]: \"{phrases[i]}\"");
                }
            }
            
            if (colors == null || colors.Length == 0)
            {
                Debug.LogWarning($"  Цвета: НЕ ЗАДАНЫ (будут белыми)");
            }
            else
            {
                Debug.Log($"  Цветов: {colors.Length}");
                for (int i = 0; i < colors.Length; i++)
                {
                    Color c = colors[i];
                    Debug.Log($"    [{i}]: RGBA({c.r:F2}, {c.g:F2}, {c.b:F2}, {c.a:F2}) → {(c.a < 0.1f ? "ПРОЗРАЧНЫЙ!" : "OK")}");
                }
                
                // Сравниваем количество фраз и цветов
                if (colors.Length < phrases.Length)
                {
                    Debug.LogWarning($"  ⚠️ Цветов ({colors.Length}) меньше чем фраз ({phrases.Length}). Остальные фразы будут БЕЛЫМИ!");
                }
                else if (colors.Length > phrases.Length)
                {
                    Debug.LogWarning($"  ⚠️ Цветов ({colors.Length}) больше чем фраз ({phrases.Length}). Лишние цвета игнорируются.");
                }
            }
        }
        
        // Проверяем позиции кнопок (если есть WButtons)
        WButtons wButtons = FindObjectOfType<WButtons>();
        if (wButtons != null)
        {
            Debug.Log("=== ВИДИМЫЕ КНОПКИ (WButtons) ===");
            var buttonsField = wButtons.GetType().GetField("buttons",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (buttonsField != null)
            {
                var buttons = buttonsField.GetValue(wButtons) as System.Collections.Generic.List<GameObject>;
                if (buttons != null)
                {
                    foreach (var btn in buttons)
                    {
                        if (btn != null)
                            Debug.Log($"  Кнопка: {btn.name}, X = {btn.transform.position.x}, активна = {btn.activeSelf}");
                    }
                }
            }
        }
        
        // Проверяем скрытые триггеры
        GameObject[] hiddenTriggers = GameObject.FindGameObjectsWithTag("HiddenTrigger");
        Debug.Log($"=== СКРЫТЫЕ ТРИГГЕРЫ (HiddenTrigger) ===");
        foreach (var t in hiddenTriggers)
        {
            Debug.Log($"  Триггер: {t.name}, X = {t.transform.position.x}");
        }
        
        // Проверяем позицию игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log($"=== ИГРОК ===");
            Debug.Log($"  Позиция X = {player.transform.position.x}");
            
            // Ближайший триггер
            float minDist = 999f;
            GameObject nearest = null;
            foreach (var t in hiddenTriggers)
            {
                float dist = Mathf.Abs(t.transform.position.x - player.transform.position.x);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = t;
                }
            }
            if (nearest != null)
            {
                Debug.Log($"  Ближайший триггер: {nearest.name}, расстояние = {minDist:F2} (порог 0.5) → {(minDist < 0.5f ? "✅ МОЖЕТ СРАБОТАТЬ" : "❌ СЛИШКОМ ДАЛЕКО")}");
            }
        }
        
        Debug.Log("========== ДИАГНОСТИКА ЗАВЕРШЕНА ==========");
    }
}