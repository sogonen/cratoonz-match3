using UnityEngine;

public class Tile : MonoBehaviour
{
    private int row;
    private int col;

    public void SetRowAndCol(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public int GetRow()
    {
        return row;
    }

    public int GetCol()
    {
        return col;
    }
}