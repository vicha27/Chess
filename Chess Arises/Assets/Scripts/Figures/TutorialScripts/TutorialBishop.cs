using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBishop : TutorialChessFigure
{
    public override bool[,] PossibleMove(TutorialChessFigure[,] ChessFigurePositions)
    {
        bool[,] r = new bool[8, 8];

        TutorialChessFigure c;
        int i, j;

        // Top Left
        i = CurrentX;
        j = CurrentY;
        while(true)
        {
            i--;
            j++;
            if (i < 0 || j >= 8) break;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
                break;
            }
        }

        // Top Right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j++;
            if (i >= 8 || j >= 8) break;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
                break;
            }
        }

        // Bottom Left
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j--;
            if (i < 0 || j < 0) break;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
                break;
            }
        }

        // Bottom Right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j--;
            if (i >= 8 || j < 0) break;
            c = ChessFigurePositions[i, j];
            if (c == null) r[i, j] = true;
            else
            {
                if (c.isWhite != isWhite) r[i, j] = true;
                break;
            }
        }

        return r;
    }
}
