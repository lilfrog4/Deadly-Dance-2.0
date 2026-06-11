using UnityEngine;

public class SimplePause : MonoBehaviour
{
    public GameObject pauseScreen;
    private bool isPaused = false;
    private DialogueSystem dialogueSystem;

    private void Start()
    {
        dialogueSystem = FindObjectOfType<DialogueSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ВАЖНО: нужно добавить скобки () после имени метода
            bool isDialogueActive = dialogueSystem != null && dialogueSystem.IsDialogueActive();
            
            if (!isDialogueActive)
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void ResumeButton()
    {
        TogglePause();
    }
}