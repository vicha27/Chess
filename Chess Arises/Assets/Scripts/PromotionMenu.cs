using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PromotionMenu : MonoBehaviour
{
    public static bool PawnIsPromoted = false;

    public static int choice = -1;

    public GameObject promotionMenuUI;
    public GameObject Player1Turn;
    public GameObject Player2Turn;

    // Update is called once per frame
    void Update()
    {
        if (PawnIsPromoted)
        {
            Pause();
        }
    }

    public void Resume()
    {
        BoardManager.Instance.SpawnChessFigure(choice, BoardManager.CurrentXLocation, BoardManager.CurrentYLocation); // User chooses which piece to promote Pawn to
        //Debug.Log("Figure should have spawned at x - " + BoardManager.CurrentXLocation + ", y - " + BoardManager.CurrentYLocation);
        promotionMenuUI.SetActive(false);
        Time.timeScale = 1f;
        PawnIsPromoted = false;
        choice = -1;
        Player1Turn.SetActive(true);
        Player2Turn.SetActive(true);
    }
    void Pause()
    {
        promotionMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Player1Turn.SetActive(false);
        Player2Turn.SetActive(false);
    }

    public void Queen()
    {
        if (!BoardManager.isWhiteTurn)
            choice = 1;
        else
            choice = 7;
        Resume();
    }

    public void Rook()
    {
        if (!BoardManager.isWhiteTurn)
            choice = 2;
        else
            choice = 8;
        Resume();
    }

    public void Bishop()
    {
        if (!BoardManager.isWhiteTurn)
            choice = 3;
        else
            choice = 9;
        Resume();
    }

    public void Knight()
    {
        if (!BoardManager.isWhiteTurn)
            choice = 4;
        else
            choice = 10;
        Resume();
    }

}
