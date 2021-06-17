using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndGameMenu : MonoBehaviour
{
    public static bool GameHasEnded = false;

    public GameObject endGameMenuUI;

    public TextMeshProUGUI youWin;

    public TextMeshProUGUI youLose;

    public TextMeshProUGUI draw;

    // Update is called once per frame
    void Update()
    {
        if (GameHasEnded)
        {
            endGameMenuUI.SetActive(true);
            //Time.timeScale = 0f;
            if (BoardManager.isDraw) 
            {
                draw.gameObject.SetActive(true);
            }
            else
            {
                if (BoardManager.WhiteWon)
                    youWin.gameObject.SetActive(true);
                else
                    youLose.gameObject.SetActive(true);
            }
        }
    }

    public void PlayAgain()
    {
        CloseEverything();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Load scene called Game
    }

    public void QuitGame()
    {
        CloseEverything();
        SceneManager.LoadScene("Menu");
    }

    void CloseEverything()
    {
        endGameMenuUI.SetActive(false);
        GameHasEnded = false;
        BoardManager.isDraw = false;
        youWin.gameObject.SetActive(false);
        youLose.gameObject.SetActive(false);
        draw.gameObject.SetActive(false);
        PauseMenu.GameIsPaused = false;
        Destroy(BoardManager.Instance);
        Time.timeScale = 1f;
    }
}
