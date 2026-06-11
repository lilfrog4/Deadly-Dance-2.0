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
    private Dictionary<string, int> remaining = new Dictionary<string, int>();
    private string sceneToLoad = ""; // Сцена для загрузки после текущего диалога
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        panel.SetActive(false);
        
        foreach (Dialogue d in dialogues)
            remaining[d.name] = d.uses;
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
                int left = remaining.ContainsKey(t.name) ? remaining[t.name] : 0;
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
                int left = remaining.ContainsKey(b.name) ? remaining[b.name] : 0;
                if (left != 0) StartDlg(b.name);
                return;
            }
        }
    }
    
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
        
        if (remaining[name] > 0) remaining[name]--;
        if (remaining[name] == 0) Debug.Log($"Диалог {name} закончился");
    }
    
    private void Close()
    {
        panel.SetActive(false);
        isActive = false;
        current = null;
        currentColors = null;
        
        PlayerController.IsMovementBlocked = false;
        
        // Загружаем следующую сцену, если указана
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"Загрузка сцены: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad);
        }
        
        sceneToLoad = "";
    }
    
    public int GetRemainingUses(string triggerName)
    {
        if (remaining.ContainsKey(triggerName))
            return remaining[triggerName];
        
        foreach (Dialogue d in dialogues)
            if (d.name == triggerName)
                return d.uses;
        
        return 0;
    }
    
    // ========== ДОБАВЛЕННЫЙ МЕТОД ==========
    public bool IsDialogueActive()
    {
        return isActive;
    }
    // ======================================
}