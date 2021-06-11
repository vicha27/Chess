using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool PausePressed = false;
    bool optionsSelected = false;

    public GameObject pauseMenuUI;

    void Update()
    {
        if (GameIsPaused)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        optionsSelected = false;
    }
    public void Pause()
    {
        if (!optionsSelected)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void Options()
    {
        pauseMenuUI.SetActive(false);
        optionsSelected = true;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        PauseMenu.GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }
}
