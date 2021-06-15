using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper 
{
    public static  Dirrection GetOppositeDirrection(Dirrection dirrection)
    {
        switch (dirrection)
        {
            case Dirrection.Back:
                return Dirrection.Forward;
            case Dirrection.Forward:
                return Dirrection.Back;
            case Dirrection.Left:
                return Dirrection.Right;
            case Dirrection.Right:
                return Dirrection.Left;
        }
        Debug.LogError("Dirrection - " + dirrection + " is not maintanable");
        return Dirrection.Right;
    }

    
    public static Vector2Int GetDirrectionForPlacing(Dirrection dirrection)
    {
        switch(dirrection)
        {
            case Dirrection.Back:
                return new Vector2Int(0, -1);
            case Dirrection.Forward:
                return new Vector2Int(0, 1);
            case Dirrection.Left:
                return new Vector2Int(-1, 0);
            case Dirrection.Right:
                return new Vector2Int(1, 0);
        }
        Debug.LogError("Dirrection - " + dirrection + " is not maintanable");
        return new Vector2Int();
    }
}

public class Joint
{
    public int Position { get; private set; }
    public int Length { get; private set; }

    public Joint(int position, int length)
    {
        Position = position;
        Length = length;
    }
}

public enum PlayerState
{
    Normal,
    Accelerated,
}

public enum Dirrection
{
    Left,
    Right,
    Forward,
    Back,
}
