using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isSwiping = false;
    private bool hasMoved = false;

    public float minSwipeDistance = 50.0f;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    isSwiping = true;
                    hasMoved = false;
                    break;
                case TouchPhase.Moved:
                    if (isSwiping && !hasMoved)
                    {
                        float distance = (touch.position - startPos).magnitude;
                        if (distance > minSwipeDistance)
                        {
                            endPos = touch.position;
                            hasMoved = true;
                        }
                    }
                    break;
                case TouchPhase.Ended:
                    if (isSwiping && hasMoved)
                    {
                        endPos = touch.position;
                        Vector2 swipe = endPos - startPos;
                        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                        {
                            // Horizontal swipe
                            if (swipe.x < 0.0f)
                            {
                                // Left swipe
                                //BoardManager.instance.SwipeLeft();
                            }
                            else
                            {
                                // Right swipe
                                //BoardManager.instance.SwipeRight();
                            }
                        }
                        else
                        {
                            // Vertical swipe
                            if (swipe.y < 0.0f)
                            {
                                // Down swipe
                                //BoardManager.instance.SwipeDown();
                            }
                            else
                            {
                                // Up swipe
                                //BoardManager.instance.SwipeUp();
                            }
                        }
                    }
                    isSwiping = false;
                    break;
            }
        }
    }
}