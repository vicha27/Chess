using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPromotionMenu : MonoBehaviour
{
    public static bool PawnIsPromoted = false;

    public static int choice = -1;

    public GameObject promotionMenuUI;
    public GameObject Player1Turn;
    public GameObject Player2Turn;
    public static bool promotionDone = false;

    // Update is called once per frame
    void Update()
    {
        if (PawnIsPromoted)
        {
            Pause();
        }
        if (promotionDone)
        {
            promotionMenuUI.SetActive(false);
        }
    }

    public static void Resume()
    {
        TutorialManager.Instance.SpawnChessFigure(choice, TutorialManager.CurrentXLocation, TutorialManager.CurrentYLocation); // User chooses which piece to promote Pawn to
        //Debug.Log("Figure should have spawned at x - " + BoardManager.CurrentXLocation + ", y - " + BoardManager.CurrentYLocation);
        Time.timeScale = 1f;
        PawnIsPromoted = false;
        choice = -1;
        promotionDone = true;
        //Player1Turn.SetActive(true);
        //Player2Turn.SetActive(true);
    }
    void Pause()
    {
        promotionMenuUI.SetActive(true);
        Time.timeScale = 0.5f;
        //Player1Turn.SetActive(false);
        //Player2Turn.SetActive(false);
    }

    public void Queen()
    {
        if (!TutorialManager.isWhiteTurn)
            choice = 1;
        else
            choice = 7;
        Resume();
    }

    public void Rook()
    {
        if (!TutorialManager.isWhiteTurn)
            choice = 2;
        else
            choice = 8;
        Resume();
    }

    public void Bishop()
    {
        if (!TutorialManager.isWhiteTurn)
            choice = 3;
        else
            choice = 9;
        Resume();
    }

    public void Knight()
    {
        if (!TutorialManager.isWhiteTurn)
            choice = 4;
        else
            choice = 10;
        Resume();
    }

}
