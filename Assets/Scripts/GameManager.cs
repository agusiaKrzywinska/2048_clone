using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]
    private Camera mainCam;
    [Header("Tile Settings")]
    [SerializeField]
    private Tile tilePrefab;
    public Color32[] tileColors;
    public float tileSpeed = 1f;

    private const int winTile = 2048;
    private readonly int[] spawnTiles = new int[] {2, 4};

    [Header("Grid Settings")]
    [SerializeField]
    private Vector2Int gridSize;
    [SerializeField]
    private int startingNodes = 1;

    [Header("UI")]
    [SerializeField]
    private Transform board;
    [SerializeField]
    private TMPro.TextMeshProUGUI scoreTxt;
    public GameObject win;
    public GameObject undoBtn;

    public bool acceptInput = true;

    private int totalScore = 0;
    private int TotalScore
    {
        get { return totalScore; }
        set 
        { 
            totalScore = value;
            scoreTxt.text = totalScore.ToString();
        }
    }
    private Tile[,] grid;

    private int[,] undoGrid;
    private int pastScore;

    private void Awake()
    {
        Instance = this;
    }

    private void OnValidate()
    {
        //ensuring that grid size is always proper
        gridSize = Vector2Int.Max(Vector2Int.one, gridSize);

        //ensuring there is always at least one starting node
        startingNodes = Mathf.Clamp(startingNodes, 1, gridSize.x*gridSize.y);
    }

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (!acceptInput) return;

        if(Input.GetKeyDown(KeyCode.UpArrow) && CanMoveY())
        {
            SaveLastMove();
            MoveTilesUp();
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow) && CanMoveY())
        {
            SaveLastMove();
            MoveTilesDown();
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow) && CanMoveX())
        {
            SaveLastMove();
            MoveTilesRight();
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow) && CanMoveX())
        {
            SaveLastMove();
            MoveTilesLeft();
        }
    }

    private void SaveLastMove()
    {
        undoGrid = new int[gridSize.x, gridSize.y];
        pastScore = totalScore;
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                undoGrid[x, y] = grid[x, y] ? grid[x, y].Value : 0;
            }
        }
        undoBtn.SetActive(true);
    }

    private void MoveTilesUp()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            //Get all tiles in a column
            List<Tile> tilesInColumn = new List<Tile>();
            for (int y = gridSize.y-1; y >= 0; y--)
            {
                if (grid[x, y])
                    tilesInColumn.Add(grid[x, y]);
            }

            tilesInColumn = MergeTiles(tilesInColumn);

            //Place back on the grid
            for (int y = gridSize.y-1; y >= 0; y--)
            {
                if (tilesInColumn.Count > 0)
                {
                    grid[x, y] = tilesInColumn[0];
                    tilesInColumn.RemoveAt(0);
                }
                else
                {
                    grid[x, y] = null;
                }

            }
        }

        StartCoroutine(FinishTurn());
    }

    private void MoveTilesDown()
    {
        for(int x = 0; x < gridSize.x; x++)
        {
            //Get all tiles in a column
            List<Tile> tilesInColumn = new List<Tile>();
            for(int y = 0; y < gridSize.y; y++)
            {
                if(grid[x,y])
                    tilesInColumn.Add(grid[x, y]);
            }

            tilesInColumn = MergeTiles(tilesInColumn);

            //Place back on the grid
            for (int y = 0; y < gridSize.y; y++)
            {
                if (tilesInColumn.Count > 0)
                {
                    grid[x, y] = tilesInColumn[0];
                    tilesInColumn.RemoveAt(0);
                }
                else
                {
                    grid[x, y] = null;
                }

            }
        }

        StartCoroutine(FinishTurn());
    }

    private void MoveTilesLeft()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            //Get all tiles in a column
            List<Tile> tilesInRow = new List<Tile>();
            for (int x = 0; x < gridSize.x; x++)
            {
                if (grid[x, y])
                    tilesInRow.Add(grid[x, y]);
            }

            tilesInRow = MergeTiles(tilesInRow);

            //Place back on the grid
            for (int x = 0; x < gridSize.x; x++)
            {
                if (tilesInRow.Count > 0)
                {
                    grid[x, y] = tilesInRow[0];
                    tilesInRow.RemoveAt(0);
                }
                else
                {
                    grid[x, y] = null;
                }

            }
        }

        StartCoroutine(FinishTurn());
    }

    private void MoveTilesRight()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            //Get all tiles in a column
            List<Tile> tilesInRow = new List<Tile>();
            for (int x = gridSize.x - 1; x >= 0; x--)
            {
                if (grid[x, y])
                    tilesInRow.Add(grid[x, y]);
            }

            tilesInRow = MergeTiles(tilesInRow);

            //Place back on the grid
            for (int x = gridSize.x - 1; x >= 0; x--)
            {
                if (tilesInRow.Count > 0)
                {
                    grid[x, y] = tilesInRow[0];
                    tilesInRow.RemoveAt(0);
                }
                else
                {
                    grid[x, y] = null;
                }

            }
        }

        StartCoroutine(FinishTurn());
    }

    private IEnumerator FinishTurn()
    {
        acceptInput = false;
        yield return UpdateVisuals();
        //check if win or lose
        if (GameWon())
        {
            win.SetActive(true);
        }
        else if(!GameLost())
        {
            AddNewTile(GetEmptyTile(), spawnTiles[Random.Range(0, spawnTiles.Length)]);
            acceptInput = true;
        }

    }

    private bool GameWon()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if (grid[x, y] && grid[x, y].Value == winTile)
                    return true;
            }
        }
        return false;
    }

    private bool GameLost()
    {
        return !(CanMoveY() || CanMoveX());   
    }

    private bool CanMoveY()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            int pastTileValue = 0;
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y])
                {
                    if (pastTileValue == grid[x, y].Value)
                        return true;
                    pastTileValue = grid[x, y].Value;
                }
                else
                    return true;
            }
        }

        return false;
    }

    private bool CanMoveX()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            int pastTileValue = 0;
            for (int x = 0; x < gridSize.x; x++)
            {
                if (grid[x, y])
                {
                    if (pastTileValue == grid[x, y].Value)
                        return true;
                    pastTileValue = grid[x, y].Value;
                }
                else
                    return true;
            }
        }

        return false;
    }

    private List<Tile> MergeTiles(List<Tile> tiles)
    {
        int pos = 0;
        //Merge values
        while (pos < tiles.Count - 1)
        {
            if (tiles[pos].Value == tiles[pos + 1].Value)
            {
                int newValue = tiles[pos].Value + tiles[pos].Value;
                tiles[pos].SetupTile(newValue);
                TotalScore += newValue;
                Destroy(tiles[pos + 1].gameObject);
                tiles.RemoveAt(pos + 1);
            }
            else
                pos++;
        }

        return tiles;
    }

    private IEnumerator UpdateVisuals()
    {
        //Start moving all the pieces
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y])
                    grid[x, y].UpdatePosition(x, y);
            }
        }
        //wait until they are all done
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y])
                {
                    while(grid[x,y].isMoving)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }
    }

    public void NewGame()
    {
        //resetting values
        acceptInput = true;
        win.SetActive(false);
        undoBtn.SetActive(false);
        TotalScore = 0;
        //positioning camera to fit the grid. 
        mainCam.transform.position = new Vector3(gridSize.x/2f, gridSize.y/2f, mainCam.transform.position.z);
        mainCam.orthographicSize = Mathf.Max(gridSize.x, gridSize.y);
        board.localScale = new Vector3(gridSize.x, gridSize.y, 1);

        SetupGrid();
    }

    public void Undo()
    {
        //delete past tiles
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y])
                {
                    Destroy(grid[x, y].gameObject);
                }
            }
        }
        grid = new Tile[gridSize.x, gridSize.y];

        //put in new tiles
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if(undoGrid[x,y] != 0)
                {
                    AddNewTile(new Vector2Int(x, y), undoGrid[x,y]);
                }
            }
        }

        TotalScore = pastScore;
        undoBtn.SetActive(false);

    }

    private void AddNewTile(Vector2Int pos, int value)
    {
        Tile tile = Instantiate(tilePrefab, new Vector3(pos.x, pos.y), Quaternion.identity);
        tile.SetupTile(value);
        grid[pos.x, pos.y] = tile;
    }

    private Vector2Int GetEmptyTile()
    {
        //finds all the possible empty tiles
        List<Vector2Int> emptyTiles = new List<Vector2Int>();
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if(!grid[x,y])
                    emptyTiles.Add(new Vector2Int(x, y));
            }
        }

        //getting random tile position
        Vector2Int randomEmpty = emptyTiles[Random.Range(0, emptyTiles.Count)];
        emptyTiles.Remove(randomEmpty);
        return randomEmpty;
    }

    private void SetupGrid()
    {
        //clearing past grid data
        if (grid != null)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (grid[x, y])
                    {
                        Destroy(grid[x, y].gameObject);
                    }
                }
            }
        }
        grid = new Tile[gridSize.x, gridSize.y];
        
        //Add starting nodes
        for (int i = 0; i < startingNodes; i++)
        {
            AddNewTile(GetEmptyTile(), spawnTiles[Random.Range(0, spawnTiles.Length)]);
        }

    }
}
