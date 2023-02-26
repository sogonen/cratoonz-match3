public class SwipeCommand : ICommand
{
    private BoardManager boardManager;
    private int startRow;
    private int startCol;
    private int endRow;
    private int endCol;

    public SwipeCommand(BoardManager boardManager, int startRow, int startCol, int endRow, int endCol)
    {
        this.boardManager = boardManager;
        this.startRow = startRow;
        this.startCol = startCol;
        this.endRow = endRow;
        this.endCol = endCol;
    }

    public void Execute()
    {
        boardManager.SwapDrops(startRow, startCol, endRow, endCol);
    }

    public void Undo()
    {
        boardManager.SwapDrops(endRow, endCol, startRow, startCol);
    }
}