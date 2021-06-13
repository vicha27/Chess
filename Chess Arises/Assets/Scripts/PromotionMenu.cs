using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PromotionMenu : MonoBehaviour
{
    public static bool PawnIsPromoted = false;

    public static int choice = -1;

    public GameObject promotionMenuUI;

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
    }
    void Pause()
    {
        promotionMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Queen()
    {
        choice = 1;
        Resume();
    }

    public void Rook()
    {
        choice = 2;
        Resume();
    }

    public void Bishop()
    {
        choice = 3;
        Resume();
    }

    public void Knight()
    {
        choice = 4;
        Resume();
    }

}
