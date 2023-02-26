using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInput : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float swipeThreshold = 50f;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    private ICommand swipeCommand;
    private Camera mainCamera;
    private BoardManager boardManager;

    private void Start()
    {
        mainCamera = Camera.main;
        boardManager = BoardManager.instance;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null && eventData.pointerEnter.tag == "Drop" && eventData.pointerEnter.GetComponent<Drop>().isMovable)
        {
            Vector2 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            eventData.pointerEnter.transform.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null && eventData.pointerEnter.tag == "Drop" && eventData.pointerEnter.GetComponent<Drop>().isMovable)
        {
            endTouchPos = eventData.position;
            Vector2 swipeDirection = endTouchPos - startTouchPos;
            float swipeMagnitude = swipeDirection.magnitude;

            if (swipeMagnitude > swipeThreshold)
            {
                int col = eventData.pointerEnter.GetComponent<Drop>().col;
                int row = eventData.pointerEnter.GetComponent<Drop>().row;

                if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                {
                    if (swipeDirection.x > 0f && col < boardManager.numCols - 1)
                    {
                        swipeCommand = new SwipeCommand(boardManager, row, col, row, col + 1);
                        swipeCommand.Execute();
                    }
                    else if (swipeDirection.x < 0f && col > 0)
                    {
                        swipeCommand = new SwipeCommand(boardManager, row, col, row, col - 1);
                        swipeCommand.Execute();
                    }
                }
                else
                {
                    if (swipeDirection.y > 0f && row < boardManager.numRows - 1)
                    {
                        swipeCommand = new SwipeCommand(boardManager, row, col, row + 1, col);
                        swipeCommand.Execute();
                    }
                    else if (swipeDirection.y < 0f && row > 0)
                    {
                        swipeCommand = new SwipeCommand(boardManager, row, col, row - 1, col);
                        swipeCommand.Execute();
                    }
                }
            }
            else
            {
                eventData.pointerEnter.GetComponent<Drop>().ReturnToStartPosition();
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
