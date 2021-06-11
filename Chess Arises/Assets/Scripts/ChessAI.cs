using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessAI : MonoBehaviour {

    System.Random r;
	public Vector2 MakeMove(ChessFigure figure)
    {
        r = new System.Random();
        
        bool[,] possibleMoves = figure.PossibleMove(BoardManager.Instance.ChessFigurePositions);

        List<Vector2> possibleMovements = new List<Vector2>();

        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            { 
                if(possibleMoves[i,j])
                {
                    //Debug.Log(i + " = X. " + j + " = Y. The possible moves for the figure " + figure);
                    possibleMovements.Add(new Vector2(i, j));
                }
            }
        }

        if (possibleMovements[r.Next(possibleMovements.Count)] != null)
        {
            //Debug.Log(possibleMovements[r.Next(possibleMovements.Count)] + " possible movements for " + figure);
            return possibleMovements[r.Next(possibleMovements.Count)];
        }

        else 
        {
            //Debug.Log(possibleMovements[r.Next(possibleMovements.Count)]);
            //Debug.Log(possibleMovements.Count + " is the count of moves available for " + figure);
            return new Vector2(-1, -1); 
        }
    }

    public ChessFigure SelectChessFigure()
    {
        r = new System.Random();
        List<GameObject> activeFigures = BoardManager.Instance.GetAllBlackFigures();
        GameObject chosenPiece = activeFigures[r.Next(activeFigures.Count)];
        return chosenPiece.GetComponent<ChessFigure>();
    }
}
