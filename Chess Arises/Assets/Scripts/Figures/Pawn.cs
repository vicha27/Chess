using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessFigure
{
    public override bool[,] PossibleMove(ChessFigure[,] ChessFigurePositions)
    {
        bool[,] r = new bool[8, 8];
        ChessFigure c, c2;

        if (isWhite)
        {
            // Diagonal Left
            if(CurrentX != 0 && CurrentY != 7)
            {
                c = ChessFigurePositions[CurrentX -1, CurrentY +1];
                if(c != null && !c.isWhite)
                {
                    r[CurrentX - 1, CurrentY + 1] = true;
                }
            }
            // En Passant Left
            if (CurrentY == 4 && CurrentX != 0)
            {
                c = ChessFigurePositions[CurrentX - 1, CurrentY];
                Vector2Int[] lastMove = BoardManager.moveList[BoardManager.moveList.Count - 1];
                if (c != null && !c.isWhite && c.GetType() == typeof(Pawn))
                {
                    if (lastMove[0].y - lastMove[1].y == 2 && lastMove[1].x == CurrentX - 1 && lastMove[1].y == CurrentY)
                    {
                        r[CurrentX - 1, CurrentY + 1] = true;
                    }
                }
            }

            // Diagonal Right
            if (CurrentX != 7 && CurrentY != 7)
            {
                c = ChessFigurePositions[CurrentX + 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                {
                    r[CurrentX + 1, CurrentY + 1] = true;
                }
            }
            // En Passant Right
            if (CurrentY == 4 && CurrentX != 7)
            {
                c = ChessFigurePositions[CurrentX + 1, CurrentY];
                Vector2Int[] lastMove = BoardManager.moveList[BoardManager.moveList.Count - 1];
                if(c != null && !c.isWhite && c.GetType() == typeof(Pawn))
                {
                    if(lastMove[0].y - lastMove[1].y == 2 && lastMove[1].x == CurrentX + 1 && lastMove[1].y == CurrentY)
                    {
                        r[CurrentX + 1, CurrentY + 1] = true;
                    }
                }
            }

            // Forward
            if(CurrentY != 7)
            {
                c = ChessFigurePositions[CurrentX, CurrentY + 1];
                if(c == null)
                {
                    r[CurrentX, CurrentY + 1] = true;
                }
            }
            // Two Steps Forward
            if(CurrentY == 1)
            {
                c = ChessFigurePositions[CurrentX, CurrentY + 1];
                c2 = ChessFigurePositions[CurrentX, CurrentY + 2];
                if(c == null && c2 == null)
                {
                    r[CurrentX, CurrentY + 2] = true;
                }
            }
        }
        else
        {//else the Pawn is Black
            // Diagonal Left
            if (CurrentX != 0 && CurrentY != 0)
            {
                c = ChessFigurePositions[CurrentX - 1, CurrentY - 1];
                if (c != null && c.isWhite)
                {
                    r[CurrentX - 1, CurrentY - 1] = true;
                }
            }
            // En Passant Left
            if (CurrentY == 3 && CurrentX != 0)
            {
                c = ChessFigurePositions[CurrentX - 1, CurrentY];
                Vector2Int[] lastMove = BoardManager.moveList[BoardManager.moveList.Count - 1];
                if (c != null && c.isWhite && c.GetType() == typeof(Pawn))
                {
                    if (lastMove[0].y - lastMove[1].y == -2 && lastMove[1].x == CurrentX - 1 && lastMove[1].y == CurrentY)
                    {
                        r[CurrentX - 1, CurrentY - 1] = true;
                    }
                }
            }
            // Diagonal Right
            if (CurrentX != 7 && CurrentY != 0)
            {
                c = ChessFigurePositions[CurrentX + 1, CurrentY - 1];
                if (c != null && c.isWhite)
                {
                    r[CurrentX + 1, CurrentY - 1] = true;
                }
            }
            // En Passant Right
            if (CurrentY == 3 && CurrentX != 7)
            {
                c = ChessFigurePositions[CurrentX + 1, CurrentY];
                Vector2Int[] lastMove = BoardManager.moveList[BoardManager.moveList.Count - 1];
                if (c != null && c.isWhite && c.GetType() == typeof(Pawn))
                {
                    if (lastMove[0].y - lastMove[1].y == -2 && lastMove[1].x == CurrentX + 1 && lastMove[1].y == CurrentY)
                    {
                        r[CurrentX + 1, CurrentY - 1] = true;
                    }
                }
            }
            // Forward
            if (CurrentY != 0)
            {
                c = ChessFigurePositions[CurrentX, CurrentY - 1];
                if (c == null)
                {
                    r[CurrentX, CurrentY - 1] = true;
                }
            }
            // Two Steps Forward
            if (CurrentY == 6)
            {
                c = ChessFigurePositions[CurrentX, CurrentY - 1];
                c2 = ChessFigurePositions[CurrentX, CurrentY - 2];
                if (c == null && c2 == null)
                {
                    r[CurrentX, CurrentY - 2] = true;
                }
            }
        }

        return r;
    }
}
