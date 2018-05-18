using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFieldScript : MonoBehaviour {

    public class Block
    {
       public GameObject Instance { get; set; }
        public Tetrimino Parent { get; internal set; }
        public Vector2Int Position { get; internal set; }
    }

    public class Tetrimino
    {
        public Tetrimino(int rotation, Vector2Int position, TetriminoType type, bool staticTetrimino)
        {
            Rotation = rotation;
            Position = position;
            Type = type;
            IsStatic = staticTetrimino;
        }

        public int Rotation { get; set; }
        public Vector2Int Position { get; internal set; }
        public TetriminoType Type { get; set; }
        public bool IsStatic { get; set; }

        public Block[,] Blocks { get; set; }
    }

    public int deathCounter;
    public GameObject blockPrefab;
    public float speed = 1;

    public float speedIncreaseFactor = -0.4f;
    public int width = 10;
    public float spacing = 1.05f;
    public float rotationUpdateSpeed = 1.05f;
    public float moveUpdateSpeed = 1.05f;
    public Material matWall;
    public Material matStaticWall;

    public GameObject player;
    public GameObject playerPrefab;

    public GameObject ui;

    public int staticNumberWallOffset = 10;

    public float score = -5;
    public float best = 0;

    private Tetrimino currentTetrimino;
    private List<List<Block>> field;

    private GameObject wallLeft;
    private GameObject wallRight;

    private int wallHeight = 0;
    private int lastInsertedStaticNumberWallHeight = -1;

    private Plane[] frustumPlanes;

    private GameObject allObjectsParent;
    private GameObject tetrominosParent;
    private GameObject wallsParent;
    private GameObject numbersParent;
    private GameObject levelBlocksParent;

    private bool gameStop = true;

    void Start() {
        InitGame();  
    }

    public void RestartGame()
    {
        Debug.Log("restart game");
        InitGame();
        Invoke("StartGame", 1);
    }

    void InitGame()
    {
        if (allObjectsParent != null)
        {
            Destroy(allObjectsParent);
        }

        if (player != null)
        {
            Destroy(player);  
        }

        player = Instantiate(playerPrefab, new Vector3(1, 0, 0), Quaternion.Euler(0, 180, 0));

        allObjectsParent = new GameObject();
        allObjectsParent.name = "game";

        tetrominosParent = new GameObject();
        tetrominosParent.transform.SetParent(allObjectsParent.transform);
        tetrominosParent.name = "tetrominos";

        wallsParent = new GameObject();
        wallsParent.transform.SetParent(allObjectsParent.transform);
        wallsParent.name = "walls";

        numbersParent = new GameObject();
        numbersParent.transform.SetParent(allObjectsParent.transform);
        numbersParent.name = "numbers";

        levelBlocksParent = new GameObject();
        levelBlocksParent.transform.SetParent(allObjectsParent.transform);
        levelBlocksParent.name = "level-separators";

        field = new List<List<Block>>();
        wallHeight = 0;
        score = -5;
        speed = 1;
        lastInsertedStaticNumberWallHeight = -1;
        InsertWalls();
        InsertStaticNumberWalls();
        currentTetrimino = InstantiateTetrimino(new Vector2Int(0, 5), TetriminoType.I(), 0, false);
        UpdateCamera(new Vector3(5f, FindTopRow(false) * spacing + 2, -10));

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", GetComponent<AudioLowPassFilter>().cutoffFrequency,
            "to", 10000,
            "time", 1f,
            "onupdatetarget", gameObject,
            "onupdate", "tweenOnUpdateCallBack",
            "easetype", iTween.EaseType.easeOutQuad
            )
        );
    }

    public void StartGame()
    {
        gameStop = false;

        if (!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Play();
    }

    void tweenOnUpdateCallBack(int newValue)
    {
        GetComponent<AudioLowPassFilter>().cutoffFrequency = newValue;
    }

    public void GameOver()
    {
        deathCounter++;
        gameStop = true;

        iTween.ValueTo(gameObject, iTween.Hash(
          "from", GetComponent<AudioLowPassFilter>().cutoffFrequency,
          "to", 250,
          "time", 1f,
          "onupdatetarget", gameObject,
          "onupdate", "tweenOnUpdateCallBack",
          "easetype", iTween.EaseType.easeOutQuad
          )
      );

        if (score > best)
        {
            best = score;
        }

        if (score < 0)
            score = 0;
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

    static Block[,] RotateBlocksLeft(Block[,] matrix, int n)
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

    static Block[,] RotateBlocksRight(Block[,] matrix, int n)
    {
        Block[,] ret = new Block[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = matrix[j, n - i - 1];
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

    private bool FieldRowContainsBlock(List<List<Block>> field, int row, bool withStatic)
    {
        if (row < 0)
            return false;

        if (row > field.Count)
            return false;

        for (var i = 0; i < field[row].Count; i++)
        {
            if (!withStatic && field[row][i] != null && field[row][i].Parent.IsStatic)
                continue;
            if (field[row][i] != null)
                return true;
        }

        return false;
    }

    public bool CanPlaceOnField(TetriminoType type, Vector2Int position)
    {
        for (var y = 0; y < type.Array.GetLength(0); y++)
        {
            for (var x = 0; x < type.Array.GetLength(0); x++)
            {
                if (position.x + x < 0)
                    continue;

                if (position.x + x >= width)
                    continue;

                if (field[position.y + y][position.x + x] != null && type.Array[y, x] != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool CanRotateLeft(Tetrimino tetrimino)
    {
        Block[,] rotated = RotateBlocksLeft(tetrimino.Blocks, tetrimino.Blocks.GetLength(0));

        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
        {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Position.x + x < 0)
                    return false;

                if (tetrimino.Position.x + x >= width)
                    return false;


                if (tetrimino.Position.y + y < 0 && BlockRowContainsBlock(rotated, 0))
                    return false;

                if (field[tetrimino.Position.y + y][tetrimino.Position.x + x] != null && rotated[y, x] != null)
                    return false;
            }
        }

        return true;
    }

    public bool CanRotateRight(Tetrimino tetrimino)
    {
        Block[,] rotated = RotateBlocksRight(tetrimino.Blocks, tetrimino.Blocks.GetLength(0));

        for (var y = 0; y < tetrimino.Blocks.GetLength(0); y++)
        {
            for (var x = 0; x < tetrimino.Blocks.GetLength(0); x++)
            {
                if (tetrimino.Position.x + x < 0)
                    return false;

                if (tetrimino.Position.x + x >= width)
                    return false;


                if (tetrimino.Position.y + y < 0 && BlockRowContainsBlock(rotated, 0))
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

    public void RotateLeft(Tetrimino tetrimino)
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

    }

    public void RotateRight(Tetrimino tetrimino)
    {
        int n = tetrimino.Blocks.GetLength(0);

        PrintBlockArray(tetrimino.Blocks);

        Block[,] ret = new Block[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i, j] = tetrimino.Blocks[j, n - i - 1];

                if (tetrimino.Blocks[j, n - i - 1] != null)
                {
                    tetrimino.Blocks[j, n - i - 1].Instance.GetComponent<BlockScript>().UpdatePosition(new Vector3((tetrimino.Position.x + j) * spacing, (tetrimino.Position.y + i) * spacing), rotationUpdateSpeed);
                }
            }
        }

        tetrimino.Blocks = ret;

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

                if (tetrimino.Position.y + y < 0)
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

                if (tetrimino.Position.y + y < 0)
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

    public int FindTopRow(bool withStatic)
    {
        for (var y = field.Count; y >= 0; y--)
        {
            if (FieldRowContainsBlock(field, y - 1, withStatic))
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

    public Tetrimino InstantiateTetrimino(Vector2Int pos, TetriminoType type, int rotation, bool staticTetrimino)
    {
        // add empty rows on top of field, to avoid array out of bounds exceptions
        for (var i = 0; i < (pos.y + 30) - field.Count; i++)
            field.Add(EmptyRow());

        int[,] array = RotateNTimes(type.Array, rotation);

        Tetrimino tetrimino = new Tetrimino(rotation, pos, type, staticTetrimino);

        tetrimino.Blocks = new Block[array.GetLength(0), array.GetLength(0)];
        GameObject newTetrimino = new GameObject();
        newTetrimino.transform.SetParent(tetrominosParent.transform);
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
       Debug.Log("insert_tetrimino(" + tetrimino.Position.x + ", " + tetrimino.Position.y + ")");
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
    public int wallHeightOffset = 7;

    public void UpdateCamera(Vector3 pos)
    {
        iTween.MoveTo(Camera.main.gameObject, pos, 2);
    }

    public void SpawnRandomTetrimino()
    {
        TetriminoType type = TetriminoType.I();
        Vector2Int position = new Vector2Int(Random.Range(4, width - type.Array.GetLength(0)), FindTopRow(false) + 10);

        Debug.Log("spawn_random_tetrimino(" + type.Name + ", (" + position.x + ", " + position.y + ")");

        if (!CanPlaceOnField(type, position))
        {
            return;
        }

        var tetrimino = InstantiateTetrimino(position, type , 0, true);
        InsertTetrimino(tetrimino);
    }

    public void InsertStaticNumberWalls()
    {
        int topRow = FindTopRow(false) + 20;

        if (lastInsertedStaticNumberWallHeight < topRow)
        {
            for (var row = lastInsertedStaticNumberWallHeight; row < topRow; row++)
            {
                if (row % staticNumberWallOffset == 0)
                {
                    var newSeparetor = new GameObject();
                    newSeparetor.name = "separator_" + row;

                    for (var a = -1; a < width + 20; a++)
                    {
                        var spawnPos = new Vector3((a * spacing) -10, (row - 1) * spacing, 1);
                        var wallBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
                        wallBlock.transform.localScale = new Vector3(1, 0.1f, 0.1f);
                        wallBlock.transform.SetParent(newSeparetor.transform);
                        wallBlock.GetComponent<Renderer>().material = matWall;
                        wallBlock.GetComponent<BlockScript>().UpdatePosition(spawnPos, 3f);
                    }

                    newSeparetor.transform.SetParent(levelBlocksParent.transform);

                    var number = DigitType.InstantiateNumber(row, new Vector3(-4, row - 0.5f, 0), blockPrefab);
                    number.transform.SetParent(numbersParent.transform);
                    number.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    number.transform.localRotation = Quaternion.Euler(new Vector3(0, -10f, 0));

                    lastInsertedStaticNumberWallHeight = topRow;
                }
            }
        }
    }

    void InsertWalls()
    {
        int topRow = FindTopRow(false) + wallHeightOffset;

        if (wallHeight < topRow)
        {
            for (var i = 0; i < topRow - wallHeight; i++)
            {
                var spawnPos = new Vector3(-1, (topRow - i + 5) * spacing, 0);
                var destPos = new Vector3(-1, (topRow - i - 2) * spacing, 0);
                var wallBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
                wallBlock.transform.SetParent(wallsParent.transform);
                wallBlock.GetComponent<Renderer>().material = matWall;
                wallBlock.GetComponent<BlockScript>().UpdatePosition(destPos, 3f);

                spawnPos = new Vector3((width + 1), (topRow - i + 5) * spacing, 0);
                destPos = new Vector3((width + 1), (topRow - i - 2) * spacing, 0);
                wallBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
                wallBlock.transform.SetParent(wallsParent.transform);
                wallBlock.GetComponent<Renderer>().material = matWall;
                wallBlock.GetComponent<BlockScript>().UpdatePosition(destPos, 3f);

            }
            wallHeight = topRow;
        }
    }

    public void Update()
    {
        if (gameStop)
            return;

        float newScore = Mathf.Round(player.transform.position.y * 100) / 100;
        if (newScore > score)
        {
            score = newScore;
            if (score > 0)
                speed = Mathf.Pow(score, speedIncreaseFactor);
        }

        InsertStaticNumberWalls();
        InsertWalls();

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
            if (CanRotateLeft(currentTetrimino))
            {
                RotateLeft(currentTetrimino);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (CanRotateRight(currentTetrimino))
            {
                RotateRight(currentTetrimino);
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
                    UpdateCamera(new Vector3(5, FindTopRow(false) * spacing + 2, -10));

                    currentTetrimino = InstantiateTetrimino(new Vector2Int(0, FindTopRow(false) + 4), TetriminoType.Random(), 0, false);
                }
            }
            period = 0;
        }

        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (!GeometryUtility.TestPlanesAABB(frustumPlanes, player.GetComponent<Collider>().bounds))
        {
            GameOver();
            ui.GetComponent<UIScript>().Show(score, best);
        }

        period += Time.deltaTime;
    }
}
