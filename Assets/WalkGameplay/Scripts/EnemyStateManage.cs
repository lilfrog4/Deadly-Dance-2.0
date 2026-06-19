using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    // ★★★ СТАТИЧЕСКАЯ ПЕРЕМЕННАЯ (доступна везде) ★★★
    private static bool enemyIsDie = false;
    private static bool isInitialized = false;
    
    void Awake()
    {
        // Чтобы объект не уничтожался между сценами (опционально)
        // Если хочешь, чтобы менеджер жил между сценами — раскомментируй:
        // DontDestroyOnLoad(gameObject);
    }
    
    // ★★★ УСТАНОВИТЬ ЗНАЧЕНИЕ ★★★
    public static void SetEnemyDie(bool value)
    {
        enemyIsDie = value;
        isInitialized = true;
        Debug.Log($"💀 EnemyIsDie установлен в: {value}");
    }
    
    // ★★★ ПОЛУЧИТЬ ЗНАЧЕНИЕ ★★★
    public static bool GetEnemyDie()
    {
        if (!isInitialized)
        {
            Debug.Log("ℹ️ EnemyStateManager ещё не инициализирован, возвращаю false");
            return false;
        }
        return enemyIsDie;
    }
    
    // ★★★ ПРОВЕРИТЬ, УСТАНОВЛЕНА ЛИ ПЕРЕМЕННАЯ ★★★
    public static bool HasValue()
    {
        return isInitialized;
    }
    
    // ★★★ СБРОСИТЬ ЗНАЧЕНИЕ ★★★
    public static void ResetEnemyDie()
    {
        enemyIsDie = false;
        isInitialized = true; // Оставляем true, но значение false
        Debug.Log("🔄 EnemyIsDie сброшен в false");
    }
}