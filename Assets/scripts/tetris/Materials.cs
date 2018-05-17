using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials {
    public static Material Green = Resources.Load("materials/mat_green", typeof(Material)) as Material;
    public static Material Blue = Resources.Load("materials/mat_blue", typeof(Material)) as Material;
    public static Material Red = Resources.Load("materials/mat_red", typeof(Material)) as Material;
    public static Material Purple = Resources.Load("materials/mat_purble", typeof(Material)) as Material;
    public static Material Orange = Resources.Load("materials/mat_orange", typeof(Material)) as Material;
    public static Material Gray = Resources.Load("materials/mat_gray", typeof(Material)) as Material;
    public static Material White = Resources.Load("materials/mat_white", typeof(Material)) as Material;

    public static Material RandomColor()
    {
        int r = UnityEngine.Random.Range(1, 6);

        switch (r)
        {
            case 1: return Green;
            case 2: return Blue;
            case 3: return Red;
            case 4: return Purple;
            case 5: return Orange;
        }

        return Gray;
    }
}
