using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessFigure
{
    public override bool[,] PossibleMove(ChessFigure[,] ChessFigurePositions)
    {
        bool[,] r = new bool[8, 8];

        ChessFigure c;
        int i, j;

        // Up/Left
        i = CurrentX;
        j = CurrentY;

        if (i != 0 && j < 6)
        {
            i--;
            j += 2;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
            }
        }
        

        // Left/Up
        i = CurrentX;
        j = CurrentY;
        if (i > 1 && j != 7)
        {
            i-=2;
            j++;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
            }
        }

        // Up/Right
        i = CurrentX;
        j = CurrentY;
        if (i != 7 && j < 6)
        {
            i++;
            j+=2;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
            }
        }

        // Right/Up
        i = CurrentX;
        j = CurrentY;
        if (i < 6 && j != 7)
        {
            i+=2;
            j++;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
            }
        }

        // Down/Left
        i = CurrentX;
        j = CurrentY;
        if (i != 0 && j > 1)
        {
            i--;
            j-=2;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
            }
        }

        // Left/Down
        i = CurrentX;
        j = CurrentY;
        if (i > 1 && j != 0)
        {
            i -= 2;
            j--;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
            }
        }

        // Down/Right
        i = CurrentX;
        j = CurrentY;
        if (i != 7 && j > 1)
        {
            i++;
            j-=2;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
            }
        }

        // Right/Down
        i = CurrentX;
        j = CurrentY;
        if (i < 6 && j != 0)
        {
            i += 2;
            j--;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
            }
        }

        return r;
    }
}
