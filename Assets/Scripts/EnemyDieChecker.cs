using UnityEngine;

public class EnemyDieChecker : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string enemyName = "Enemy"; // Имя врага
    [SerializeField] private bool deactivateIfDie = true; // Деактивировать если мёртв
    [SerializeField] private bool destroyIfDie = false; // Уничтожить если мёртв
    
    void Start()
    {
        // ★★★ ПРОВЕРЯЕМ СОСТОЯНИЕ ПРИ ЗАГРУЗКЕ СЦЕНЫ ★★★
        bool enemyIsDie = EnemyStateManager.GetEnemyDie();
        
        Debug.Log($"🔍 Проверка состояния врага: enemyIsDie = {enemyIsDie}");
        Debug.Log($"📦 Враг: {gameObject.name}");
        
        if (enemyIsDie)
        {
            Debug.Log($"💀 Враг {gameObject.name} должен быть мёртв!");
            
            if (deactivateIfDie)
            {
                gameObject.SetActive(false);
                Debug.Log($"🔴 Враг {gameObject.name} деактивирован");
            }
            
            if (destroyIfDie)
            {
                Destroy(gameObject);
                Debug.Log($"💥 Враг {gameObject.name} уничтожен");
            }
            
            // ★★★ СБРАСЫВАЕМ СОСТОЯНИЕ ПОСЛЕ ПРИМЕНЕНИЯ ★★★
            // Чтобы при следующем переходе враг не был мёртв
            EnemyStateManager.ResetEnemyDie();
            Debug.Log("🔄 Состояние enemyIsDie сброшено");
        }
        else
        {
            Debug.Log($"✅ Враг {gameObject.name} жив");
        }
    }
}