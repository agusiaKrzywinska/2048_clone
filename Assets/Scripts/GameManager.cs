using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]
    private Tile tilePrefab;

    private const int winTile = 2048;
    private readonly int[] spawnTiles = new int[] {2, 4};

    [Header("Grid Settings")]
    [SerializeField]
    private Vector2Int gridSize;
    [SerializeField]
    private int startingNodes = 1;

    private int totalScore = 0;
    private Tile[,] grid;

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
        if(Input.GetKeyDown(KeyCode.UpArrow) && CanMoveY())
        {
            MoveTilesUp();
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow) && CanMoveY())
        {
            MoveTilesDown();
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow) && CanMoveX())
        {
            MoveTilesRight();
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow) && CanMoveX())
        {
            MoveTilesLeft();
        }
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

        FinishTurn();
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

        FinishTurn();
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

        FinishTurn();
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

        FinishTurn();
    }

    private void FinishTurn()
    {
        UpdateVisuals();
        //check if win or lose
        if (GameWon())
        {

        }
        else if(GameLost())
        {
            
        }
        else
        {
            AddNewTile();
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
                totalScore += newValue;
                Destroy(tiles[pos + 1].gameObject);
                tiles.RemoveAt(pos + 1);
            }
            else
                pos++;
        }

        return tiles;
    }

    private void UpdateVisuals()
    {
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y])
                    grid[x, y].UpdatePosition(x, y);
            }
        }
    }

    public void NewGame()
    {
        SetupGrid();
    }

    private void AddNewTile()
    {
        Vector2Int pos = GetEmptyTile();
        int value = spawnTiles[Random.Range(0, spawnTiles.Length)];
        Tile tile = Instantiate(tilePrefab, new Vector3(pos.x, pos.y), Quaternion.identity);
        tile.SetupTile(value);
        grid[pos.x, pos.y] = tile;
    }

    private Vector2Int GetEmptyTile()
    {
        List<Vector2Int> emptyTiles = new List<Vector2Int>();
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if(!grid[x,y])
                    emptyTiles.Add(new Vector2Int(x, y));
            }
        }

        Vector2Int randomEmpty = emptyTiles[Random.Range(0, emptyTiles.Count)];
        emptyTiles.Remove(randomEmpty);
        return randomEmpty;
    }

    private void SetupGrid()
    {
        grid = new Tile[gridSize.x, gridSize.y];

        for (int i = 0; i < startingNodes; i++)
        {
            AddNewTile();
        }

        Print();
    }

    private void Print()
    {
        string result = "\n";
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                if (grid[x, y])
                    result += $"'{grid[x, y].Value}'";
                else
                    result += "'0'";
            }
            result += "\n";
        }
    }

}
