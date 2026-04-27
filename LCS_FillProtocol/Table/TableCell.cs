namespace LCS_FillProtocol.Table
{
    public class TableCell
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Text {  get; set; }
        public bool IsWritable { get; set; }
        public Color CellColor { get; set; }

        public TableCell(int row, int column, Color cellColor, string text) 
        {
            Row = row;
            Column = column;
            CellColor = cellColor;
            Text = text;
            IsWritable = true;
        }

        public TableCell(int row, int column, Color cellColor, string text, bool isWritable)
        {
            Row = row;
            Column = column;
            CellColor = cellColor;
            Text = text;
            IsWritable = isWritable;
        }
    }
}
