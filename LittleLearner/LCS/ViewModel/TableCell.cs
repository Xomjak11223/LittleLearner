using CommunityToolkit.Mvvm.ComponentModel;

namespace LittleLearner.LCS.ViewModel
{
    public partial class TableCell : ObservableObject
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Text {  get; set; }
        public bool IsWritable { get; set; }
        public Color CellColor { get; set; }
        public string Type { get; set; }

        public bool SolutionVisible { get; set; }

        public string SolutinValue { get; set; }
        public string SolutionType { get; set; }
        public string AlternativeRepresentation { get; set; }

        public TableCell(int row, int column, Color cellColor, string text) 
        {
            Row = row;
            Column = column;
            CellColor = cellColor;
            Text = text;
            IsWritable = true;
            SolutionVisible = false;
            SolutinValue = "";
            SolutionType = "";
            AlternativeRepresentation = "";
        }

        public TableCell(int row, int column, Color cellColor, string text, bool isWritable)
        {
            Row = row;
            Column = column;
            CellColor = cellColor;
            Text = text;
            IsWritable = isWritable;
            SolutionVisible = false;
            SolutinValue = "";
            SolutionType = "";
            AlternativeRepresentation = "";
        }
    }
}
