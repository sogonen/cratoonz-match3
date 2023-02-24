using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    public int numRows = 8;
    public int numCols = 8;
    public float tileSize = 1.0f;
    public DropPool drops;
    public GameObject tilePrefab;
    public GameObject[,] tiles;
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

        // Calculate the width of the camera view area
        float cameraWidth = Camera.main.orthographicSize * 0.3f * Camera.main.aspect;

        // Calculate the width of the board
        float boardWidth = numCols * tileSize;

        // Calculate the horizontal offset to center the board
        float xOffset = (cameraWidth - boardWidth) / 2.0f;

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.transform.position = new Vector3(col * tileSize + xOffset, row * tileSize, 0.0f);
                tiles[row, col] = tile;
            }
        }
    }

    void FillBoard()
    {
        // Create a list to hold the drop types
        List<int> dropTypes = new List<int>();

        // Add each drop type to the list
        for (int i = 0; i < drops.dropSprites.Length; i++)
        {
            dropTypes.Add(i);
        }

        // Shuffle the list of drop types
        for (int i = 0; i < dropTypes.Count; i++)
        {
            int temp = dropTypes[i];
            int randomIndex = Random.Range(i, dropTypes.Count);
            dropTypes[i] = dropTypes[randomIndex];
            dropTypes[randomIndex] = temp;
        }

        dropsList = new List<GameObject>();
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                int dropType = -1;

                // Keep selecting a random drop type until it doesn't match any of its neighbors
                while (dropType == -1 || (col >= 2 && IsMatching(dropType, row, col - 1, row, col - 2)) || (row >= 2 && IsMatching(dropType, row - 1, col, row - 2, col)))
                {
                    dropType = dropTypes[Random.Range(0, dropTypes.Count)];
                }

                // Get a drop object from the pool and set its properties
                GameObject dropObject = drops.GetDrop(dropType);
                var drop = dropObject.GetComponent<Drop>();
                drop.SetDrop(row, col, dropType, drops.dropSprites[dropType]);
                Vector3 position = tiles[numRows - 1, col].transform.position + Vector3.up * 5.0f;
                position.z = -1.0f;
                drop.transform.position = position;

                // Add the new drop to the drops list
                dropsList.Add(dropObject);
                
            }
        }
        
        DropAnimations();
    }

    void DropAnimations()
    {
        for (int col = 0; col < numCols; col++)
        {
            int numEmpty = 0;
            for (int row = 0; row < numRows; row++)
            {
                if (tiles[row, col].GetComponentInChildren<SpriteRenderer>().sprite == null)
                {
                    numEmpty++;
                }
                else if (numEmpty > 0)
                {
                    // Get the drop at the current position
                    GameObject drop = GetDropAt(row, col);

                    // Calculate the new row position
                    int newRow = row - numEmpty;

                    // Calculate the new position for the drop
                    Vector3 newPosition = tiles[newRow, col].transform.position;
                    newPosition.z = -1.0f;

                    // Animate the drop falling into place
                    drop.transform.DOMove(newPosition, 0.2f * numEmpty).SetEase(Ease.InOutCubic);

                    // Update the tile at the new position to match the drop
                    tiles[newRow, col].GetComponentInChildren<SpriteRenderer>().sprite = drop.GetComponent<SpriteRenderer>().sprite;

                    // Clear the tile at the old position
                    tiles[row, col].GetComponentInChildren<SpriteRenderer>().sprite = null;
                }
            }
        }
    }
    
    GameObject GetDropAt(int row, int col)
    {
        if (row < 0 || row >= numRows || col < 0 || col >= numCols)
        {
            return null;
        }

        foreach (GameObject drop in dropsList)
        {
            if (drop.GetComponent<Drop>().row == row && drop.GetComponent<Drop>().col == col)
            {
                return drop;
            }
        }

        return null;
    }

    bool HasMatches()
    {
        foreach (GameObject drop in dropsList)
        {
            int row = drop.GetComponent<Drop>().row;
            int col = drop.GetComponent<Drop>().col;
            if (MatchesAt(row, col, drop)) return true;
        }

        return false;
    }
    
    bool IsMatching(int dropType, int row1, int col1, int row2, int col2)
    {
        // Check if the specified positions are within the bounds of the board
        if (row1 < 0 || row1 >= numRows || col1 < 0 || col1 >= numCols || row2 < 0 || row2 >= numRows || col2 < 0 || col2 >= numCols)
        {
            return false;
        }

        // Check if the specified drops match the specified drop type
        return tiles[row1, col1].GetComponentInChildren<SpriteRenderer>().sprite == drops.dropSprites[dropType] &&
               tiles[row2, col2].GetComponentInChildren<SpriteRenderer>().sprite == drops.dropSprites[dropType];
    }

    void RemoveMatches()
    {
        foreach (GameObject drop in dropsList.ToArray())
        {
            int row = drop.GetComponent<Drop>().row;
            int col = drop.GetComponent<Drop>().col;
            if (MatchesAt(row, col, drop))
            {
                dropsList.Remove(drop);
                tiles[row, col] = null;
                drop.GetComponent<Drop>().ScaleAndDestroy();
            }
        }
    }

    public void FillEmptyTiles()
    {
        for (int col = 0; col < numCols; col++)
        {
            for (int row = 0; row < numRows; row++)
            {
                if (tiles[row, col] == null)
                {
                    GameObject drop = dropsList[Random.Range(0, dropsList.Count)];
                    drop.GetComponent<Drop>().MoveTo(row, col);
                    drop.GetComponent<Drop>().row = row;
                    drop.GetComponent<Drop>().col = col;
                    tiles[row, col] = drop;
                    dropsList.Remove(drop);
                }
            }
        }
    }

    bool MatchesAt(int row, int col, GameObject drop)
    {
        Sprite dropSprite = drop.GetComponent<SpriteRenderer>().sprite;

        // Check for horizontal matches
        int count = 1;
        for (int i = col - 1; i >= 0; i--)
        {
            GameObject tile = tiles[row, i];
            if (tile == null) break;
            Sprite sprite = tile.GetComponentInChildren<SpriteRenderer>()?.sprite;
            if (sprite == null || sprite != dropSprite) break;
            count++;
        }

        for (int i = col + 1; i < numCols; i++)
        {
            GameObject tile = tiles[row, i];
            if (tile == null) break;
            Sprite sprite = tile.GetComponentInChildren<SpriteRenderer>()?.sprite;
            if (sprite == null || sprite != dropSprite) break;
            count++;
        }

        if (count >= 3) return true;

        // Check for vertical matches
        count = 1;
        for (int i = row - 1; i >= 0; i--)
        {
            GameObject tile = tiles[i, col];
            if (tile == null) break;
            Sprite sprite = tile.GetComponentInChildren<SpriteRenderer>()?.sprite;
            if (sprite == null || sprite != dropSprite) break;
            count++;
        }

        for (int i = row + 1; i < numRows; i++)
        {
            GameObject tile = tiles[i, col];
            if (tile == null) break;
            Sprite sprite = tile.GetComponentInChildren<SpriteRenderer>()?.sprite;
            if (sprite == null || sprite != dropSprite) break;
            count++;
        }

        if (count >= 3) return true;

        // No match found
        return false;
    }
}