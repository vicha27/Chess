using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialChessFigure : MonoBehaviour
{
    public int CurrentX { get; set; }
    public int CurrentY { get; set; }
    public bool isWhite;
    public bool hasMoved;
    private Vector3 desiredPosition;
    private Vector3 desiredScale = new Vector3(0.2f, 0.2f, 0.2f);
    //private Vector3 desiredScale =Vector3.one;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        CurrentX = (int)desiredPosition.x;
        CurrentY = (int)desiredPosition.z;
        if (force)
        {
            transform.position = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
            transform.localScale = desiredScale;
    }

    public virtual bool[,] PossibleMove(TutorialChessFigure[,] ChessFigurePositions)
    {
        return new bool[8,8];
    }
}
