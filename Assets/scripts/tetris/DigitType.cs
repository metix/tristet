using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitType {

    public int[,] Array { get; set; }
    public Material Material { get; set; }
    public string Name { get; set; }

    public static DigitType N_0()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 1, 1},
            { 1, 0, 1},
            { 1, 0, 1},
            { 1, 0, 1},
            { 1, 1, 1},
        },
            Material = Materials.Gray,
            Name = "0"
        };
    }

    public static DigitType N_1()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 0, 1, 0},
            { 1, 1, 0},
            { 0, 1, 0},
            { 0, 1, 0},
            { 0, 1, 0},
        },
            Material = Materials.Gray,
            Name = "1"
        };
    }

    public static DigitType N_2()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 1, 1},
            { 1, 0, 1},
            { 0, 0, 1},
            { 0, 1, 0},
            { 1, 1, 1},
        },
            Material = Materials.Gray,
            Name = "2"
        };
    }

    public static DigitType N_3()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 1, 1},
            { 0, 0, 1},
            { 1, 1, 1},
            { 0, 0, 1},
            { 1, 1, 1},
        },
            Material = Materials.Gray,
            Name = "3"
        };
    }

    public static DigitType N_4()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 0, 1},
            { 1, 0, 1},
            { 1, 1, 1},
            { 0, 0, 1},
            { 0, 0, 1},
        },
            Material = Materials.Gray,
            Name = "4"
        };
    }

    public static DigitType N_5()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 1, 1},
            { 1, 0, 0},
            { 1, 1, 1},
            { 0, 0, 1},
            { 1, 1, 1},
        },
            Material = Materials.Gray,
            Name = "5"
        };
    }

    public static DigitType N_6()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 1, 1},
            { 1, 0, 0},
            { 1, 1, 1},
            { 1, 0, 1},
            { 1, 1, 1},
        },
            Material = Materials.Gray,
            Name = "6"
        };
    }

    public static DigitType N_7()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 1, 1},
            { 0, 0, 1},
            { 0, 1, 0},
            { 1, 0, 0},
            { 1, 0, 0},
        },
            Material = Materials.Gray,
            Name = "7"
        };
    }

    public static DigitType N_8()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 1, 1},
            { 1, 0, 1},
            { 1, 1, 1},
            { 1, 0, 1},
            { 1, 1, 1},
        },
            Material = Materials.Gray,
            Name = "8"
        };
    }

    public static DigitType N_9()
    {
        return new DigitType
        {
            Array = new int[,] {
            { 1, 1, 1},
            { 1, 0, 1},
            { 1, 1, 1},
            { 0, 0, 1},
            { 1, 1, 1},
        },
            Material = Materials.Gray,
            Name = "9"
        };
    }

    public static DigitType Digit(int digit)
    {
        switch (digit)
        {
            case 0: return N_0();
            case 1: return N_1();
            case 2: return N_2();
            case 3: return N_3();
            case 4: return N_4();
            case 5: return N_5();
            case 6: return N_6();
            case 7: return N_7();
            case 8: return N_8();
            case 9: return N_9();
        }

        return null;
    }

    public static GameObject InstantiateNumber(int number, Vector3 pos, GameObject prefab)
    {
        var newNumber = new GameObject();
        newNumber.name = "row_number_" + number;
        var spacingBlocks = 1f;
        var spacingDigits = 1;

        string numberString = number.ToString();

        for (var i = 0; i < numberString.Length; i++)
        {
            var digitType = Digit(int.Parse(numberString[i].ToString()));

            for (var y = 0; y < digitType.Array.GetLength(0); y++)
            {
                for (var x = 0; x < digitType.Array.GetLength(1); x++)
                {
                    if (digitType.Array[y, x] != 1)
                        continue;

                    var targetPosition = new Vector3((pos.x + x + (i * (3 + spacingDigits))) * spacingBlocks, (pos.y + digitType.Array.GetLength(0) - y) * spacingBlocks, pos.z);

                    var digit = Object.Instantiate(prefab, targetPosition, Quaternion.identity);
                    digit.GetComponent<Renderer>().material = Materials.Gray;

                    digit.transform.SetParent(newNumber.transform);
                }
            }
        }
        return newNumber;
    }
}
