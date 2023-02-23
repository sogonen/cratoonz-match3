using UnityEngine;
using UnityEngine.Serialization;

public class Drop : MonoBehaviour
{
    public int row;
    public int col;
    public int type;
    public int targetRow;
    public int targetCol;
    public float moveSpeed = 10.0f;
    public SpriteRenderer spriteRenderer;
    
    private bool isMoving;

    void Update()
    {
        if (isMoving)
        {
            Vector3 targetPos = new Vector3(targetCol * BoardManager.instance.tileSize, targetRow * BoardManager.instance.tileSize, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            if (transform.position == targetPos)
            {
                isMoving = false;
                BoardManager.instance.FillEmptyTiles();
            }
        }
    }

    public void MoveTo(int newRow, int newCol)
    {
        if (!isMoving)
        {
            targetRow = newRow;
            targetCol = newCol;
            isMoving = true;
        }
    }
    
    public void SetDrop(int newRow, int newCol, int newType, Sprite newSprite)
    {
        this.name = newSprite.name;
        row = newRow;
        col = newCol;
        type = newType;
        spriteRenderer.sprite = newSprite;
    }

    public void ScaleAndDestroy()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Destroy(gameObject, 0.2f);
    }
}
