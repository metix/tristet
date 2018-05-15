using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriminoType {

    public static Material mGreen = Resources.Load("materials/mat_green", typeof(Material)) as Material;
    public static Material mBlue = Resources.Load("materials/mat_blue", typeof(Material)) as Material;
    public static Material mRed = Resources.Load("materials/mat_red", typeof(Material)) as Material;
    public static Material mPurple = Resources.Load("materials/mat_purble", typeof(Material)) as Material;
    public static Material mOrange = Resources.Load("materials/mat_orange", typeof(Material)) as Material;
    public static Material mGray = Resources.Load("materials/mat_gray", typeof(Material)) as Material;
    public static Material mWhite = Resources.Load("materials/mat_white", typeof(Material)) as Material;

    public int[,] Array { get; set; }
    public Material Material { get; set; }
    public string Name { get; set; }

    public static TetriminoType I()
    {
        return new TetriminoType
        {
            Array = new int[,] {
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 }
        },
            Material = mGreen,
            Name = "I"
        };
    }

    public static TetriminoType J()
    {
        return new TetriminoType
        {
            Array = new int[,] {
            { 0, 0, 0},
            { 1, 1, 1 },
            { 0, 0, 1 },
        },
            Material = mBlue,
            Name = "J"
        };
    }

    public static TetriminoType L()
    {
        return new TetriminoType
        {
            Array = new int[,] {
            { 0, 0, 0},
            { 1, 1, 1 },
            { 1, 0, 0 },
        },
            Material = mRed,
            Name = "L"
        };
    }

    public static TetriminoType O()
    {
        return new TetriminoType
        {
            Array = new int[,] {
            { 0, 0, 0, 0 },
            { 0, 1, 1, 0 },
            { 0, 1, 1, 0 },
            { 0, 0, 0, 0 }
        },
            Material = mPurple,
            Name = "O"
        };
    }

    public static TetriminoType S()
    {
    return new TetriminoType
    {
        Array = new int[,] {
            { 0, 1, 1},
            { 0, 1, 0 },
            { 1, 1, 0 },
        },
        Material = mOrange,
        Name = "S"
    };
    }

    public static TetriminoType T()
    {
        return new TetriminoType
        {
            Array = new int[,] {
            { 0, 0, 0},
            { 0, 1, 0 },
            { 1, 1, 1 },
        },
            Material = mGray,
            Name = "T"
        };
    }

    public static TetriminoType Z()
    {
        return new TetriminoType
        {
            Array = new int[,] {
            { 1, 1, 0},
            { 0, 1, 0 },
            { 0, 1, 1 },
        },
            Material = mWhite,
            Name = "Z"
        };
    }

    public static TetriminoType Random()
    {
        int r = UnityEngine.Random.Range(1, 8);

        switch (r)
        {
            case 1: return TetriminoType.I();
            case 2: return TetriminoType.J();
            case 3: return TetriminoType.L();
            case 4: return TetriminoType.O();
            case 5: return TetriminoType.S();
            case 6: return TetriminoType.T();
            case 7: return TetriminoType.Z();
        }

        return I();
    }
}
