using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject choiseBattleMenu;
    public void StartGame()
    {
        //SceneManager.LoadScene("FirstBattleScene");
        //Time.timeScale = 1;
        choiseBattleMenu.SetActive(true);
    }
    public void OptionsMenu()
    {
        //SceneManager.LoadScene("FirstBattleScene");
        //Time.timeScale = 1;
        optionsMenu.SetActive(true);
    }

    public void LoadCBMenu()
    {
        SceneManager.LoadScene("CustomBattleMenu");
    }

    public void ExitGame()
    {

        Application.Quit();


#if UNITY_EDITOR
        Debug.Log("Game is exiting... (Application.Quit() doesn't work in Editor)");
#endif
    }
    public void BackToMenuFromBattleList()
    {
        choiseBattleMenu.SetActive(false);
    }
    public void BackToMenuFromOptions()
    {
        optionsMenu.SetActive(false);
    }
}