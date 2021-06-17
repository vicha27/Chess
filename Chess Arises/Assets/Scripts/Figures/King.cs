using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessFigure
{
    public override bool[,] PossibleMove(ChessFigure[,] ChessFigurePositions)
    {
        bool[,] r = new bool[8, 8];

        ChessFigure c;
        ChessFigure c2;
        ChessFigure c3;
        ChessFigure c4;
        int i, j;

        // Top
        i = CurrentX - 1;
        j = CurrentY + 1;
        if(CurrentY < 7)
        {
            for(int k = 0; k < 3; k++)
            {
                if(i >= 0 && i < 8)
                {
                    c = ChessFigurePositions[i, j];
                    if (c == null) r[i, j] = true;
                    else if (c.isWhite != isWhite) r[i, j] = true;
                }
                i++;
            }
        }

        // Bottom
        i = CurrentX - 1;
        j = CurrentY - 1;
        if (CurrentY > 0)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 && i < 8)
                {
                    c = ChessFigurePositions[i, j];
                    if (c == null) r[i, j] = true;
                    else if (c.isWhite != isWhite) r[i, j] = true;
                }
                i++;
            }
        }

        // Left
        if(CurrentX > 0)
        {
            c = ChessFigurePositions[CurrentX - 1, CurrentY];
            if (c == null) r[CurrentX - 1, CurrentY] = true;
            else if (c.isWhite != isWhite) r[CurrentX - 1, CurrentY] = true;
        }

        // Right
        if (CurrentX < 7)
        {
            c = ChessFigurePositions[CurrentX + 1, CurrentY];
            if (c == null) r[CurrentX + 1, CurrentY] = true;
            else if (c.isWhite != isWhite) r[CurrentX + 1, CurrentY] = true;
        }

        // Castle Long (Aka Go to the Left)
        if (CurrentX > 3 && !hasMoved && !BoardManager.inCheck)
        {
            c = ChessFigurePositions[CurrentX - 1, CurrentY];
            c2 = ChessFigurePositions[CurrentX - 2, CurrentY];
            c3 = ChessFigurePositions[CurrentX - 3, CurrentY];
            c4 = ChessFigurePositions[CurrentX - 4, CurrentY];
            if (c4 != null && !c4.hasMoved)
            {
                if (c == null && c2 == null && c3 == null)
                {
                    ChessFigure[,] simulation = new ChessFigure[8, 8];
                    List<ChessFigure> simAttackingPieces = new List<ChessFigure>();
                    int removeMovesCounter = 0;
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            if (ChessFigurePositions[x, y] != null) //if there is a figure at the location
                            {
                                simulation[x, y] = ChessFigurePositions[x, y]; //simulation board copies that location
                                if (simulation[x, y].isWhite != c4.isWhite) //if the piece located there is an enemy
                                {
                                    simAttackingPieces.Add(simulation[x, y]); //add the enemy piece to the list of enemy pieces that will simulate attacking
                                }//end of if simulation piece is an enemy
                            }//end of if the position is not null
                        }//end for loop
                    }//end outer for loop
                     // Get all the simulated attacking pieces moves
                    for (int a = 0; a < simAttackingPieces.Count; a++)
                    {
                        var pieceMoves = simAttackingPieces[a].PossibleMove(simulation); //get the boolean array [,] of allowed moves for the simulating attack piece, use simulation instead of the actual board to simulate what the board would look like
                        for (int d = 0; d < 8; d++)
                        {
                            for (int e = 0; e < 8; e++)
                            {
                                if (pieceMoves[d, e] && (CurrentX - 1) == d && CurrentY == e) //if the move is legal and runs into the king, increment the remove moves counter
                                {
                                    removeMovesCounter = removeMovesCounter + 1;
                                    //Debug.Log("Theoretically, the enemy killed the king at " + d + ", " + e);
                                }
                            }
                        }
                    }//end of simAttackingPieces for loop
                    if (removeMovesCounter == 0) //if the king died in our simulation, remove the move from allowed moves so the king doesn't die
                    {
                        r[CurrentX - 2, CurrentY] = true;
                    }//end if
                }//end if everything is null
            }//end if c4 isn't null and c4 hasn't moved
        }//end if castle long

        // Castle Short (Aka Go to the Right)
        if (CurrentX < 5 && !hasMoved && !BoardManager.inCheck)
        {
            c = ChessFigurePositions[CurrentX + 1, CurrentY];
            c2 = ChessFigurePositions[CurrentX + 2, CurrentY];
            c3 = ChessFigurePositions[CurrentX + 3, CurrentY];
            if (c3 != null && !c3.hasMoved)
            {
                if (c == null && c2 == null) 
                {
                    ChessFigure[,] simulation = new ChessFigure[8, 8];
                    List<ChessFigure> simAttackingPieces = new List<ChessFigure>();
                    int removeMovesCounter = 0;
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            if (ChessFigurePositions[x, y] != null) //if there is a figure at the location
                            {
                                simulation[x, y] = ChessFigurePositions[x, y]; //simulation board copies that location
                                if (simulation[x, y].isWhite != c3.isWhite) //if the piece located there is an enemy
                                {
                                    simAttackingPieces.Add(simulation[x, y]); //add the enemy piece to the list of enemy pieces that will simulate attacking
                                }//end of if simulation piece is an enemy
                            }//end of if the position is not null
                        }//end for loop
                    }//end outer for loop
                     // Get all the simulated attacking pieces moves
                    for (int a = 0; a < simAttackingPieces.Count; a++)
                    {
                        var pieceMoves = simAttackingPieces[a].PossibleMove(simulation); //get the boolean array [,] of allowed moves for the simulating attack piece, use simulation instead of the actual board to simulate what the board would look like
                        for (int d = 0; d < 8; d++)
                        {
                            for (int e = 0; e < 8; e++)
                            {
                                if (pieceMoves[d, e] && (CurrentX + 1) == d && CurrentY == e) //if the move is legal and runs into the king, increment the remove moves counter
                                {
                                    removeMovesCounter = removeMovesCounter + 1;
                                    //Debug.Log("Theoretically, the enemy killed the king at " + d + ", " + e);
                                }
                            }
                        }
                    }//end of simAttackingPieces for loop
                    if (removeMovesCounter == 0) //if the king died in our simulation, remove the move from allowed moves so the king doesn't die
                    {
                        r[CurrentX + 2, CurrentY] = true;
                    }//end if
                }//end if everything is null
            }//end if c3 is good to go
        }//end if Castle Short

        return r;
    }
}
