using UnityEngine;
using System.Collections.Generic;

public class WButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();
    
    private Transform player;
    private DialogueSystem dialogueSystem;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        dialogueSystem = FindObjectOfType<DialogueSystem>();
        
        foreach (GameObject btn in buttons)
        {
            if (btn != null) btn.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (player == null || dialogueSystem == null) return;
        
        float playerX = player.position.x;
        float tolerance = 0.5f;
        
        foreach (GameObject btn in buttons)
        {
            if (btn == null) continue;
            
            // Проверяем, остались ли у этого триггера использования
            string triggerName = btn.name;
            int remainingUses = dialogueSystem.GetRemainingUses(triggerName);
            
            // Если использований не осталось - кнопка не появляется
            if (remainingUses == 0)
            {
                if (btn.activeSelf) btn.SetActive(false);
                continue;
            }
            
            // Иначе включаем как обычно
            float buttonX = btn.transform.position.x;
            bool shouldBeActive = Mathf.Abs(playerX - buttonX) < tolerance;
            
            if (btn.activeSelf != shouldBeActive)
                btn.SetActive(shouldBeActive);
        }
    }
}