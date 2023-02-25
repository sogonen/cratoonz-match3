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

    void Awake()
    {
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
        int[,] boardGraph = new int[numRows, numCols];

        // Fill the board graph using a backtracking algorithm
        FillBoardBacktrack(boardGraph, 0, 0);

        // Assign the board graph to the tiles
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                GameObject tile = tiles[row, col];
                int dropType = boardGraph[row, col];
                GameObject drop = dropPool.GetDrop(dropType);
                drop.transform.SetParent(tiles[row, col].transform);
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
}