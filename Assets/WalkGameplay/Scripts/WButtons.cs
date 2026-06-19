using UnityEngine;
using System.Collections.Generic;

public class WButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();
    [SerializeField] private float tolerance = 0.5f; // Дистанция активации
    
    private Transform player;
    private DialogueSystem dialogueSystem;
    private bool isInitialized = false;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        dialogueSystem = FindObjectOfType<DialogueSystem>();
        
        if (player == null)
            Debug.LogError("WButtons: Игрок с тегом 'Player' не найден!");
        
        if (dialogueSystem == null)
            Debug.LogError("WButtons: DialogueSystem не найден в сцене!");
        
        // Инициализация кнопок
        foreach (GameObject btn in buttons)
        {
            if (btn != null)
            {
                // Проверяем, есть ли у кнопки CanvasGroup (для прозрачности)
                CanvasGroup cg = btn.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = 0f; // Прозрачная вместо выключенной
                    cg.interactable = false;
                    cg.blocksRaycasts = false;
                }
                else
                {
                    // Если нет CanvasGroup, просто выключаем
                    btn.SetActive(false);
                }
                
                Debug.Log($"WButtons: Кнопка '{btn.name}' инициализирована");
            }
        }
        
        isInitialized = true;
    }
    
    private void Update()
    {
        if (!isInitialized) return;
        if (player == null || dialogueSystem == null) return;
        
        float playerX = player.position.x;
        
        foreach (GameObject btn in buttons)
        {
            if (btn == null) continue;
            
            // Проверяем, остались ли у этого триггера использования
            string triggerName = btn.name;
            int remainingUses = dialogueSystem.GetRemainingUses(triggerName);
            
            // Если использований не осталось - кнопка не появляется
            if (remainingUses <= 0)
            {
                SetButtonActive(btn, false);
                continue;
            }
            
            // Проверяем расстояние до игрока
            float buttonX = btn.transform.position.x;
            bool shouldBeActive = Mathf.Abs(playerX - buttonX) < tolerance;
            
            SetButtonActive(btn, shouldBeActive);
            
            // Диагностика (для отладки)
            if (shouldBeActive)
            {
                Debug.Log($"WButtons: Кнопка '{btn.name}' активна (дистанция: {Mathf.Abs(playerX - buttonX)})");
            }
        }
    }
    
    private void SetButtonActive(GameObject btn, bool active)
    {
        // Проверяем, есть ли CanvasGroup
        CanvasGroup cg = btn.GetComponent<CanvasGroup>();
        
        if (cg != null)
        {
            // Используем CanvasGroup для плавного появления
            cg.alpha = active ? 1f : 0f;
            cg.interactable = active;
            cg.blocksRaycasts = active;
            
            // Обязательно включаем GameObject, чтобы CanvasGroup работал
            if (!btn.activeSelf) btn.SetActive(true);
        }
        else
        {
            // Обычный способ
            if (btn.activeSelf != active)
                btn.SetActive(active);
        }
    }
    
    // Метод для принудительного обновления кнопок
    public void RefreshButtons()
    {
        if (!isInitialized) return;
        
        foreach (GameObject btn in buttons)
        {
            if (btn == null) continue;
            
            string triggerName = btn.name;
            int remainingUses = dialogueSystem != null ? dialogueSystem.GetRemainingUses(triggerName) : 0;
            
            if (remainingUses <= 0)
            {
                SetButtonActive(btn, false);
            }
        }
    }
    
    // Визуализация в редакторе
    private void OnDrawGizmosSelected()
    {
        if (buttons == null) return;
        
        Gizmos.color = Color.cyan;
        foreach (GameObject btn in buttons)
        {
            if (btn == null) continue;
            
            // Рисуем зону активации
            Vector3 pos = btn.transform.position;
            Gizmos.DrawWireSphere(pos, tolerance);
            
            // Рисуем линию к игроку (если есть)
            if (player != null)
            {
                float dist = Mathf.Abs(player.position.x - pos.x);
                if (dist < tolerance)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(pos, player.position);
                }
            }
        }
    }
}