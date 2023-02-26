using System.Collections;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public int row;
    public int col;
    public int type;
    public bool isMovable = true;
    public SpriteRenderer spriteRenderer;
    
    private Vector2 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    public void ReturnToStartPosition()
    {
        isMovable = false;
        StartCoroutine(MoveToPosition(startPosition));
    }

    private IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            transform.position = Vector2.Lerp(transform.position, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
        isMovable = true;
    }
    
    public void SetDrop(int newRow, int newCol, int newType, Sprite newSprite)
    {
        this.name = newSprite.name;
        row = newRow;
        col = newCol;
        type = newType;
        spriteRenderer.sprite = newSprite;
    }
}