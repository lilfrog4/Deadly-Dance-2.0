using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public GameObject pauseScreen;
    public MusicManager manager;
    private bool isPaused = false;
    public GameObject Player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPauseScreen();
            Player.GetComponent<characterControls>().Paused = !Player.GetComponent<characterControls>().Paused;
        }
    }

    public void ShowPauseScreen()
    {
        isPaused = !isPaused;

        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (manager != null && manager.mainSongForThisScene != null)
        {
            if (isPaused)
            {
                manager.mainSongForThisScene.Pause();
            }
            else
            {
                manager.mainSongForThisScene.Play();
            }
        }
    }

    public void RestartButton()
    {
        Time.timeScale = 1;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ExitButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}