using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2); //Classic = 2
    }

    public void PlayTutorial()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //Tutorial = 1
    }
    //Need to make a third for Arising and a 4rth for Practice, etc. etc.

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
