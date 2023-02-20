using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject[,] gameBoard; // a 2D array to hold the game board
    public GameObject[] dropPrefabs; // an array of prefabs to represent the different color drops
    public int boardSize = 8; // the size of the game board
    public int minMatches = 3; // the minimum number of matches required to remove tiles
    public int score = 0; // the player's score

    // Start is called before the first frame update
    void Start()
    {
        // initialize the game board
        gameBoard = new GameObject[boardSize, boardSize];

        // randomly populate the game board
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                bool matchFound = true;
                while (matchFound)
                {
                    // randomly assign a drop to the tile
                    int dropIndex = Random.Range(0, dropPrefabs.Length);
                    GameObject dropPrefab = dropPrefabs[dropIndex];
                    GameObject drop = Instantiate(dropPrefab, new Vector3(i, j, 0), Quaternion.identity);
                    gameBoard[i, j] = drop;

                    // check for matches
                    int horizontalMatches = CountMatches(i, j, Vector2.right);
                    int verticalMatches = CountMatches(i, j, Vector2.up);
                    if (horizontalMatches < minMatches && verticalMatches < minMatches)
                    {
                        matchFound = false;
                    }
                    else
                    {
                        // remove the drop and try again
                        Destroy(drop);
                    }
                }
            }
        }
    }

    // count the number of matches in a given direction from a given tile
    private int CountMatches(int x, int y, Vector2 direction)
    {
        int count = 0;
        GameObject drop = gameBoard[x, y];
        if (drop != null)
        {
            while (x >= 0 && x < boardSize && y >= 0 && y < boardSize && gameBoard[x, y] == drop)
            {
                count++;
                x = Mathf.RoundToInt(x + direction.x);
                y = Mathf.RoundToInt(y + direction.y);
            }
        }
        return count;
    }

    // remove a tile from the game board
    public void RemoveTile(int x, int y)
    {
        Destroy(gameBoard[x, y]);
        gameBoard[x, y] = null;
    }

    // add points to the player's score
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score " + score);
        //UIManagerScript.instance.UpdateScore(score);
    }

    // check for matches on the game board
    public void CheckBoard()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                GameObject drop = gameBoard[i, j];
                if (drop != null)
                {
                    // check for horizontal matches
                    if (i < boardSize - 2 && gameBoard[i + 1, j] == drop && gameBoard[i + 2, j] == drop)
                    {
                        RemoveTile(i, j);
                        RemoveTile(i + 1, j);
                        RemoveTile(i + 2, j);
                        AddScore(10);
                    }

                    // check for vertical matches
                    if (j < boardSize - 2 && gameBoard[i, j + 1] == drop && gameBoard[i, j + 2] == drop)
                    {
                        RemoveTile(i, j);
                        RemoveTile(i, j + 1);
                        RemoveTile(i, j + 2);
                        AddScore(10);
                    }
                }
            }
        }
    }
}