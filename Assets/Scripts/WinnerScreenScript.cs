using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinnerScreenScript : MonoBehaviour
{
    public MusicManager mg;
    public GameObject background; 

    void Start()
    {
        StartCoroutine(WaitAndDoSomething());
       
    }

    private IEnumerator WaitAndDoSomething()
    {
        yield return null;
        yield return new WaitForSeconds(mg.mainSongForThisScene.clip.length + 0.1f);

        background.SetActive(true);
        mg.winSound.Play();
        
        Time.timeScale = 0;
    }

    public void ExitButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
   
}