using System;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    public float minSwipeDistance = 20f;

    private Vector2 swipeStartPosition;
    private Vector2 swipeEndPosition;
    private bool isSwiping = false;
    
    private BoardManager board;

    private void Start()
    {
        board = BoardManager.instance;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                swipeStartPosition = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                swipeEndPosition = touch.position;

                // Check if the swipe distance is greater than the minimum distance
                float distance = Vector2.Distance(swipeStartPosition, swipeEndPosition);
                if (distance >= minSwipeDistance)
                {
                    // Calculate the swipe direction
                    Vector2 direction = swipeEndPosition - swipeStartPosition;
                    direction.Normalize();

                    // Check if the swipe direction is valid
                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    {
                        // Swipe left or right
                        int col1 = (int)Mathf.Round(swipeStartPosition.x / Screen.width * board.numCols);
                        int col2 = (int)Mathf.Round(swipeEndPosition.x / Screen.width * board.numCols);

                        if (Mathf.Abs(col1 - col2) == 1)
                        {
                            int row = (int)Mathf.Round(swipeStartPosition.y / Screen.height * board.numRows);
                            board.SwapDrops(row, col1, row, col2);
                        }
                    }
                    else
                    {
                        // Swipe up or down
                        int row1 = (int)Mathf.Round(swipeStartPosition.y / Screen.height * board.numRows);
                        int row2 = (int)Mathf.Round(swipeEndPosition.y / Screen.height * board.numRows);

                        if (Mathf.Abs(row1 - row2) == 1)
                        {
                            int col = (int)Mathf.Round(swipeStartPosition.x / Screen.width * board.numCols);
                            board.SwapDrops(row1, col, row2, col);
                        }
                    }

                    isSwiping = false;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isSwiping = false;
            }
        }
    }

}