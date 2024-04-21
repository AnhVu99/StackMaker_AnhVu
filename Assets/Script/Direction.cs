using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Direction : Singleton<Direction> // like config
{
    public enum Dir
    {
        Left = -1,
        Right = 1,
        Up = 2,
        Down = -2,
    }
    public static readonly int maxBrick = 20;
    public static readonly float offsetLineY = 2.64f;
    public static readonly float offsetWallY = 0f;
    public static readonly float offsetPlayerY = 3f;
    //public static readonly string WIN_POINT = "WIN";

}
