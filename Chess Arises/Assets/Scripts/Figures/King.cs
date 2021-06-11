using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessFigure
{
    public override bool[,] PossibleMove(ChessFigure[,] ChessFigurePositions)
    {
        bool[,] r = new bool[8, 8];
        bool inCheck = false;

        ChessFigure c;
        ChessFigure c2;
        ChessFigure c3;
        ChessFigure c4;
        int i, j;

        // Top
        i = CurrentX - 1;
        j = CurrentY + 1;
        if(CurrentY < 7 && !inCheck)
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
        if (CurrentY > 0 && !inCheck)
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
        if(CurrentX > 0 && !inCheck)
        {
            c = ChessFigurePositions[CurrentX - 1, CurrentY];
            if (c == null) r[CurrentX - 1, CurrentY] = true;
            else if (c.isWhite != isWhite) r[CurrentX - 1, CurrentY] = true;
        }

        // Right
        if (CurrentX < 7 && !inCheck)
        {
            c = ChessFigurePositions[CurrentX + 1, CurrentY];
            if (c == null) r[CurrentX + 1, CurrentY] = true;
            else if (c.isWhite != isWhite) r[CurrentX + 1, CurrentY] = true;
        }

        // Castle Long (Aka Go to the Left)
        if (CurrentX > 3 && !hasMoved && !inCheck)
        {
            c = ChessFigurePositions[CurrentX - 1, CurrentY];
            c2 = ChessFigurePositions[CurrentX - 2, CurrentY];
            c3 = ChessFigurePositions[CurrentX - 3, CurrentY];
            c4 = ChessFigurePositions[CurrentX - 4, CurrentY];
            if (c4 != null && !c4.hasMoved)
            {
                if (c == null && c2 == null && c3 == null) r[CurrentX - 2, CurrentY] = true;
            }
            //else if (c.isWhite != isWhite) r[CurrentX - 1, CurrentY] = true;
        }

        // Castle Short (Aka Go to the Right)
        if (CurrentX < 5 && !hasMoved && !inCheck)
        {
            c = ChessFigurePositions[CurrentX + 1, CurrentY];
            c2 = ChessFigurePositions[CurrentX + 2, CurrentY];
            c3 = ChessFigurePositions[CurrentX + 3, CurrentY];
            if (c3 != null && !c3.hasMoved)
            {
                if (c == null && c2 == null) r[CurrentX + 2, CurrentY] = true;
            }
            //else if(c == null) r[CurrentX + 1, CurrentY] = true;
        }

        return r;
    }
}
