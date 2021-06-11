using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    //Actual Board Manager Instance
    public static BoardManager Instance { get; set; }
    //Moves the pieces are allowed, determined by Individual Figure Classes
    private bool[,] allowedMoves { get; set; }
    //The positions of the figures are noted
    public ChessFigure[,] ChessFigurePositions { get; set; }
    //The figure the user selects to move
    private ChessFigure selectedFigure;
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
    //the ai itself. Initiated on Start()
    private ChessAI ai;
    //if isWhiteTurn, move White pieces
    public bool isWhiteTurn = true;
    //if WhiteWon, do white shit
    public static bool WhiteWon = false;
    //Audio Sources for victory and defeat
    public AudioSource victory;
    public AudioSource defeat;
    //Rotates board regardless
    Animator rotateBoard;
    //TimeSpan of the game
    public float timeSpan = 0.3f;
    private float timer;
    private Transform cameraTransform;
    //Used for the promotion menu because I needed a public static int
    public static int CurrentXLocation = 0;
    public static int CurrentYLocation = 0;

    //User Input - used for all player actions
    [Header("Input Settings")]
    public static PlayerInput playerInput;
    [SerializeField] public GameObject CheckScreen;

    //Action Maps - will eventually use when we need to switch action maps
    //private string actionMapPlayerControls = "CameraControls";
    //private string actionMapMenuControls = "MenuControls";
    //public void SwitchCurrentActionMap(string mapNameOrId)

    //Current Control Scheme
    private string currentControlScheme;
    
    //Button Booleans - prolly could have handled this better, but it works for now
    public static bool zoomingIn = false;
    public static bool zoomingOut = false;
    public static bool rotatingLeft = false;
    public static bool rotatingRight = false;
    public static bool mouseClicked = false;
    public static bool inCheck = false;

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

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["GameLeftClick"].performed += ctx => MouseClickedTrue();
        playerInput.actions["Zoom In"].started += ctx => zoomingInTrue();
        playerInput.actions["Zoom In"].canceled += ctx => zoomingInFalse();
        playerInput.actions["Zoom Out"].started += ctx => zoomingOutTrue();
        playerInput.actions["Zoom Out"].canceled += ctx => zoomingOutFalse();
        playerInput.actions["Rotate Left"].started += ctx => rotatingLeftTrue();
        playerInput.actions["Rotate Left"].canceled += ctx => rotatingLeftFalse();
        playerInput.actions["Rotate Right"].started += ctx => rotatingRightTrue();
        playerInput.actions["Rotate Right"].canceled += ctx => rotatingRightFalse();
    }

    void Start()
    {
        Instance = this;

        //ai = new ChessAI();
        ai = gameObject.AddComponent<ChessAI>();
        cameraTransform = Camera.main.transform;

        ChessFigurePositions = new ChessFigure[8, 8];
        SpawnAllChessFigures();
        rotateBoard = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        try 
        {
        if (!PauseMenu.GameIsPaused)
        {
            DrawChessBoard();
            UpdateSelection();

            if (mouseClicked && !EndGameMenu.GameHasEnded)
            {
                if (selectionX >= 0 && selectionY >= 0)
                {
                    if (selectedFigure == null)
                    {
                        // Select Figure
                        SelectChessFigure(selectionX, selectionY);
                    }
                    else
                    {
                        // Move Figure
                        MoveChessFigure(selectionX, selectionY);
                        if (CheckForCheckMate())
                        {
                            EndGame();
                            return;
                        }
                    }
                }
            }
            if (inCheck)
            {
                //if the king is in check, do something!!!
                //I'm thinking of like a banner and a sound alert that goes
                //KING IS IN CHECK!
                CheckScreen.SetActive(true);
            }
            if(!inCheck)
            {
                CheckScreen.SetActive(false);        
            }

            //Zoom In Logic
            if (zoomingIn)
            {
                OnZoomIn();
            }
            //Zoom Out Logic
            if (zoomingOut)
            {
                OnZoomOut();
            }
            //Rotate Left Logic
            if (rotatingLeft)
            {
                OnRotateLeft();
            }
            //Rotate Right Logic
            if (rotatingRight)
            {
                OnRotateRight();
            }

            // AI should be black player
            if (!isWhiteTurn && PromotionMenu.PawnIsPromoted == false)
            {
                    //Normal AI code
                    //Vector2 aiMove = new Vector2();
                    //int aiX = -1;
                    //int aiY = -1;
                    //while (aiY == -1)
                    //{
                    //    selectedFigure = ai.SelectChessFigure();
                    //    aiMove = ai.MakeMove(selectedFigure);
                    //    aiX = (int)Math.Round(aiMove.x);
                    //    aiY = (int)Math.Round(aiMove.y);
                    //        if (aiY > -1)
                    //    {
                    //        break;
                    //    }
                    //}
                    //allowedMoves = ChessFigurePositions[selectedFigure.CurrentX, selectedFigure.CurrentY].PossibleMove();
                    //CurrentXLocation = selectedFigure.CurrentX;
                    //CurrentYLocation = selectedFigure.CurrentY;
                    //MoveChessFigure(aiX, aiY);

                    //Debug play myself
                    if (mouseClicked && !EndGameMenu.GameHasEnded)
                    {
                        if (selectionX >= 0 && selectionY >= 0)
                        {
                            if (selectedFigure == null)
                            {
                                // Select Figure
                                SelectChessFigure(selectionX, selectionY);
                            }
                            else
                            {
                                // Move Figure
                                MoveChessFigure(selectionX, selectionY);
                                if (CheckForCheckMate())
                                {
                                    EndGame();
                                    return;
                                }
                            }
                        }
                    }
            }//black turn
        }//end of if !paused
        }//end of try catch
        catch (Exception e)
        {
            Debug.Log(e + " is the error");
            //throw;
        }
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

        if (!hasAtLeastOneMove) return;

        selectedFigure = ChessFigurePositions[x, y];
        mouseClicked = false;
        CurrentXLocation = x;
        CurrentYLocation = y;
        PreventCheck();
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
                ChessFigure c = ChessFigurePositions[x, y];
                ChessFigurePositions[CurrentXLocation, CurrentYLocation] = null;
                //selectedFigure.transform.position = GetTileCenter(x, y);
                selectedFigure.SetPosition(GetTileCenter(x, y),false);
                ChessFigurePositions[x, y] = selectedFigure; 
                if (selectedFigure.GetType() == typeof(Pawn) && selectedFigure.isWhite && y == 7)
                { //In this instance, Pawn will get promoted instantly if it kills, otherwise it has to confirm movement.
                    if ((c == null) || (c != null && c.GetType() != typeof(King)))
                    {
                        Time.timeScale = 0f;
                        PromotionMenu.PawnIsPromoted = true;
                        CurrentXLocation = x;
                        CurrentYLocation = y;
                        activeFigures.Remove(selectedFigure.gameObject);
                        whiteFigures.Remove(selectedFigure.gameObject);
                        Destroy(selectedFigure.gameObject);
                    }
                }
                if (selectedFigure.GetType() == typeof(Pawn) && !selectedFigure.isWhite && y == 0)
                { //In this instance, AI Pawn auto spawns a Queen
                    if ((c == null) || (c != null && c.GetType() != typeof(King)))
                    {
                        SpawnChessFigure(7, x, y); // AI will spawn Queen by default
                        activeFigures.Remove(selectedFigure.gameObject);
                        blackFigures.Remove(selectedFigure.gameObject);
                        Destroy(selectedFigure.gameObject);
                    }
                }
                if(selectedFigure.GetType() == typeof(Pawn) && y == 5 && Math.Abs(x-CurrentXLocation) == 1 && selectedFigure.isWhite)
                { // En Passant White
                    ChessFigure c2 = ChessFigurePositions[x, y - 1]; //check the figure right below the move
                    Vector2Int[] lastMove = moveList[moveList.Count - 1];//check that the last move was on the same x axis
                    if (c2 != null && c2.GetType() == typeof(Pawn) && !c2.isWhite && lastMove[1].x == x && lastMove[1].y == y - 1) //don't be null, be a pawn, don't be white, last move is the same x axis we just moved to and last move y is right below us
                    {// if all these conditions meet, kill the pawn
                        deadBlackFigures.Add(c2.gameObject);
                        blackFigures.Remove(c2.gameObject);
                        Vector3 blackDeath = new Vector3(3.5f * TILE_SIZE, 0, 13f * TILE_SIZE)
                            - bounds
                            + (Vector3.back * deathSpacing) * deadBlackFigures.Count * 3f;
                        //Debug.Log(blackDeath + " is the black death vector"); //great for debugging the death locations
                        c2.SetPosition(blackDeath);
                    }
                }
                if (selectedFigure.GetType() == typeof(Pawn) && y == 2 && Math.Abs(x - CurrentXLocation) == 1 && !selectedFigure.isWhite)
                { // En Passant Black
                    ChessFigure c2 = ChessFigurePositions[x, y + 1]; //check the figure right above the move
                    Vector2Int[] lastMove = moveList[moveList.Count - 1];//check that the last move was on the same x axis
                    if (c2 != null && c2.GetType() == typeof(Pawn) && c2.isWhite && lastMove[1].x == x && lastMove[1].y == y + 1) //don't be null, be a pawn, be white, last move is the same x axis we just moved to and last move y is right above us
                    {// if all these conditions meet, kill the pawn
                        deadWhiteFigures.Add(c2.gameObject);
                        whiteFigures.Remove(c2.gameObject);
                        Vector3 whiteDeath = new Vector3(12.5f * TILE_SIZE, 0, 2.7f * TILE_SIZE)
                            - bounds
                            + (Vector3.forward * deathSpacing) * deadWhiteFigures.Count * 2.5f;
                        //Debug.Log(whiteDeath + " is the white death vector"); //great for debugging the death locations
                        c2.SetPosition(whiteDeath);
                    }
                }

                if (selectedFigure.GetType() == typeof(King) && Math.Abs(x - CurrentXLocation) > 1)
                {// King is Castling
                    if((x - CurrentXLocation) == 2)
                    {// King Castling Short
                        ChessFigure c2 = ChessFigurePositions[x + 1, y];
                        //c2.transform.position = GetTileCenter(x - 1, y);
                        c2.SetPosition(GetTileCenter(x - 1, y),false);
                        ChessFigurePositions[x - 1, y] = c2;
                        ChessFigurePositions[x + 1, y] = null;
                    }
                    else 
                    {// King Castling Long
                        ChessFigure c2 = ChessFigurePositions[x - 2, y];
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
                        Vector3 whiteDeath = new Vector3(12.5f * TILE_SIZE, 0, 2.7f * TILE_SIZE)
                            - bounds
                            + (Vector3.forward * deathSpacing) * deadWhiteFigures.Count*2.5f;
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

                    if (c.GetType() == typeof(King))
                    {
                        EndGame(); //comment out to do endless testing
                        return;
                    }
                }
                moveList.Add(new Vector2Int[] { new Vector2Int(CurrentXLocation, CurrentYLocation), new Vector2Int(x, y) } );
                isWhiteTurn = !isWhiteTurn;
                selectedFigure.hasMoved = true;
            }//end of if allowed moves
            else
            {
                if (isWhiteTurn)
                {
                    selectedFigure.SetPosition(GetTileCenter(CurrentXLocation, CurrentYLocation));
                    ChessFigurePositions[selectedFigure.CurrentX, selectedFigure.CurrentY] = selectedFigure;
                    //selectedFigure.transform.position = GetTileCenter(x, y);
                    //Debug.Log(" I have returned home");
                }
            }
        }//end of try
        catch (Exception e)
        {
            Debug.Log(e + " is the error");
            selectedFigure.SetPosition(GetTileCenter(CurrentXLocation, CurrentYLocation));
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
        if(selectedFigure != null && isWhiteTurn)
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
        ChessFigurePositions[x, y] = go.GetComponent<ChessFigure>();
        ChessFigurePositions[x, y].SetPosition(GetTileCenter(x, y),true);
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
        ChessFigure targetKing = null;
        for(int x = 0; x < 8; x++)
        {
            for(int y = 0; y < 8; y++)
            {
                if(ChessFigurePositions[x, y] != null && ChessFigurePositions[x, y].GetType() == typeof(King))
                {
                    if(ChessFigurePositions[x, y].isWhite == selectedFigure.isWhite || !ChessFigurePositions[x, y].isWhite == !selectedFigure.isWhite)
                    {
                        targetKing = ChessFigurePositions[x, y];
                        //Debug.Log("We have found the king, he is at " + x + ", " + y + ". And he is " + ChessFigurePositions[x, y].isWhite);
                    }//end if
                }//end outer if
            }//end for loop
        }//end outer for loop
        SimulateMoveForSinglePiece(selectedFigure, allowedMoves, targetKing);
    }//end PreventCheck

    private void SimulateMoveForSinglePiece(ChessFigure c,bool[,] allowingMoves, ChessFigure targetKing)
    {
        // Save the current values to reset after the function call
        // Go through all the moves and simulate if king is in check
        List<int> xValues = new List<int>();
        List<int> yValues = new List<int>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (allowingMoves[x, y])
                {//needed a list of all allowed moves for Chess Figure c
                    xValues.Add(x);
                    yValues.Add(y);
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
            if (c.GetType() == typeof(King))
                kingPositionThisSim = new Vector2Int(simX, simY);
            
            //Copy the [,] and not a reference
            ChessFigure[,] simulation = new ChessFigure[8, 8];
            List<ChessFigure> simAttackingPieces = new List<ChessFigure>();
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
    }//end of this long ass method

    private bool CheckForCheckMate()
    {
        var lastMove = moveList[moveList.Count - 1];//grab the move that just occurred
        bool lastPieceWhite = isWhiteTurn; //could have totally just used "isWhiteTurn", but I'll just leave it for now

        List<ChessFigure> attackingPieces = new List<ChessFigure>();
        List<ChessFigure> defendingPieces = new List<ChessFigure>();
        ChessFigure targetKing = null;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (ChessFigurePositions[x, y] != null) 
                {
                    if (ChessFigurePositions[x, y].isWhite == lastPieceWhite) //if you are white and the last piece was white
                    {//then you are defending right now
                        defendingPieces.Add(ChessFigurePositions[x, y]);
                        if (ChessFigurePositions[x, y].GetType() == typeof(King))
                        {//get the king who is going to be targeted by the enemies
                            targetKing = ChessFigurePositions[x, y];
                            Debug.Log("Target King located at " + x + ", " + y);
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

        if(attackMovesCounter > 0)
        {//if the attackers are able to kill the king, he is in Check
            inCheck = true;
            Debug.Log("The King is in check!");
            int defenderMovesCounter = 0;
            for (int i = 0; i < defendingPieces.Count; i++)
            {//get all of the defender configurations allowed moves
                var pieceMoves = defendingPieces[i].PossibleMove(ChessFigurePositions); //get the boolean array [,] of allowed moves
                SimulateMoveForSinglePiece(defendingPieces[i], pieceMoves, targetKing);
                for (int d = 0; d < 8; d++)
                {
                    for (int e = 0; e < 8; e++)
                    {
                        if (pieceMoves[d, e])
                        {
                            defenderMovesCounter = defenderMovesCounter + 1;
                            Debug.Log("There is a way to save the king");
                        }
                    }//end of inner for loop
                }//end of inner for loop
            }//end for loop that counts the defending Pieces
            if(defenderMovesCounter == 0) //if there is no way to save the king, return true - checkmate
            {
                return true;
            }
            Debug.Log(defenderMovesCounter + " possible moves to save the king");
        }//end if attackMoves can kill king
        else
        {
            inCheck = false;
        }//else the king is not in check

        return false;
    }

    private void EndGame()
    {
        EndGameMenu.GameHasEnded = true;
        rotateBoard.enabled = true;
        BoardHighlighting.Instance.HideHighlights();
        selectedFigure = null;
        if (isWhiteTurn)
        {
            WhiteWon = true;
            victory.Play();
        }
        else
        {
            WhiteWon = false;
            defeat.Play();
        }
        //Reload Scene and Play Again
        
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

    public static void MouseClickedTrue()
    {
        mouseClicked = true;
    }

    public static void Pause()
    {
        PauseMenu.GameIsPaused = !PauseMenu.GameIsPaused;
    }

    //Zooming Boolean Functions
    public static void zoomingInTrue()
    {
        zoomingIn = true;
    }
    public static void zoomingInFalse()
    {
        zoomingIn = false;
    }
    public static void zoomingOutTrue()
    {
        zoomingOut = true;
    }
    public static void zoomingOutFalse()
    {
        zoomingOut = false;
    }
    //Rotating Boolean Functions
    public static void rotatingLeftTrue()
    {
        rotatingLeft = true;
    }
    public static void rotatingLeftFalse()
    {
        rotatingLeft = false;
    }
    public static void rotatingRightTrue()
    {
        rotatingRight = true;
    }
    public static void rotatingRightFalse()
    {
        rotatingRight = false;
    }

    private void OnZoomIn()
    {   //Zoom Camera In
        Camera.main.transform.position += cameraTransform.forward * (Time.deltaTime);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 80, 2 * Time.deltaTime);
    }//end OnZoomIn

    private void OnZoomOut()
    {   //Zoom Camera Out
        Camera.main.transform.position -= cameraTransform.forward * (Time.deltaTime);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 80, 2 * Time.deltaTime);
    }//end OnZoomOut

    private void OnRotateLeft()
    {   //Rotate Camera Left
        Camera.main.transform.Rotate(0.0f, 0.0f, 0.2f, Space.Self);
    }//end OnRotateLeft

    private void OnRotateRight()
    {   //Rotate Camera Right
        Camera.main.transform.Rotate(0.0f, 0.0f, -0.2f, Space.Self);
    }//end OnRotateRight
}