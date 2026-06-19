using UnityEngine;
using UnityEngine.UI;

public class KillEnemyButton : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string enemyName = "Enemy";
    
    void Start()
    {
        // Автоматически находим кнопку на объекте
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnButtonClick);
            Debug.Log("✅ Кнопка убийства врага настроена");
        }
    }
    
    public void OnButtonClick()
    {
        Debug.Log("💀 Кнопка убийства врага нажата!");
        
        // ★★★ УСТАНАВЛИВАЕМ enemyIsDie = true ★★★
        EnemyStateManager.SetEnemyDie(true);
        Debug.Log("💀 EnemyIsDie установлен в TRUE");
    }
    
    // Можно также вызвать этот метод напрямую из OnClick
    public void KillEnemy()
    {
        OnButtonClick();
    }
}