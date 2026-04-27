using CommunityToolkit.Mvvm.ComponentModel;
using LCS_FillProtocol.TaskDeclaration;
using System.Collections.ObjectModel;

namespace LCS_FillProtocol.Table
{
    public partial class TableViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<TableCell> cells;

        [ObservableProperty]
        public ColumnDefinitionCollection columnDefinitions;

        [ObservableProperty]
        public RowDefinitionCollection rowDefinitions;

        [ObservableProperty]
        public string taskTitle;

        public string taskCode;

        public static readonly Color headerColor = Color.FromRgba("#095982");
        public static readonly Color primaryRowColor = Color.FromRgba("#CBCBCB");
        public static readonly Color secondaryRowColor = Color.FromRgba("#E1E1E1");
        public static readonly Color notExistingRowColor = Color.FromRgba("#222222");

        public static readonly int columnLengt = 120;
        public static readonly int rowHeight = 35;

        public string[] tableHead;
        public TableCell[][] tableBody;   //row -> column

        public TableViewModel()
        {
            Cells = new ObservableCollection<TableCell>();
            ColumnDefinitions = new ColumnDefinitionCollection();
            RowDefinitions = new RowDefinitionCollection();
            TaskTitle = "";
            taskCode = "";

            tableHead = [];
            tableBody = [[]];
        }

        public void InitializeFromTask(TaskInput task)
        {
            if (task == null) return;

            Cells.Clear();

            tableHead = [];
            tableBody = [];
            int colorSwapper = -1;

            if (string.IsNullOrEmpty(task.Name)) TaskTitle = "New Task";
            else TaskTitle = task.Name;

            if (!string.IsNullOrEmpty(task.Code)) taskCode = task.Code;

            if (task.Protokol == null) return;
            if (task.Protokol.Entrys == null || task.Protokol.Entrys?.Length == 0) return;

            foreach (TaskDeclaration.Label label in task.Protokol.Entrys)
            {
                string[] currentLabelVriables = [];
                if(label.VarEntrys != null)
                {
                    // Appends every variable to the current variable list and the global variable list
                    foreach (LabelEntry variable in label.VarEntrys)
                    {
                        if (!string.IsNullOrEmpty(variable.Name) && !currentLabelVriables.Contains(variable.Name))
                        {
                            currentLabelVriables = currentLabelVriables.Append(variable.Name).ToArray();
                            if (!tableHead.Contains(variable.Name))
                            {
                                tableHead = tableHead.Append(variable.Name).ToArray();
                                for(int i = 0; i < tableBody.Length; i++)
                                {
                                    TableCell[] row = tableBody[i];
                                    tableBody[i] = row.Append(new(i + 1, row.Length, notExistingRowColor, "-", false)).ToArray();
                                }
                            }
                        }
                    }
                }

                colorSwapper++;
                Color rowColor = colorSwapper % 2 == 0 ? (primaryRowColor) : (secondaryRowColor);
                TableCell[] newRow = new TableCell[tableHead.Length + 1];

                newRow[0] = new(tableBody.Length + 1, 0, rowColor, label.Num.ToString(), false);
                for(int i = 0; i < tableHead.Length; i++)
                {
                    if (currentLabelVriables.Contains(tableHead[i])) newRow[i + 1] = new(tableBody.Length + 1, i + 1, rowColor, "");
                    else newRow[i + 1] = new(tableBody.Length + 1, i + 1, notExistingRowColor, "-", false);
                }

                tableBody = tableBody.Append(newRow).ToArray();
            }

            if (tableBody.Length == 0 && tableHead.Length == 0) return;

            ColumnDefinitionCollection columnLengths = new ColumnDefinitionCollection();
            for (int i = 0; i < tableHead.Length + 1; i++) columnLengths.Add(new(columnLengt));
            ColumnDefinitions = columnLengths;

            RowDefinitionCollection rowHeights = new RowDefinitionCollection();
            for (int i = 0; i < tableBody.Length + 1; i++) rowHeights.Add(new(rowHeight));
            RowDefinitions = rowHeights;

            Cells.Add(new(0, 0, headerColor, "Label", false));
            for (int i = 0; i < tableHead.Length; i++) Cells.Add(new(0, i + 1, headerColor, tableHead[i], false));

            foreach (TableCell[] rows in tableBody)
            {
                foreach (TableCell cell in rows) Cells.Add(cell);
            }
        }

        public InputProtokol ExportCurrentTast()
        {
            InputProtokol output = new InputProtokol();
            int maxVarCount = 0;

            output.Points = null;
            output.ProtocolLabelOrVarMismatch = false;
            output.ProtocolOrLabelMismatchMessage = "";

            output.Entrys = [];
            foreach (TableCell[] row in tableBody)
            {
                int rowVars = 0;
                TaskDeclaration.Label label = new();
                label.Num = int.Parse(row[0].Text);
                label.VarEntrys = [];


                for(int i = 1; i < row.Length; i++)
                {
                    TableCell column = row[i];
                    LabelEntry entry = new();

                    entry.Index = i - 1;
                    entry.Name = tableHead[i - 1];
                    entry.Type = "";
                    entry.Value = column.Text;
                    entry.ValueRepresentation = "";
                    entry.Corrected = false;
                    entry.AbsCorrectedType = false;
                    entry.FailedToInclude = false;
                    entry.GotPoint = false;
                    entry.FailedToIncludeMessage = "";
                    entry.HasErrors = false;

                    if (column.IsWritable)
                    {
                        rowVars++;
                        label.VarEntrys = label.VarEntrys.Append(entry).ToArray();
                    }
                }

                if(rowVars > maxVarCount) maxVarCount = rowVars;
                output.Entrys = output.Entrys.Append(label).ToArray();
            }

            output.MaxVarCount = maxVarCount;
            return output;
        }
    }
}
