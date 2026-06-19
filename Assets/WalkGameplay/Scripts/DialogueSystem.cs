using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Text text;
    
    [System.Serializable]
    public class Dialogue
    {
        public string name;
        [TextArea] public string[] phrases;
        public Color[] phraseColors;
        public int uses = 1;
        public string nextScene = ""; // Имя сцены для загрузки после диалога
    }
    
    [SerializeField] private Dialogue[] dialogues;
    
    public Color white = new Color(1f, 1f, 1f, 1f);
    public Color red = new Color(1f, 0f, 0f, 1f);
    public Color green = new Color(0f, 1f, 0f, 1f);
    public Color blue = new Color(0f, 0f, 1f, 1f);
    public Color yellow = new Color(1f, 1f, 0f, 1f);
    public Color black = new Color(0f, 0f, 0f, 1f);
    public Color gray = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color cyan = new Color(0f, 1f, 1f, 1f);
    public Color magenta = new Color(1f, 0f, 1f, 1f);
    public Color purple = new Color(0.7f, 0f, 0.7f, 1f);
    public Color violet = new Color(0.93f, 0.51f, 0.93f, 1f);
    
    private Transform player;
    private bool isActive = false;
    private string[] current;
    private Color[] currentColors;
    private int index;
    private string sceneToLoad = ""; // Сцена для загрузки после текущего диалога
    
    // ★★★ СТАТИЧЕСКИЙ СЛОВАРЬ (общий для всех сцен) ★★★
    private static Dictionary<string, int> remaining = new Dictionary<string, int>();
    private static bool isInitialized = false; // Флаг, что словарь уже заполнен
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        panel.SetActive(false);
        
        // ★★★ ИНИЦИАЛИЗИРУЕМ СЛОВАРЬ ТОЛЬКО ОДИН РАЗ ★★★
        if (!isInitialized)
        {
            remaining.Clear();
            foreach (Dialogue d in dialogues)
            {
                remaining[d.name] = d.uses;
            }
            isInitialized = true;
            Debug.Log("DialogueSystem: Словарь инициализирован (первая загрузка)");
        }
        else
        {
            // Проверяем, нет ли новых диалогов, которых нет в словаре
            foreach (Dialogue d in dialogues)
            {
                if (!remaining.ContainsKey(d.name))
                {
                    remaining[d.name] = d.uses;
                    Debug.Log($"DialogueSystem: Добавлен новый диалог '{d.name}'");
                }
            }
            Debug.Log("DialogueSystem: Словарь уже существует, использования сохранены");
        }
    }
    
    private void Update()
    {
        if (player == null) return;
        
        if (isActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                index++;
                if (index < current.Length)
                    ShowPhrase();
                else
                    Close();
            }
            return;
        }
        
        CheckTriggers();
        CheckButtons();
<<<<<<< Updated upstream
=======
        CheckYButton();
>>>>>>> Stashed changes
    }
    
    private void ShowPhrase()
    {
        text.text = current[index];
        
        if (currentColors != null && index < currentColors.Length)
        {
            text.color = currentColors[index];
        }
        else
        {
            text.color = Color.white;
        }
    }
    
    private void CheckTriggers()
    {
        GameObject[] triggers = GameObject.FindGameObjectsWithTag("HiddenTrigger");
        foreach (GameObject t in triggers)
        {
            if (Mathf.Abs(t.transform.position.x - player.position.x) < 0.5f)
            {
                int left = GetRemainingUses(t.name);
                if (left != 0) StartDlg(t.name);
                return;
            }
        }
    }
    
    private void CheckButtons()
    {
        if (!Input.GetKeyDown(KeyCode.W)) return;
        
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("WButton");
        foreach (GameObject b in buttons)
        {
            if (b.activeSelf && Mathf.Abs(b.transform.position.x - player.position.x) < 0.5f)
            {
                int left = GetRemainingUses(b.name);
                if (left != 0) StartDlg(b.name);
                return;
            }
        }
    }
    
<<<<<<< Updated upstream
=======
    private void CheckYButton()
    {
        if (!Input.GetKeyDown(KeyCode.Y)) return;
        
        Debug.Log("Нажата клавиша Y! Поиск диалога для запуска...");
        
        foreach (Dialogue d in dialogues)
        {
            int left = GetRemainingUses(d.name);
            
            if (left > 0)
            {
                Debug.Log($"Запуск диалога '{d.name}' по клавише Y");
                StartDlg(d.name);
                return;
            }
        }
        
        Debug.Log("Нет доступных диалогов для запуска по клавише Y");
    }
    
>>>>>>> Stashed changes
    private void StartDlg(string name)
    {
        Dialogue currentDialogue = null;
        
        foreach (Dialogue d in dialogues)
        {
            if (d.name == name)
            {
                current = d.phrases;
                currentColors = d.phraseColors;
                currentDialogue = d;
                sceneToLoad = d.nextScene;
                break;
            }
        }
        if (current == null) return;
        
        isActive = true;
        index = 0;
        
        PlayerController.IsMovementBlocked = true;
        
        panel.SetActive(true);
        ShowPhrase();
        
        // ★★★ УМЕНЬШАЕМ ИСПОЛЬЗОВАНИЕ ★★★
        if (remaining.ContainsKey(name) && remaining[name] > 0)
        {
            remaining[name]--;
        }
        
        if (GetRemainingUses(name) == 0)
        {
            Debug.Log($"Диалог {name} закончился");
        }
    }
    
    private void Close()
    {
        panel.SetActive(false);
        isActive = false;
        current = null;
        currentColors = null;
        
        PlayerController.IsMovementBlocked = false;
        
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Загрузка сцены: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        
        sceneToLoad = "";
    }
    
    // ★★★ ПУБЛИЧНЫЙ МЕТОД ДЛЯ ПОЛУЧЕНИЯ ИСПОЛЬЗОВАНИЙ ★★★
    public int GetRemainingUses(string triggerName)
    {
        if (remaining.ContainsKey(triggerName))
            return remaining[triggerName];
        
        foreach (Dialogue d in dialogues)
            if (d.name == triggerName)
                return d.uses;
        
        return 0;
    }
    
<<<<<<< Updated upstream
    // ========== ДОБАВЛЕННЫЙ МЕТОД ==========
=======
    // ★★★ ПРОВЕРКА, АКТИВЕН ЛИ ДИАЛОГ ★★★
>>>>>>> Stashed changes
    public bool IsDialogueActive()
    {
        return isActive;
    }
<<<<<<< Updated upstream
    // ======================================
=======
    
    // ★★★ НОВЫЙ МЕТОД: Сброс всех использований (для отладки) ★★★
    public static void ResetAllUses()
    {
        remaining.Clear();
        isInitialized = false;
        Debug.Log("DialogueSystem: Все использования сброшены");
    }
    
    // ★★★ НОВЫЙ МЕТОД: Сброс конкретного диалога ★★★
    public static void ResetUses(string dialogueName, int newUses)
    {
        if (remaining.ContainsKey(dialogueName))
        {
            remaining[dialogueName] = newUses;
            Debug.Log($"DialogueSystem: Диалог '{dialogueName}' сброшен до {newUses} использований");
        }
    }
    
    // ★★★ НОВЫЙ МЕТОД: Получить количество использований (статический) ★★★
    public static int GetUses(string dialogueName)
    {
        if (remaining.ContainsKey(dialogueName))
            return remaining[dialogueName];
        return 0;
    }
>>>>>>> Stashed changes
}