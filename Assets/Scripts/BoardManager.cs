using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public int numRows = 8;
    public int numCols = 8;
    public float tileSize = 1.0f;
    public DropPool dropPool;
    public GameObject tilePrefab;
    public GameObject[,] tiles;
    public GameObject[,] drops;
    public Vector2 boardOffset = new Vector2(-3.5f, -3.5f);
    private List<GameObject> dropsList;
    public int[,] boardGraph;
    void Awake()
    {
        int[,] boardGraph = new int[numRows, numCols];
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateBoard();
        FillBoard();
    }

    void CreateBoard()
    {
        tiles = new GameObject[numRows, numCols];
        drops = new GameObject[numRows, numCols];
        // Calculate the width of the camera view area
        float cameraWidth = Camera.main.orthographicSize * 0.2f * Camera.main.aspect;

        // Calculate the width of the board
        float boardWidth = numCols * tileSize;

        // Calculate the horizontal offset to center the board
        float xOffset = (cameraWidth - boardWidth) / 2.0f;

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.position = new Vector3(col * tileSize + xOffset, row * tileSize, 0f);
                tile.name = "Tile (" + row + ", " + col + ")";
                tiles[row, col] = tile;
            }
        }
    }
    
    /* Backtracking algorithm to fill the board */
    void FillBoard()
    {
        boardGraph = new int[numRows, numCols];

        // Fill the board graph using a backtracking algorithm
        FillBoardBacktrack(boardGraph, 0, 0);

        // Assign the board graph to the tiles
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                GameObject tile = tiles[row, col];
                int dropType = boardGraph[row, col];
                GameObject drop = dropPool.GetDrop();

                drop.transform.SetParent(tile.transform);
                drop.transform.localPosition = new Vector3(0, 0, -1);
                drops[row, col] = drop;
                drop.GetComponent<Drop>().SetDrop(row, col, dropType, dropPool.dropSprites[dropType]);
            }
        }
    }
    
    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
    
    bool FillBoardBacktrack(int[,] boardGraph, int row, int col)
    {
        if (row == numRows)
        {
            return true;
        }

        List<int> dropTypes = new List<int>(new int[] { 0, 1, 2, 3 });
        ShuffleList(dropTypes);

        for (int i = 0; i < dropTypes.Count; i++)
        {
            boardGraph[row, col] = dropTypes[i];

            if (!HasHorizontalMatch(boardGraph, row, col) && !HasVerticalMatch(boardGraph, row, col))
            {
                int nextRow = col == numCols - 1 ? row + 1 : row;
                int nextCol = (col + 1) % numCols;

                if (FillBoardBacktrack(boardGraph, nextRow, nextCol))
                {
                    return true;
                }
            }
        }

        boardGraph[row, col] = -1;
        return false;
    }
    
    private GameObject GetDrop(int row, int col)
    {
        if (row < 0 || row >= numRows || col < 0 || col >= numCols)
        {
            return null;
        }

        return drops[row, col];
    }

    bool HasHorizontalMatch(int[,] boardGraph, int row, int col)
    {
        if (col < 2)
        {
            return false;
        }

        int dropType = boardGraph[row, col];
        return dropType == boardGraph[row, col - 1] && dropType == boardGraph[row, col - 2];
    }

    bool HasVerticalMatch(int[,] boardGraph, int row, int col)
    {
        if (row < 2)
        {
            return false;
        }

        int dropType = boardGraph[row, col];
        return dropType == boardGraph[row - 1, col] && dropType == boardGraph[row - 2, col];
    }
    
    public bool MatchesOnBoard()
    {
        // Check for horizontal matches
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols - 2; col++)
            {
                int dropType = boardGraph[row, col];
                if (dropType != -1 && boardGraph[row, col + 1] == dropType && boardGraph[row, col + 2] == dropType)
                {
                    return true;
                }
            }
        }

        // Check for vertical matches
        for (int row = 0; row < numRows - 2; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                int dropType = boardGraph[row, col];
                if (dropType != -1 && boardGraph[row + 1, col] == dropType && boardGraph[row + 2, col] == dropType)
                {
                    return true;
                }
            }
        }

        // No matches found
        return false;
    }
    
    public void RemoveMatches()
    {
        // Create a list to store the removed drops
        List<GameObject> removedDrops = new List<GameObject>();

        // Loop through the board to find matches
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                // Check for horizontal matches
                int dropType = boardGraph[row, col];
                if (dropType != -1 && col < numCols - 2 && boardGraph[row, col + 1] == dropType && boardGraph[row, col + 2] == dropType)
                {
                    // Add the drops to the removedDrops list
                    removedDrops.Add(GetDrop(row, col));
                    removedDrops.Add(GetDrop(row, col + 1));
                    removedDrops.Add(GetDrop(row, col + 2));

                    // Mark the drops as empty on the boardGraph
                    boardGraph[row, col] = -1;
                    boardGraph[row, col + 1] = -1;
                    boardGraph[row, col + 2] = -1;
                }

                // Check for vertical matches
                dropType = boardGraph[row, col];
                if (dropType != -1 && row < numRows - 2 && boardGraph[row + 1, col] == dropType && boardGraph[row + 2, col] == dropType)
                {
                    // Add the drops to the removedDrops list
                    removedDrops.Add(GetDrop(row, col));
                    removedDrops.Add(GetDrop(row + 1, col));
                    removedDrops.Add(GetDrop(row + 2, col));

                    // Mark the drops as empty on the boardGraph
                    boardGraph[row, col] = -1;
                    boardGraph[row + 1, col] = -1;
                    boardGraph[row + 2, col] = -1;
                }
            }
        }

        // Fill the board with new drops
        FillBoard();

        // Return the removed drops to the DropPool
        foreach (GameObject removedDrop in removedDrops)
        {
           dropPool.ReturnDrop(removedDrop);
        }
    }
    
    public void SwapDrops(int row1, int col1, int row2, int col2)
    {
        // Check if the swap is between two non-empty tiles and not between cross tiles
        if (boardGraph[row1, col1] != -1 && boardGraph[row2, col2] != -1 && 
            (Mathf.Abs(row1 - row2) <= 1 && Mathf.Abs(col1 - col2) == 0 || Mathf.Abs(row1 - row2) == 0 && Mathf.Abs(col1 - col2) <= 1))
        {
            // Swap the drops
            int temp = boardGraph[row1, col1];
            boardGraph[row1, col1] = boardGraph[row2, col2];
            boardGraph[row2, col2] = temp;

            Check for matches
            if (MatchesOnBoard())
            {
                RemoveMatches();
            }
            else
            {
                // Invalid move, swap the drops back
                temp = boardGraph[row1, col1];
                boardGraph[row1, col1] = boardGraph[row2, col2];
                boardGraph[row2, col2] = temp;

                // Animate the swap back
                GameObject drop1 = GetDrop(row1, col1);
                GameObject drop2 = GetDrop(row2, col2);
                AnimateSwap(drop1, drop2);
            }
        }
    }
    
    public void AnimateSwap(GameObject drop1, GameObject drop2)
    {
        // Animate the swap back to the original position
        drop1.transform.DOMove(drop2.transform.position, 0.3f);
        drop2.transform.DOMove(drop1.transform.position, 0.3f);
    }
}