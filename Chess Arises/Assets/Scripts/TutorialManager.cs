using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    //Actual Tutorial Manager Instance
    public static TutorialManager Instance { get; set; }
    //Moves the pieces are allowed, determined by Individual Figure Classes
    private bool[,] allowedMoves { get; set; }
    //The positions of the figures are noted
    public TutorialChessFigure[,] ChessFigurePositions { get; set; }
    //The figure the user selects to move
    private TutorialChessFigure selectedFigure;
    //SelectionX and Y are used for the mouse selection
    private int selectionX = -1;
    private int selectionY = -1;
    //List of all chess figures, active ones, white ones, and black ones //6/9/2021 - adding dead ones to see if it looks cool
    public List<GameObject> chessFigures;
    private List<GameObject> activeFigures = new List<GameObject>();
    private List<GameObject> whiteFigures = new List<GameObject>();
    private List<GameObject> blackFigures = new List<GameObject>();
    private List<GameObject> deadWhiteFigures = new List<GameObject>();
    private List<GameObject> deadBlackFigures = new List<GameObject>();
    private List<int> defenderXLocations = new List<int>();
    private List<int> defenderYLocations = new List<int>();
    //the ai itself. Initiated on Start()
    private ChessAI ai;
    //if isWhiteTurn, move White pieces
    public static bool isWhiteTurn = true;
    //if WhiteWon, do white shit
    public static bool WhiteWon = false;
    //Audio Sources for victory and defeat
    public AudioSource victory;
    public AudioSource defeat;
    //Rotates board regardless
    Animator rotateBoard;
    //TimeSpan of the game
    public float timeSpan = 0.3f;
    private Transform cameraTransform;
    //Used for the promotion menu because I needed a public static int
    public static int CurrentXLocation = 0;
    public static int CurrentYLocation = 0;

    //User Input - used for all player actions
    [Header("Input Settings")]
    public static PlayerInput playerInput;
    [SerializeField] public GameObject CheckScreen;
    [SerializeField] public TMP_Text Player1Turn;
    [SerializeField] public TMP_Text Player2Turn;
    [SerializeField] public TMP_Text Moves;
    [SerializeField] private ProfileSettings m_profiles;
    [SerializeField] public GameObject popUpObject;
    [SerializeField] public GameObject nextButton;
    [SerializeField] public GameObject exitButton;
    [SerializeField] public TMP_Text waitingText;

    //Current Control Scheme
    private string currentControlScheme;

    //Button Booleans - prolly could have handled this better, but it works for now
    public static bool zoomingIn = false;
    public static bool zoomingOut = false;
    public static bool rotatingLeft = false;
    public static bool rotatingRight = false;
    public static bool mouseClicked = false;
    public static bool inCheck = false;
    public static bool isDraw = false;
    public static bool activeMouse = false;

    //Adding variables from YouTube - Epitome to make the game look smoother
    //[SerializeField] private float deathSize = 0.1f;
    [SerializeField] private float deathSpacing = 0.2f;
    [SerializeField] private const float TILE_SIZE = 1.0f;
    [SerializeField] private const float TILE_OFFSET = 0.5f;
    //[SerializeField] private Material tileMaterial;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float dragOffset = 1f;
    private Vector3 bounds;
    public static List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    public string moveListString = "";

    //Tutorial Section
    public GameObject[] popUps;
    private int popUpIndex = 0;
    private bool pawnDemoRunning = false;
    private bool rookDemoRunning = false;
    private bool bishopDemoRunning = false;
    private bool knightDemoRunning = false;
    private bool queenDemoRunning = false;
    private bool kingDemoRunning = false;
    private bool checkDemoRunning = false;
    private bool castleDemoRunning = false;
    private bool previousDemoDone = false;
    private float runTime = 0.0f;
    private float timer = 0.0f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        //playerInput.actions["GameLeftClick"].performed += ctx => MouseClickedTrue();
        //playerInput.actions["Zoom In"].started += ctx => zoomingInTrue();
        //playerInput.actions["Zoom In"].canceled += ctx => zoomingInFalse();
        //playerInput.actions["Zoom Out"].started += ctx => zoomingOutTrue();
        //playerInput.actions["Zoom Out"].canceled += ctx => zoomingOutFalse();
        //playerInput.actions["Rotate Left"].started += ctx => rotatingLeftTrue();
        //playerInput.actions["Rotate Left"].canceled += ctx => rotatingLeftFalse();
        //playerInput.actions["Rotate Right"].started += ctx => rotatingRightTrue();
        //playerInput.actions["Rotate Right"].canceled += ctx => rotatingRightFalse();

        if (m_profiles != null)
        {
            m_profiles.SetProfile(m_profiles);
        }
    }

    void Start()
    {
        Instance = this;

        //ai = new ChessAI();
        ai = gameObject.AddComponent<ChessAI>();
        cameraTransform = Camera.main.transform;

        ChessFigurePositions = new TutorialChessFigure[8, 8];
        SpawnAllChessFigures();
        rotateBoard = gameObject.GetComponent<Animator>();
        if (Settings.profile)
        {
            Color playerColor = new Color(0, 0, 0);
            playerColor = Settings.profile.GetColorSettings(0);
            Player1Turn.faceColor = playerColor;
            playerColor = Settings.profile.GetColorSettings(1);
            Player2Turn.faceColor = playerColor;
        }

    }

    void Update()
    {
        try
        {
            if (!PauseMenu.GameIsPaused)
            {
                DrawChessBoard();
                //UpdateSelection();
                timer += Time.deltaTime;
                for (int i = 0; i < popUps.Length; i++)
                {
                    if(i == popUpIndex)
                    {
                        popUps[i].SetActive(true);
                    }
                    else
                    {
                        popUps[i].SetActive(false);
                    }
                }//end active for loop
                if (popUpIndex == 1 || popUpIndex == 2 || popUpIndex == 3)
                {
                    
                    if (!pawnDemoRunning)
                    {
                        pawnDemoRunning = true;
                        timer = 0.0f;
                        StartCoroutine(pawnDemo());
                    }
                }
                if(popUpIndex == 4 || popUpIndex == 7 || popUpIndex == 9 || popUpIndex == 11 || popUpIndex == 13 || popUpIndex == 15 || popUpIndex == 19)
                {
                    if (previousDemoDone)
                    {
                        nextButton.SetActive(true);
                        waitingText.gameObject.SetActive(false);
                    }
                    else
                    {
                        float timeDif = runTime - timer;
                        nextButton.SetActive(false);
                        waitingText.gameObject.SetActive(true);
                        waitingText.text = "(Click NEXT in " + Math.Round(timeDif) + " seconds)";
                    }
                }
                if(popUpIndex == 5 || popUpIndex == 6)
                {
                    if (!rookDemoRunning && previousDemoDone)
                    {
                        rookDemoRunning = true;
                        timer = 0.0f;
                        StartCoroutine(rookDemo());
                    }
                }
                if (popUpIndex == 8)
                {
                    if (!bishopDemoRunning && previousDemoDone)
                    {
                        bishopDemoRunning = true;
                        timer = 0.0f;
                        StartCoroutine(bishopDemo());
                    }
                }
                if (popUpIndex == 10)
                {
                    if (!knightDemoRunning && previousDemoDone)
                    {
                        knightDemoRunning = true;
                        timer = 0.0f;
                        StartCoroutine(knightDemo());
                    }
                }
                if (popUpIndex == 12)
                {
                    if (!queenDemoRunning && previousDemoDone)
                    {
                        queenDemoRunning = true;
                        timer = 0.0f;
                        StartCoroutine(queenDemo());
                    }
                }
                if (popUpIndex == 14)
                {
                    if (!kingDemoRunning && previousDemoDone)
                    {
                        kingDemoRunning = true;
                        timer = 0.0f;
                        StartCoroutine(kingDemo());
                    }
                }
                if (popUpIndex == 16 || popUpIndex == 17 || popUpIndex == 18)
                {
                    if (!checkDemoRunning && previousDemoDone)
                    {
                        checkDemoRunning = true;
                        timer = 0.0f;
                        StartCoroutine(checkDemo());
                    }
                }//checkDemoRunning
                if (popUpIndex == 20 || popUpIndex == 21 || popUpIndex == 22)
                {
                    if (!castleDemoRunning && previousDemoDone)
                    {
                        castleDemoRunning = true;
                        timer = 0.0f;
                        StartCoroutine(castleDemo());
                    }
                }//castleDemoRunning
                if (popUpIndex == 23)
                {
                    nextButton.SetActive(false);
                    if (previousDemoDone)
                    {
                        exitButton.SetActive(true);
                        waitingText.gameObject.SetActive(false);
                    }
                    else
                    {
                        float timeDif = runTime - timer;
                        exitButton.SetActive(false);
                        waitingText.gameObject.SetActive(true);
                        waitingText.text = "(Click NEXT in " + Math.Round(timeDif) + " seconds)";
                    }
                }
            }//end of if !paused
        }//end of try catch
        catch (Exception e)
        {
            Debug.Log(e + " is the error");
            //throw;
        }
    }

    public void nextPopUp()
    {
        popUpIndex = popUpIndex + 1;
    }
    IEnumerator pawnDemo()
    {
        previousDemoDone = false;
        int waitTime = 1;
        runTime = 19f;
        //select pawn at B2
        SelectChessFigure(1, 1); 

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Pawn Forward 2 spaces
        MoveChessFigure(1, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Enemy Pawn move forward
        SelectChessFigure(2, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Pawn Forward 2 spaces
        MoveChessFigure(2, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select pawn at B4
        SelectChessFigure(1, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Attack Enemy Pawn
        MoveChessFigure(2, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Enemy Pawn move forward
        SelectChessFigure(1, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Pawn Forward 2 spaces
        MoveChessFigure(1, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        
        //select pawn at C5
        SelectChessFigure(2, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Attack Enemy Pawn
        MoveChessFigure(1, 5);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select pawn at A7
        SelectChessFigure(0, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Enemy Pawn Forward 
        MoveChessFigure(0, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select pawn at B6
        SelectChessFigure(1, 5);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Pawn Forward 
        MoveChessFigure(1, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select pawn at A5
        SelectChessFigure(0, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Enemy Pawn Forward 
        MoveChessFigure(0, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select pawn at B7
        SelectChessFigure(1, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Pawn Forward 
        MoveChessFigure(0, 7);
        //Player 1 Pawn should now be promoted

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Open Promotion Menu and choose a Queen
        TutorialPromotionMenu.choice = 1;

        //Wait for waitTime
        //yield return new WaitForSeconds(waitTime);

        //Spawn the Queen
        TutorialPromotionMenu.Resume();

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Reset the board and start again 
        activeFigures.Remove(ChessFigurePositions[0, 7].gameObject);
        Destroy(ChessFigurePositions[0, 7].gameObject);
        ChessFigurePositions[0, 7] = null;
        TutorialPromotionMenu.promotionDone = false;
        ResetBoard();
        pawnDemoRunning = false;
        previousDemoDone = true;
    }//end of Pawn Demo

    IEnumerator rookDemo()
    {
        previousDemoDone = false;
        int waitTime = 1;
        runTime = 8f;
        SpawnChessFigure(2, 3, 4); // Spawn a Rook in the middle of the board

        //select Rook at 3,4
        SelectChessFigure(3, 4);
        
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        
        //Move Rook far left
        MoveChessFigure(0, 4);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        isWhiteTurn = true;

        //select Rook at 0,4
        SelectChessFigure(0, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Rook far right
        MoveChessFigure(7, 4);
        isWhiteTurn = true;
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        activeFigures.Remove(ChessFigurePositions[7, 0].gameObject);
        activeFigures.Remove(ChessFigurePositions[7, 1].gameObject);
        activeFigures.Remove(ChessFigurePositions[7, 6].gameObject);
        activeFigures.Remove(ChessFigurePositions[7, 7].gameObject);
        Destroy(ChessFigurePositions[7,0].gameObject);
        Destroy(ChessFigurePositions[7,1].gameObject);
        Destroy(ChessFigurePositions[7,6].gameObject);
        Destroy(ChessFigurePositions[7,7].gameObject);
        ChessFigurePositions[7, 0] = null;
        ChessFigurePositions[7, 1] = null;
        ChessFigurePositions[7, 6] = null;
        ChessFigurePositions[7, 7] = null;

        //select Rook at 7,4
        SelectChessFigure(7, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Rook straight down
        MoveChessFigure(7, 0);
        isWhiteTurn = true;

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select Rook at 7,0
        SelectChessFigure(7, 0);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Rook straight up
        MoveChessFigure(7, 7);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        ResetBoard();
        rookDemoRunning = false;
        previousDemoDone = true;
    }

    IEnumerator bishopDemo()
    {
        previousDemoDone = false;
        int waitTime = 1;
        runTime = 8f;
        SpawnChessFigure(3, 3, 4); // Spawn a Bishop in the middle of the board

        //select Bishop at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Bishop Diagonal left
        MoveChessFigure(1, 6);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        isWhiteTurn = true;

        //select Bishop at 1,6
        SelectChessFigure(1, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Bishop Diagonal Right
        MoveChessFigure(3, 4);
        isWhiteTurn = true;
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select Bishop at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Diagonal Right again
        MoveChessFigure(5, 6);
        isWhiteTurn = true;

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select Bishop at 5,6
        SelectChessFigure(5,6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move back Diagonal Left
        MoveChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        ResetBoard();
        bishopDemoRunning = false;
        previousDemoDone = true;
    }

    IEnumerator knightDemo()
    {
        previousDemoDone = false;
        int waitTime = 1;
        runTime = 8f;
        SpawnChessFigure(4, 3, 4); // Spawn a Knight in the middle of the board

        //select Knight at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Knight Left
        MoveChessFigure(2, 6);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        isWhiteTurn = true;

        //select Knight at 1,6
        SelectChessFigure(2, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Knight Back
        MoveChessFigure(3, 4);
        isWhiteTurn = true;
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select Knight at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Diagonal Right again
        MoveChessFigure(4, 6);
        isWhiteTurn = true;

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select Knight at 5,6
        SelectChessFigure(4, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move back Diagonal Left
        MoveChessFigure(3, 4);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        ResetBoard();
        knightDemoRunning = false;
        previousDemoDone = true;
    }

    IEnumerator queenDemo()
    {
        previousDemoDone = false;
        int waitTime = 1;
        runTime = 8f;
        SpawnChessFigure(1, 3, 4); // Spawn a Queen in the middle of the board

        //select Queen at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Queen Left
        MoveChessFigure(1, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        isWhiteTurn = true;

        //select Queen at 1,6
        SelectChessFigure(1, 6);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Queen Back
        MoveChessFigure(3, 4);
        isWhiteTurn = true;
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select Queen at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move straight right
        MoveChessFigure(7, 4);
        isWhiteTurn = true;

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select Queen at 7,4
        SelectChessFigure(7,4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move back Diagonal Left
        MoveChessFigure(5, 2);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        ResetBoard();
        queenDemoRunning = false;
        previousDemoDone = true;
    }

    IEnumerator kingDemo()
    {
        previousDemoDone = false;
        int waitTime = 1;
        runTime = 8f;
        SpawnChessFigure(0, 3, 4); // Spawn a King in the middle of the board

        //select King at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move King Left
        MoveChessFigure(2, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        isWhiteTurn = true;

        //select King at 2,3
        SelectChessFigure(2, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move King Back
        MoveChessFigure(3, 4);
        isWhiteTurn = true;
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select King at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move right
        MoveChessFigure(4, 4);
        isWhiteTurn = true;

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select King at 4,4
        SelectChessFigure(4, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move down right
        MoveChessFigure(5, 3);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        ResetBoard();
        kingDemoRunning = false;
        previousDemoDone = true;
    }

    IEnumerator checkDemo()
    {
        previousDemoDone = false;
        int waitTime = 1;
        runTime = 12f;
        SpawnChessFigure(0, 3, 4); // Spawn a King in the middle of the board

        //select King at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move King Left
        MoveChessFigure(2, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        SpawnChessFigure(7, 0, 4); // Spawn a Queen in the middle of the board
        //select Enemy Queen at 0,4
        SelectChessFigure(0, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Enemy Queen to put King in Check
        MoveChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select King at 2,3
        SelectChessFigure(2, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Away from Queen
        MoveChessFigure(1, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select Enemy Queen at 3,4
        SelectChessFigure(3, 4);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move down 
        MoveChessFigure(3, 3);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //select King at 1,3
        SelectChessFigure(1, 3);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move King Away
        MoveChessFigure(0, 4);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        SpawnChessFigure(7, 2, 5); // Spawn another enemy Queen above King

        //select Enemy Queen at 1,5
        SelectChessFigure(2,5);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Move Queen
        MoveChessFigure(0, 5);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        ResetBoard();
        checkDemoRunning = false;
        previousDemoDone = true;
    }

    IEnumerator castleDemo()
    {
        previousDemoDone = false;
        int waitTime = 1;
        runTime = 4f;
        //Now Delete the Pieces in the way of the king and the rook
        activeFigures.Remove(ChessFigurePositions[1, 0].gameObject); //Delete Knight
        activeFigures.Remove(ChessFigurePositions[2, 0].gameObject); //Delete Bishop
        activeFigures.Remove(ChessFigurePositions[3, 0].gameObject); //Delete Queen
        activeFigures.Remove(ChessFigurePositions[5, 0].gameObject); //Delete Bishop
        activeFigures.Remove(ChessFigurePositions[6, 0].gameObject); //Delete Knight
        Destroy(ChessFigurePositions[1, 0].gameObject);
        Destroy(ChessFigurePositions[2, 0].gameObject);
        Destroy(ChessFigurePositions[3, 0].gameObject);
        Destroy(ChessFigurePositions[5, 0].gameObject);
        Destroy(ChessFigurePositions[6, 0].gameObject);
        ChessFigurePositions[1, 0] = null;
        ChessFigurePositions[2, 0] = null;
        ChessFigurePositions[3, 0] = null;
        ChessFigurePositions[5, 0] = null;
        ChessFigurePositions[6, 0] = null;

        //select King at 4,0
        SelectChessFigure(4, 0);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Castle Short
        MoveChessFigure(6, 0);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);
        isWhiteTurn = true;

        SpawnChessFigure(0, 4, 0); // Spawn a King in the middle of the board

        //select King at 4,0
        SelectChessFigure(4, 0);

        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        //Castle Short
        MoveChessFigure(2, 0);
        //Wait for waitTime
        yield return new WaitForSeconds(waitTime);

        ResetBoard();
        castleDemoRunning = false;
        previousDemoDone = true;
    }


    private void ResetBoard()
    {
        BoardHighlighting.Instance.HideHighlights();
        selectedFigure = null;
        for (int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if(ChessFigurePositions[i,j] != null)
                {
                    Destroy(ChessFigurePositions[i, j].gameObject); //Destroy everything
                }//end if
            }//end inner for loop
        }//end outer for loop
        for(int a = 0; a < deadBlackFigures.Count; a++)
            Destroy(deadBlackFigures[a].gameObject);
        deadBlackFigures.Clear();
        ChessFigurePositions = new TutorialChessFigure[8, 8];
        SpawnAllChessFigures();
        isWhiteTurn = true;
    }

    private void SelectChessFigure(int x, int y)
    {
        try
        {
            if (ChessFigurePositions[x, y] == null) return;
            if (ChessFigurePositions[x, y].isWhite != isWhiteTurn) return;

            bool hasAtLeastOneMove = false;
            allowedMoves = ChessFigurePositions[x, y].PossibleMove(ChessFigurePositions);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (allowedMoves[i, j])
                    {
                        hasAtLeastOneMove = true;
                        // break outer loop
                        i = 7;

                        // break inner loop
                        break;
                    }
                }
            }

            if (!hasAtLeastOneMove)
            {
                return;
            }//if there are no moves available
            selectedFigure = ChessFigurePositions[x, y];
            mouseClicked = false;
            CurrentXLocation = x;
            CurrentYLocation = y;
            //PreventCheck();
            BoardHighlighting.Instance.HighlightAllowedMoves(allowedMoves);
        }
        catch (Exception e)
        {
            Debug.Log(e + " is the error");
            //throw;
        }
    }

    private void MoveChessFigure(int x, int y)
    {
        try
        {
            if (allowedMoves[x, y])
            {
                TutorialChessFigure c = ChessFigurePositions[x, y];
                ChessFigurePositions[CurrentXLocation, CurrentYLocation] = null;
                //Debug.Log("Trying to move x - " + x + ", y - " + y);
                selectedFigure.SetPosition(GetTileCenter(x, y), false);
                ChessFigurePositions[x, y] = selectedFigure;
                if (selectedFigure.GetType() == typeof(TutorialPawn) && selectedFigure.isWhite && y == 7)
                { //In this instance, Pawn will get promoted instantly if it kills, otherwise it has to confirm movement.
                    if ((c == null) || (c != null && c.GetType() != typeof(TutorialKing)))
                    {
                        Time.timeScale = 0f;
                        CurrentXLocation = x;
                        CurrentYLocation = y;
                        TutorialPromotionMenu.PawnIsPromoted = true;
                        activeFigures.Remove(selectedFigure.gameObject);
                        whiteFigures.Remove(selectedFigure.gameObject);
                        Destroy(selectedFigure.gameObject);
                    }
                }
                if (selectedFigure.GetType() == typeof(TutorialPawn) && !selectedFigure.isWhite && y == 0)
                { //In this instance, AI Pawn auto spawns a Queen
                    if ((c == null) || (c != null && c.GetType() != typeof(TutorialKing)))
                    {
                        //SpawnChessFigure(7, x, y); // AI will spawn Queen by default
                        Time.timeScale = 0f;
                        CurrentXLocation = x;
                        CurrentYLocation = y;
                        TutorialPromotionMenu.PawnIsPromoted = true;
                        activeFigures.Remove(selectedFigure.gameObject);
                        blackFigures.Remove(selectedFigure.gameObject);
                        Destroy(selectedFigure.gameObject);
                    }
                }
                if (selectedFigure.GetType() == typeof(TutorialPawn) && y == 5 && Math.Abs(x - CurrentXLocation) == 1 && selectedFigure.isWhite)
                { // En Passant White
                    TutorialChessFigure c2 = ChessFigurePositions[x, y - 1]; //check the figure right below the move
                    Vector2Int[] lastMove = moveList[moveList.Count - 1];//check that the last move was on the same x axis
                    if (c2 != null && c2.GetType() == typeof(TutorialPawn) && !c2.isWhite && lastMove[1].x == x && lastMove[1].y == y - 1) //don't be null, be a pawn, don't be white, last move is the same x axis we just moved to and last move y is right below us
                    {// if all these conditions meet, kill the pawn
                        deadBlackFigures.Add(c2.gameObject);
                        blackFigures.Remove(c2.gameObject);
                        Vector3 blackDeath = new Vector3(3.5f * TILE_SIZE, 0, 13f * TILE_SIZE)
                            - bounds
                            + (Vector3.back * deathSpacing) * deadBlackFigures.Count * 3f;
                        //Debug.Log(blackDeath + " is the black death vector"); //great for debugging the death locations
                        c2.SetPosition(blackDeath);
                        activeFigures.Remove(c2.gameObject);
                        ChessFigurePositions[x, y - 1] = null;
                    }
                }
                if (selectedFigure.GetType() == typeof(TutorialPawn) && y == 2 && Math.Abs(x - CurrentXLocation) == 1 && !selectedFigure.isWhite)
                { // En Passant Black
                    TutorialChessFigure c2 = ChessFigurePositions[x, y + 1]; //check the figure right above the move
                    Vector2Int[] lastMove = moveList[moveList.Count - 1];//check that the last move was on the same x axis
                    if (c2 != null && c2.GetType() == typeof(TutorialPawn) && c2.isWhite && lastMove[1].x == x && lastMove[1].y == y + 1) //don't be null, be a pawn, be white, last move is the same x axis we just moved to and last move y is right above us
                    {// if all these conditions meet, kill the pawn
                        deadWhiteFigures.Add(c2.gameObject);
                        whiteFigures.Remove(c2.gameObject);
                        Vector3 whiteDeath = new Vector3(12.5f * TILE_SIZE, 0, 3.5f * TILE_SIZE)
                            - bounds
                            + (Vector3.forward * deathSpacing) * deadWhiteFigures.Count * 3f;
                        //Debug.Log(whiteDeath + " is the white death vector"); //great for debugging the death locations
                        c2.SetPosition(whiteDeath);
                        activeFigures.Remove(c2.gameObject);
                        ChessFigurePositions[x, y + 1] = null;
                    }
                }

                if (selectedFigure.GetType() == typeof(TutorialKing) && Math.Abs(x - CurrentXLocation) > 1)
                {// King is Castling
                    if ((x - CurrentXLocation) == 2)
                    {// King Castling Short
                        TutorialChessFigure c2 = ChessFigurePositions[x + 1, y];
                        //c2.transform.position = GetTileCenter(x - 1, y);
                        c2.SetPosition(GetTileCenter(x - 1, y), false);
                        ChessFigurePositions[x - 1, y] = c2;
                        ChessFigurePositions[x + 1, y] = null;
                    }
                    else
                    {// King Castling Long
                        TutorialChessFigure c2 = ChessFigurePositions[x - 2, y];
                        //c2.transform.position = GetTileCenter(x + 1, y);
                        c2.SetPosition(GetTileCenter(x + 1, y), false);
                        ChessFigurePositions[x + 1, y] = c2;
                        ChessFigurePositions[x - 2, y] = null;
                    }
                }
                if (c != null && c.isWhite != isWhiteTurn)
                {//Taking out game pieces
                    activeFigures.Remove(c.gameObject);
                    if (c.isWhite)
                    {
                        deadWhiteFigures.Add(c.gameObject);
                        whiteFigures.Remove(c.gameObject);
                        Vector3 whiteDeath = new Vector3(12.5f * TILE_SIZE, 0, 3.5f * TILE_SIZE)
                            - bounds
                            + (Vector3.forward * deathSpacing) * deadWhiteFigures.Count * 3f;
                        //Debug.Log(whiteDeath + " is the white death vector"); //great for debugging the death locations
                        c.SetPosition(whiteDeath);
                    }
                    else
                    {
                        deadBlackFigures.Add(c.gameObject);
                        blackFigures.Remove(c.gameObject);
                        Vector3 blackDeath = new Vector3(3.5f * TILE_SIZE, 0, 13f * TILE_SIZE)
                            - bounds
                            + (Vector3.back * deathSpacing) * deadBlackFigures.Count * 3f;
                        //Debug.Log(blackDeath + " is the black death vector"); //great for debugging the death locations
                        c.SetPosition(blackDeath);
                    }
                    //Destroy(c.gameObject);

                    if (c.GetType() == typeof(TutorialKing))
                    {
                        EndGame(); //comment out to do endless testing
                        return;
                    }
                }
                moveList.Add(new Vector2Int[] { new Vector2Int(CurrentXLocation, CurrentYLocation), new Vector2Int(x, y) });
                UpdateMoveList(); //moveList
                isWhiteTurn = !isWhiteTurn;
                selectedFigure.hasMoved = true;
                defenderXLocations.Clear();
                defenderYLocations.Clear();
            }//end of if allowed moves
            //else
            //{
            //    //if (isWhiteTurn)
            //    //{
            //        selectedFigure.SetPosition(GetTileCenter(CurrentXLocation, CurrentYLocation));
            //        ChessFigurePositions[selectedFigure.CurrentX, selectedFigure.CurrentY] = selectedFigure;
            //        //selectedFigure.transform.position = GetTileCenter(x, y);
            //        //Debug.Log(" I have returned home");
            //    //}
            //}
        }//end of try
        catch (Exception e)
        {
            Debug.Log(e + " is the error");
            //selectedFigure.SetPosition(GetTileCenter(CurrentXLocation, CurrentYLocation));
            throw;
        }

        BoardHighlighting.Instance.HideHighlights();
        selectedFigure = null;
        mouseClicked = false;
    }

    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;
        bounds = new Vector3(4 * TILE_SIZE, 0, 4 * TILE_SIZE) + boardCenter;

        // Draw Chessboard
        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        // Draw Selection
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * (selectionX + 1),
               Vector3.forward * (selectionY + 1) + Vector3.right * selectionX);
        }
    }

    private void UpdateSelection()
    {
        if (!Camera.main) return;

        RaycastHit hit;
        Vector3 newMouseInput = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(newMouseInput);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
        if (selectedFigure != null)//&& isWhiteTurn
        {
            //attempt 1 at "dragging" pieces
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * TILE_OFFSET);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
                selectedFigure.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
        }//end if selectedFigure != null
    }

    public void SpawnChessFigure(int index, int x, int y)
    {
        GameObject go = Instantiate(chessFigures[index], GetTileCenter(x, y), chessFigures[index].transform.rotation) as GameObject;
        go.transform.SetParent(transform);
        ChessFigurePositions[x, y] = go.GetComponent<TutorialChessFigure>();
        ChessFigurePositions[x, y].SetPosition(GetTileCenter(x, y), true);
        activeFigures.Add(go);
        if (index <= 5)
        {
            whiteFigures.Add(go);
        }
        else
        {
            blackFigures.Add(go);
        }
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void SpawnAllChessFigures()
    {
        //Should I add 2 arrays or tuples and just for loop through them.
        //idk if it's more time efficient, but it seems like it would be easier to modify their positions.
        // White (Index of Chess Figures List, x coordinate, y coordinate)
        SpawnChessFigure(0, 4, 0); // King
        SpawnChessFigure(1, 3, 0); // Queen
        SpawnChessFigure(2, 0, 0); // Rook
        SpawnChessFigure(2, 7, 0); // Rook
        SpawnChessFigure(3, 2, 0); // Bishop
        SpawnChessFigure(3, 5, 0); // Bishop
        SpawnChessFigure(4, 1, 0); // Knight
        SpawnChessFigure(4, 6, 0); // Knight
        SpawnChessFigure(5, 0, 1); // Pawn
        SpawnChessFigure(5, 1, 1); // Pawn
        SpawnChessFigure(5, 2, 1); // Pawn
        SpawnChessFigure(5, 3, 1); // Pawn
        SpawnChessFigure(5, 4, 1); // Pawn
        SpawnChessFigure(5, 5, 1); // Pawn
        SpawnChessFigure(5, 6, 1); // Pawn
        SpawnChessFigure(5, 7, 1); // Pawn

        // Black
        SpawnChessFigure(6, 4, 7); // King
        SpawnChessFigure(7, 3, 7); // Queen
        SpawnChessFigure(8, 0, 7); // Rook
        SpawnChessFigure(8, 7, 7); // Rook
        SpawnChessFigure(9, 2, 7); // Bishop
        SpawnChessFigure(9, 5, 7); // Bishop
        SpawnChessFigure(10, 1, 7); // Knight
        SpawnChessFigure(10, 6, 7); // Knight
        SpawnChessFigure(11, 0, 6); // Pawn
        SpawnChessFigure(11, 1, 6); // Pawn
        SpawnChessFigure(11, 2, 6); // Pawn
        SpawnChessFigure(11, 3, 6); // Pawn
        SpawnChessFigure(11, 4, 6); // Pawn
        SpawnChessFigure(11, 5, 6); // Pawn
        SpawnChessFigure(11, 6, 6); // Pawn
        SpawnChessFigure(11, 7, 6); // Pawn
    }

    private void PreventCheck()
    {
        TutorialChessFigure targetKing = null;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (ChessFigurePositions[x, y] != null && ChessFigurePositions[x, y].GetType() == typeof(TutorialKing))
                {
                    if (ChessFigurePositions[x, y].isWhite == selectedFigure.isWhite || !ChessFigurePositions[x, y].isWhite == !selectedFigure.isWhite)
                    {
                        targetKing = ChessFigurePositions[x, y];
                        //Debug.Log("We have found the king, he is at " + x + ", " + y + ". And he is " + ChessFigurePositions[x, y].isWhite);
                    }//end if
                }//end outer if
            }//end for loop
        }//end outer for loop
        SimulateMoveForSinglePiece(selectedFigure, allowedMoves, targetKing);
    }//end PreventCheck

    private void SimulateMoveForSinglePiece(TutorialChessFigure c, bool[,] allowingMoves, TutorialChessFigure targetKing)
    {
        // Save the current values to reset after the function call
        // Go through all the moves and simulate if king is in check
        List<int> xValues = new List<int>();
        List<int> yValues = new List<int>();
        int friendlies = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (allowingMoves[x, y])
                {//needed a list of all allowed moves for Chess Figure c
                    xValues.Add(x);
                    yValues.Add(y);
                    //if (c.GetType() == typeof(TutorialKing))
                    //  Debug.Log("King's allowed move x - " + x + ", y - " + y);
                }
            }
        }
        //Go through the moves one by one and simulate what would happen if Chess Figure c moves to those allowed positions
        for (int i = 0; i < xValues.Count; i++)
        {
            int simX = xValues[i];
            int simY = yValues[i];
            int removeMovesCounter = 0;
            //Debug.Log("First move looking at " + simX + ", " + simY);

            Vector2Int kingPositionThisSim = new Vector2Int(targetKing.CurrentX, targetKing.CurrentY);
            // Did we simulate the king's move?
            if (c.GetType() == typeof(TutorialKing))
                kingPositionThisSim = new Vector2Int(simX, simY);

            //Copy the [,] and not a reference
            TutorialChessFigure[,] simulation = new TutorialChessFigure[8, 8];
            List<TutorialChessFigure> simAttackingPieces = new List<TutorialChessFigure>();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (ChessFigurePositions[x, y] != null) //if there is a figure at the location
                    {
                        simulation[x, y] = ChessFigurePositions[x, y]; //simulation board copies that location
                        if (simulation[x, y].isWhite != c.isWhite) //if the piece located there is an enemy
                        {
                            simAttackingPieces.Add(simulation[x, y]); //add the enemy piece to the list of enemy pieces that will simulate attacking
                        }//end of if simulation piece is an enemy
                        if(simulation[x,y].isWhite == c.isWhite)//if the piece has friends, count them
                        {
                            friendlies = friendlies + 1;
                            //Debug.Log(friendlies + " friendlies remaining");
                        }
                    }//end of if the position is not null
                }//end for loop
            }//end outer for loop

            // Simulate that move
            simulation[CurrentXLocation, CurrentYLocation] = null; //current x and y will become null
            c.CurrentX = simX;
            c.CurrentY = simY;
            simulation[simX, simY] = c; //the selected chess figure will now be simulating moving to the allowed move

            // Did the attack kill an enemy?
            var deadPiece = simAttackingPieces.Find(x => x.CurrentX == simX && x.CurrentY == simY);
            if (deadPiece != null) //if we killed a piece in our simulation, remove that piece from the list of attacking enemies
            {
                simAttackingPieces.Remove(deadPiece);
                //Debug.Log("Dead piece - " + deadPiece + " was removed");
            }

            // Get all the simulated attacking pieces moves
            for (int a = 0; a < simAttackingPieces.Count; a++)
            {
                var pieceMoves = simAttackingPieces[a].PossibleMove(simulation); //get the boolean array [,] of allowed moves for the simulating attack piece, use simulation instead of the actual board to simulate what the board would look like
                for (int d = 0; d < 8; d++)
                {
                    for (int e = 0; e < 8; e++)
                    {
                        if (pieceMoves[d, e] && kingPositionThisSim.x == d && kingPositionThisSim.y == e) //if the move is legal and runs into the king, increment the remove moves counter
                        {
                            removeMovesCounter = removeMovesCounter + 1;
                            //Debug.Log("Theoretically, the enemy killed the king at " + d + ", " + e);
                        }
                    }
                }
            }//end of simAttackingPieces for loop
            if (removeMovesCounter > 0) //if the king died in our simulation, remove the move from allowed moves so the king doesn't die
            {
                allowingMoves[simX, simY] = false;
            }//end else
            // Restore the actual c data
            c.CurrentX = CurrentXLocation;
            c.CurrentY = CurrentYLocation;
        }//end for loop of going through all this mess
        xValues.Clear();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (allowingMoves[x, y])
                {//needed a list of all allowed moves for Chess Figure c
                    xValues.Add(x);
                }
            }
        }
        if (friendlies <= 8 && xValues.Count == 0 && c.GetType() == typeof(TutorialKing) && !inCheck)
        {
            isDraw = true;
        }
    }//end of this long ass method

    private bool CheckForCheckMate()
    {
        //var lastMove = moveList[moveList.Count - 1];//grab the move that just occurred
        bool lastPieceWhite = isWhiteTurn; //could have totally just used "isWhiteTurn", but I'll just leave it for now
        int tempX = CurrentXLocation;
        int tempY = CurrentYLocation;

        List<TutorialChessFigure> attackingPieces = new List<TutorialChessFigure>();
        List<TutorialChessFigure> defendingPieces = new List<TutorialChessFigure>();
        TutorialChessFigure targetKing = null;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (ChessFigurePositions[x, y] != null)
                {
                    if (ChessFigurePositions[x, y].isWhite == lastPieceWhite) //if you are white and the last piece was white
                    {//then you are defending right now
                        defendingPieces.Add(ChessFigurePositions[x, y]);
                        if (ChessFigurePositions[x, y].GetType() == typeof(TutorialKing))
                        {//get the king who is going to be targeted by the enemies
                            targetKing = ChessFigurePositions[x, y];
                        }
                    }
                    else
                    {//get all of the attacking pieces
                        attackingPieces.Add(ChessFigurePositions[x, y]);
                    }
                }//end outer if
            }//end for loop
        }//end outer for loop

        // Is the King attacked right now?
        int attackMovesCounter = 0;
        for (int i = 0; i < attackingPieces.Count; i++)
        {//get all of the attackers configurations of allowed moves
            var pieceMoves = attackingPieces[i].PossibleMove(ChessFigurePositions); //get the boolean array [,] of allowed moves
            for (int d = 0; d < 8; d++)
            {
                for (int e = 0; e < 8; e++)
                {//iterate through allowed moves to see if they hit the targeted king
                    if (pieceMoves[d, e] && targetKing.CurrentX == d && targetKing.CurrentY == e) //if the move is legal and runs into the king, increment the attackMovesCounter
                    {
                        attackMovesCounter = attackMovesCounter + 1;
                    }//end if
                }//end inner for loop
            }//end 2nd inner for loop
        }//end for loop that counts the attacking Pieces

        if (attackMovesCounter > 0)
        {//if the attackers are able to kill the king, he is in Check
            inCheck = true;
            //Debug.Log("The King is in check!");
            int defenderMovesCounter = 0;
            for (int i = 0; i < defendingPieces.Count; i++)
            {//get all of the defender configurations allowed moves
                var pieceMoves = defendingPieces[i].PossibleMove(ChessFigurePositions); //get the boolean array [,] of allowed moves
                CurrentXLocation = defendingPieces[i].CurrentX;
                CurrentYLocation = defendingPieces[i].CurrentY;
                SimulateMoveForSinglePiece(defendingPieces[i], pieceMoves, targetKing);
                for (int d = 0; d < 8; d++)
                {
                    for (int e = 0; e < 8; e++)
                    {
                        if (pieceMoves[d, e] && !defenderXLocations.Contains(d))
                        {
                            //Debug.Log(defendingPieces[i].GetType() + " can move");
                            defenderMovesCounter = defenderMovesCounter + 1;
                            defenderXLocations.Add(defendingPieces[i].CurrentX);
                            defenderYLocations.Add(defendingPieces[i].CurrentY);
                            //Debug.Log("There is a way to save the king at x - " + d + ", y - " + e);
                        }
                    }//end of inner for loop
                }//end of inner for loop
            }//end for loop that counts the defending Pieces
            if (defenderMovesCounter == 0) //if there is no way to save the king, return true - checkmate
            {
                inCheck = true;
                return true;
            }
            //Debug.Log(defenderMovesCounter + " possible moves to save the king");
        }//end if attackMoves can kill king
        else
        {
            inCheck = false;
        }//else the king is not in check
        CurrentXLocation = tempX;
        CurrentYLocation = tempY;
        return false;
    }

    private void EndGame()
    {
        EndGameMenu.GameHasEnded = true;
        rotateBoard.enabled = true;
        BoardHighlighting.Instance.HideHighlights();
        selectedFigure = null;
        inCheck = false;
        if (isDraw) 
        {
            if (AudioManager.muted)
            {
                defeat.Play();
            }
        }
        else
        {
            if (isWhiteTurn)
            {
                WhiteWon = true;
                if (AudioManager.muted)
                {
                    victory.Play();
                }
            }
            else
            {
                WhiteWon = false;
                if (AudioManager.muted)
                {
                    defeat.Play();
                }
            }
        }
        //Reload Scene and Play Again
    }

    public void UpdateMoveList()
    { //moveList
      Vector2Int[] lastMove = moveList[moveList.Count - 1];//grab the last move
      string lastMoveX0 = convertToAlphabet(lastMove[0].x);
      string lastMoveY0 = (lastMove[0].y + 1).ToString();
      string lastMoveX1 = convertToAlphabet(lastMove[1].x);
      string lastMoveY1 = (lastMove[1].y + 1).ToString();
      moveListString = lastMoveX0 + lastMoveY0 + " -> " + lastMoveX1 + lastMoveY1;
      Moves.text = moveListString;
    }
    public string convertToAlphabet(int x)
    {
        switch (x)
        {
            case 0:
                return "A";
            case 1:
                return "B";
            case 2:
                return "C";
            case 3:
                return "D";
            case 4:
                return "E";
            case 5:
                return "F";
            case 6:
                return "G";
            case 7:
                return "H";
        }
        return "";
    }
    public List<GameObject> GetAllActiveFigures()
    {
        return activeFigures;
    }
    public List<GameObject> GetAllWhiteFigures()
    {
        return whiteFigures;
    }
    public List<GameObject> GetAllBlackFigures()
    {
        return blackFigures;
    }

    //public static void MouseClickedTrue()
    //{
    //    mouseClicked = true;
    //}

    public static void Pause()
    {
        PauseMenu.GameIsPaused = !PauseMenu.GameIsPaused;
    }
}