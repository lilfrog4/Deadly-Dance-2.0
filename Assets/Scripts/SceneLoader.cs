using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Настройки загрузки")]
    [SerializeField] private string sceneName = "Level2";
    [SerializeField] private bool loadByIndex = false;
    [SerializeField] private int sceneIndex = 1;
    
    [Header("Настройки перехода")]
    [SerializeField] private float delayBeforeLoad = 0f;
    [SerializeField] private GameObject loadingPanel;
    
    [Header("Настройки врага на целевой сцене")]
    [SerializeField] private string enemyName = "Enemy"; // Имя врага
    [SerializeField] private bool deactivateEnemy = true; // Деактивировать врага?
    
    public void LoadScene()
    {
        Debug.Log($"🎮 Загрузка сцены: {sceneName}");
        Debug.Log($"💀 Проверка enemyIsDie: {EnemyStateManager.GetEnemyDie()}");
        
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        
        // ★★★ ПЕРЕДАЁМ СОСТОЯНИЕ ВРАГА ПЕРЕД ЗАГРУЗКОЙ ★★★
        // Сохраняем состояние врага в статическую переменную
        // Она уже сохранена, просто проверяем
        
        if (delayBeforeLoad > 0)
        {
            Invoke(nameof(LoadSceneNow), delayBeforeLoad);
        }
        else
        {
            LoadSceneNow();
        }
    }
    
    private void LoadSceneNow()
    {
        if (loadByIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}