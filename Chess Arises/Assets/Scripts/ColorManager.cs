using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class ColorManager : MonoBehaviour
{
    [SerializeField]
    private ProfileSettings m_profiles;
    [SerializeField]
    public GameObject player1Pawn;
    [SerializeField]
    public GameObject player2Pawn;
    [SerializeField] public TMP_Text Player1Turn;
    [SerializeField] public TMP_Text Player2Turn;

    public int player = 0;
    public Color playerColor = new Color(0, 0, 0);

    //Use this for initialization
    void Awake()
    {
        if (m_profiles != null)
        {
            m_profiles.SetProfile(m_profiles);
        }
    }

    void Start()
    {
        if (Settings.profile)
        {
            CancelChanges();
            Color playerColor = new Color(0, 0, 0);
            playerColor = Settings.profile.GetColorSettings(0);
            Player1Turn.faceColor = playerColor;
            playerColor = Settings.profile.GetColorSettings(1);
            Player2Turn.faceColor = playerColor;
        }
    }

    public void ApplyChanges()
    {
        if (Settings.profile)
        {
            playerColor = ColorPicker.SaveColorChoice();
            Settings.profile.SaveColorSettings(playerColor, player);
            if(player == 0)
                Player1Turn.faceColor = playerColor;
            else
                Player2Turn.faceColor = playerColor;
            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        }
    }//end of ApplyChanges

    public void CancelChanges()
    {
        if (Settings.profile)
        {
            playerColor = Settings.profile.GetColorSettings(player);
            ColorPicker.SetColorChoice(playerColor);
        }
    }//end of CancelChanges

    public void player1()
    {
        player = 0;
        player1Pawn.SetActive(true);
        player2Pawn.SetActive(false);
        CancelChanges();
    }

    public void player2()
    {
        player = 1;
        player1Pawn.SetActive(false);
        player2Pawn.SetActive(true);
        CancelChanges();
    }
}//end of ColorManager
