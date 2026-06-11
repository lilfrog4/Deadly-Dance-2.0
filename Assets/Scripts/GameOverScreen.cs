using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameOverScreen : MonoBehaviour
{
    public void SetUpTrue()
    {
        gameObject.SetActive(true);
    }

    public void RestartButton()
    {

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1;
    }
    public void ExitButton()
    {

        SceneManager.LoadScene("MainMenu");
    }
}
