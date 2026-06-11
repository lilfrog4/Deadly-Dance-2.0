using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectBattle : MonoBehaviour
{
    public void LoadFirstBattle()
    {
        SceneManager.LoadScene("FirstBattleScene");
        Time.timeScale = 1;
    }
    public void LoadSecondBattle()
    {
        SceneManager.LoadScene("SecondBattleScene");
        Time.timeScale = 1;
    }
    public void LoadThirdBattle()
    {
        SceneManager.LoadScene("ThirdBattleScene");
        Time.timeScale = 1;
    }
    public void LoadStoryModeScene()
    {
        SceneManager.LoadScene("1WalkScene");
        Time.timeScale = 1;
    }
    public void LoadEditor()
    {
        SceneManager.LoadScene("CustomBattleMenu");
        Time.timeScale = 1;
    }
}
