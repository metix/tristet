using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldScript : MonoBehaviour {

    public class Block
    {
       public GameObject Instance { get; set; }
        public Tetrimino Parent { get; internal set; }
        public Vector2Int Position { get; internal set; }
    }

    public class Tetrimino
    {
        public Tetrimino(int rotation, Vector2Int position, TetriminoType type)
        {
            Rotation = rotation;
            Position = position;
            Type = type;
        }

        public int Rotation { get; set; }
        public Vector2Int Position { get; internal set; }
        public TetriminoType Type { get; set; }

        public Block[,] Blocks { get; set; }
    }

    public GameObject blockPrefab;
    public float speed = 1;
    public int width = 10;
    public float spacing = 1.05f;
    public float rotationUpdateSpeed = 1.05f;
    public float moveUpdateSpeed = 1.05f;
    public Material matWall;

    private Tetrimino currentTetrimino;
    private List<List<Block>> field;

    private GameObject wallLeft;
    private GameObject wallRight;

    void Start () {
        field = new List<List<Block>>();

        currentTetrimino = InstantiateTetrimino(new Vector2Int(0, 5), TetriminoType.I(), 0);

        PrintField();
    }

    private void PrintField()
    {
        string f = "";

        for (var y = 0; y < field.Count; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (field[field.Count - y - 1][x] != null)
                    f += "1 ";
                else
                    f += "0 ";
            }

            f += "\n";
        }

        Debug.Log(f);
    }

    private List<Block> EmptyRow()
    {
        List<Block> row = new List<Block>();

        for (var i = 0; i < width; i++)
        {
            row.Add(null);
        }

        return row;
    }

    static int[,] RotateMatrix(int[,] matrix, int n)
    {
        int[,] ret = new int[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = matrix[n - j - 1, i];
            }
        }

        return ret;
    }

    static Block[,] RotateBlocks(Block[,] matrix, int n)
    {
        Block[,] ret = new Block[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = matrix[n - j - 1, i];
            }
        }

        return ret;
    }

    static int[,] RotateNTimes(int[,] array, int n)
    {
        for (var i = 0; i < n; i++)
        {
            array = RotateMatrix(array, array.GetLength(0));
        }

        return array;
    }

    private bool BlockRowContainsBlock(Block [,] array, int row)
    {
        for (var i = 0; i < array.GetLength(0); i++)
            if (array[row, i] != null) return true;
        return false;
    }

    private bool BlockColumnContainsBlocks(Block[,] array, int column)
    {
        for (var i = 0; i < array.GetLength(0); i++)
            if (array[i, column] != null) return true;
        return false;
    }

    private bool FieldRowContainsBlock(List<List<Block>> field, int row)
    {
        if (row < 0)
            return false;

        if (row > field.Count)
            return false;

        for (var i = 0; i < field[row].Count; i++)
        {
            if (field[row][i] != null)
                return true;
        }

        return false;
    }

    public bool CanRotate(Tetrimino tetrimino)
    {
        Block[,] rotated = RotateBlocks(tetrimino.Blocks, tetrimino.Blocks.GetLength(0));

        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
        {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Position.x + x < 0)
                    return false;

                if (tetrimino.Position.x + x >= width)
                    return false;

                if (field[tetrimino.Position.y + y][tetrimino.Position.x + x] != null && rotated[y, x] != null)
                    return false;
            }
        }

        return true;
    }

    public void PrintBlockArray(Block[,] blocks)
    {
        string f = "";

        for (var y = 0; y < blocks.GetLength(0); y++)
        {
            for (var x = 0; x < blocks.GetLength(0); x++)
            {
                if (blocks[blocks.GetLength(0) - y - 1, x] != null)
                    f += "1 ";
                else
                    f += "0 ";
            }

            f += "\n";
        }

        Debug.Log(f);
    }

    public void Rotate(Tetrimino tetrimino)
    {
        int n = tetrimino.Blocks.GetLength(0);

        PrintBlockArray(tetrimino.Blocks);

        Block[,] ret = new Block[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = tetrimino.Blocks[n - j - 1, i];

                if (tetrimino.Blocks[n - j - 1, i] != null)
                {
                    tetrimino.Blocks[n - j - 1, i].Instance.GetComponent<BlockScript>().UpdatePosition(new Vector3((tetrimino.Position.x + j) * spacing, (tetrimino.Position.y + i) * spacing), rotationUpdateSpeed);
                }
            }
        }

        tetrimino.Blocks = ret;

        PrintBlockArray(tetrimino.Blocks);

    }

    public bool CanMoveRight(Tetrimino tetrimino)
    {
        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
        {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Position.x + x + 1 == width)
                {
                    if (BlockColumnContainsBlocks(tetrimino.Blocks, tetrimino.Blocks.GetLength(0) - 1)) return false;
                    else continue;
                }

                if (tetrimino.Position.x + x + 1 == width + 1)
                {
                    if (BlockColumnContainsBlocks(tetrimino.Blocks, tetrimino.Blocks.GetLength(0) - 2)) return false;
                    else continue;
                }

                if (tetrimino.Position.x + x + 1 >= width + 1)
                {
                    return false;
                }

                if (tetrimino.Position.x + x < 0)
                    continue;

                if (field[tetrimino.Position.y + y][tetrimino.Position.x + x + 1] != null &&
                    tetrimino.Blocks[y, x] != null)
                    return false;
            }
        }

        return true;
    }

    public void MoveRight(Tetrimino tetrimino)
    {
        tetrimino.Position += new Vector2Int(1, 0);

        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
        {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Blocks[y, x] != null)
                    tetrimino.Blocks[y, x].Instance.GetComponent<BlockScript>().UpdatePosition(new Vector3((tetrimino.Position.x + x) * spacing, (tetrimino.Position.y + y) * spacing), moveUpdateSpeed);
            }
        }
    }

    public bool CanMoveLeft(Tetrimino tetrimino)
    {
        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
        {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Position.x + x - 1 == -1)
                {
                    if (BlockColumnContainsBlocks(tetrimino.Blocks, 0)) return false;
                    else continue;
                }

                if (tetrimino.Position.x + x - 1 == -2)
                {
                    if (BlockColumnContainsBlocks(tetrimino.Blocks, 1)) return false;
                    else continue;
                }

                if (tetrimino.Position.x + x - 1 <= -3)
                {
                    return false;
                }


                if (tetrimino.Position.x + x >=  width)
                    continue;

                if (field[tetrimino.Position.y + y][tetrimino.Position.x + x - 1] != null &&
                    tetrimino.Blocks[y, x] != null)
                return false;
            }
        }

        return true;
    }

    public void MoveLeft(Tetrimino tetrimino)
    {
        tetrimino.Position += new Vector2Int(-1, 0);

        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
        {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Blocks[y, x] != null)
                    tetrimino.Blocks[y, x].Instance.GetComponent<BlockScript>().UpdatePosition(new Vector3((tetrimino.Position.x + x) * spacing, (tetrimino.Position.y + y) * spacing), moveUpdateSpeed);
            }
        }
    }

    public bool CanMoveDown(Tetrimino tetrimino)
    {
        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++) {

            if (tetrimino.Position.y + y - 1 <= -3)
            {
                return false;
            }

            if (tetrimino.Position.y + y - 1 == -2)
            {
                if (BlockRowContainsBlock(tetrimino.Blocks, 1))
                    return false;
                else
                    continue;
            }

            if (tetrimino.Position.y + y - 1 == -1)
            {
                if (BlockRowContainsBlock(tetrimino.Blocks, 0))
                    return false;
                else
                    continue;
            }

            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Position.x + x < 0 || tetrimino.Position.x + x >= width)
                {
                    continue;
                }

                if (field[tetrimino.Position.y + y - 1][tetrimino.Position.x + x] != null &&
                    tetrimino.Blocks[y, x] != null)
                    return false;
            }
        }

        return true;
    }

    public int FindTopRow()
    {
        for (var y = field.Count; y >= 0; y--)
        {
            if (FieldRowContainsBlock(field, y - 1))
                return y;
        }

        return 0;
    }

    public void MoveDown(Tetrimino tetrimino)
    {
        tetrimino.Position += new Vector2Int(0, -1);

        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
        {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Blocks[y, x] != null)
                tetrimino.Blocks[y, x].Instance.GetComponent<BlockScript>().UpdatePosition(new Vector3((tetrimino.Position.x + x) * spacing, (tetrimino.Position.y + y) * spacing), moveUpdateSpeed);
            }
        }
    }

    public Tetrimino InstantiateTetrimino(Vector2Int pos, TetriminoType type, int rotation)
    {
        // add empty rows on top of field, to avoid array out of bounds exceptions
        for (var i = 0; i < (pos.y + 20) - field.Count; i++)
            field.Add(EmptyRow());

        int[,] array = RotateNTimes(type.Array, rotation);

        Tetrimino tetrimino = new Tetrimino(rotation, pos, type);

        tetrimino.Blocks = new Block[array.GetLength(0), array.GetLength(0)];
        GameObject newTetrimino = new GameObject();
        newTetrimino.name = "tetrimino_" + type.Name;


        for (var y = 0; y < array.GetLength(0); y++)
        {
            for (var x = 0; x < array.GetLength(0); x++)
            {
                if (array[array.GetLength(0) - y - 1, x] == 1)
                {
                    Block block = new Block();
                    block.Parent = tetrimino;
                    block.Position = new Vector2Int(pos.x + x, pos.y + y);
                    block.Instance = Instantiate(blockPrefab, new Vector3((pos.x + x) * spacing, (pos.y + y) * spacing), Quaternion.identity);
                    block.Instance.GetComponent<Renderer>().material = type.Material;

                    block.Instance.transform.SetParent(newTetrimino.transform);

                    tetrimino.Blocks[y, x] = block;
                }
            }
        }

        return tetrimino;
    }

    public void InsertTetrimino(Tetrimino tetrimino)
    {
       for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
       {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Blocks[y, x] != null)
                {
                    field[tetrimino.Position.y + y][tetrimino.Position.x + x] = tetrimino.Blocks[y, x];
                }
            }
        }
    }

    private float period = 0.0f;
    private int wallHeight = 0;

    public void Update()
    {
        int topRow = FindTopRow() + 10;

        if (wallHeight < topRow)
        {
            Debug.Log("add walls");
            
           for (var i = 0; i < topRow - wallHeight; i++)
           {
                var spawnPos = new Vector3(-1, (topRow - i + 5) * spacing, 0);
                var destPos = new Vector3(-1, (topRow - i - 2) * spacing, 0);
                var wallBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
                wallBlock.GetComponent<Renderer>().material = matWall;
                wallBlock.GetComponent<BlockScript>().UpdatePosition(destPos, 3f);

                spawnPos = new Vector3(width + 1, (topRow - i + 5) * spacing, 0);
                destPos = new Vector3(width + 1, (topRow - i - 2) * spacing, 0);
                wallBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
                wallBlock.GetComponent<Renderer>().material = matWall;
                wallBlock.GetComponent<BlockScript>().UpdatePosition(destPos, 3f);
            }

            wallHeight = topRow;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentTetrimino != null)
            {
                if (CanMoveRight(currentTetrimino))
                    MoveRight(currentTetrimino);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentTetrimino != null)
            {
                if (CanMoveLeft(currentTetrimino))
                    MoveLeft(currentTetrimino);
            }
        } else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (CanRotate(currentTetrimino))
            {
                Rotate(currentTetrimino);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            while (CanMoveDown(currentTetrimino))
            {
                MoveDown(currentTetrimino);
            }
        }

        if (period > speed)
        {
            if (currentTetrimino != null)
            {
                if (CanMoveDown(currentTetrimino))
                {
                    MoveDown(currentTetrimino);
                }
                else
                {
                    InsertTetrimino(currentTetrimino);
                    currentTetrimino = null;

                    currentTetrimino = InstantiateTetrimino(new Vector2Int(0, FindTopRow() + 2), TetriminoType.Random(), 0);

                    iTween.MoveTo(Camera.main.gameObject, new Vector3(0, FindTopRow() * spacing + 2, -10), 2);

                    PrintField();
                }
            }
            period = 0;
        }
        period += UnityEngine.Time.deltaTime;
    }
}
