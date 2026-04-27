using Antlr4.Runtime;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LimitCSolver.LimitCInterpreter;
using LimitCSolver.LimitCInterpreter.Memory;
using LimitCSolver.LimitCInterpreter.Parser;
using LimitCSolver.LimitCInterpreter.SubTypes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace LittleLearner.LCS.ViewModel
{
    public partial class TableViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<TableCell> cells;

        [ObservableProperty]
        public ColumnDefinitionCollection columnDefinitions;

        [ObservableProperty]
        public RowDefinitionCollection rowDefinitions;

        public static readonly Color headerColor = Color.FromRgba("#095982");
        public static readonly Color primaryRowColor = Color.FromRgba("#CBCBCB");
        public static readonly Color secondaryRowColor = Color.FromRgba("#E1E1E1");
        public static readonly Color notExistingRowColor = Color.FromRgba("#222222");

        public static readonly int columnLengt = 120;
        public static readonly int rowHeight = 50;

        public string[] tableHead;
        public TableCell[][] tableBody;   //row -> column
        public VariableSolution[][] solution;   //row -> column
        private string currentCode;

        public TableViewModel()
        {
            Cells = new ObservableCollection<TableCell>();
            ColumnDefinitions = new ColumnDefinitionCollection();
            RowDefinitions = new RowDefinitionCollection();

            currentCode = "";
            tableHead = [];
            tableBody = [];
            solution = [];
        }

        [RelayCommand]
        public void ToggleSolutionVisibility()
        {
            foreach(TableCell cell in Cells) { cell.SolutionVisible = !cell.SolutionVisible; }
        }

        [RelayCommand]
        public void CompareEntrys()
        {

        }

        public void InitializeTableFromCode(string code)
        {
            // newProtocol is the variable that holds all the lables
            Protocol newProtocol = new();
            var interpreter = new LimitCInterpreter();
            int colorSwap = -1;

            Cells.Clear();
            ColumnDefinitions.Clear();
            RowDefinitions.Clear();
            currentCode = "";
            tableHead = [];
            tableBody = [];

            var program = parse(code);
            if (program == null) return; // an error occured during tree build, we cannot continue safely
            currentCode = code;

            // This code gets executed after interpreter.evaluate() and is called, whenever a Label in the code is reached
            interpreter.LabelCheckPointReached += (sender, args) =>
            {
                string[] variables = [];
                colorSwap++;
                //var npe = new ProtocolEntryViewModel() { Num =  };

                foreach (var (name, addr) in args.VisibleVars)
                {
                    TypedValue memVal = args.MemoryStorage.Memory[addr];
                    var p = new string('*', memVal.Type.Count(c => c == '*'));

                    //npe.VarEntrys.Add(new VarViewModel($"{p}{name}", "", "", ""));

                    variables = variables.Append(p + name).ToArray<string>();

                    if (!tableHead.Contains(p + name))
                    {
                        tableHead = tableHead.Append(p + name).ToArray<string>();
                        for(int i = 0; i < tableBody.Length; i++)
                        {
                            TableCell[] row = tableBody[i];
                            tableBody[i] = row.Append(new(0, row.Length, notExistingRowColor, "-", false)).ToArray();
                        }
                    }
                }

                TableCell[] newRow = new TableCell[tableHead.Length + 1];
                Color rowColor = colorSwap % 2 == 0 ? (primaryRowColor) : (secondaryRowColor);
                newRow[0] = new(tableBody.Length, 0, rowColor, args.LabelNum.ToString(), false);
                for (int i = 1; i < newRow.Length; i++) 
                {
                    if (variables.Contains(tableHead[i-1])) { newRow[i] = new(tableBody.Length - 1, i, rowColor, " "); }
                    else { newRow[i] = new(tableBody.Length - 1, i, rowColor, " ", false); }
                }

                tableBody = tableBody.Append(newRow).ToArray();
            };

            interpreter.evaluate(program);

            // Generates Solution and adds it to each writable cell
            generateSolution();
            for (int i = 0; i < solution.Length; i++)
            {
                VariableSolution[] row = solution[i];
                foreach (VariableSolution column in row)
                {
                    int variableIndex = tableHead.IndexOf(column.Name);
                    if(variableIndex == -1) { throw new Exception("I solution Variable was found, that was not present in the creation Prozess"); }
                    tableBody[i][variableIndex + 1].SolutinValue = column.Value;
                    tableBody[i][variableIndex + 1].SolutionType = column.Type;
                    tableBody[i][variableIndex + 1].AlternativeRepresentation = column.ValueRepresentation;
                }
            }

            // Code after interpretation, adds every row, column and header to the observable Object
            for (int i = 0; i < tableHead.Length + 1; i++) ColumnDefinitions.Add(new(columnLengt));
            for (int i = 0; i < tableBody.Length + 1; i++) RowDefinitions.Add(new(rowHeight));

            Cells.Add(new(0, 0, headerColor, "Label", false));
            for (int i = 0; i < tableHead.Length; i++) Cells.Add(new(0, i + 1, headerColor, tableHead[i], false));

            for (int i = 0; i < tableBody.Length; i++)
            {
                for (int j = 0; j < tableBody[i].Length; j++)
                {
                    TableCell cell = tableBody[i][j];
                    Cells.Add(new(i + 1, j, cell.CellColor, cell.Text, cell.IsWritable));
                }
            }
        }

        private void generateSolution()
        {
            if (string.IsNullOrWhiteSpace(currentCode)) return;

            solution = [];

            var program = parse(currentCode);
            if (program == null) return; // an error occured during tree build, we cannot continue safely

            var interpreter = new LimitCInterpreter();

            interpreter.LabelCheckPointReached += VisitorOnLabelCheckPointReachedCreateSolution;
            interpreter.evaluate(program);
        }

        private static LimitCParser.ProgContext? parse(string code)
        {
            var inputStream = new AntlrInputStream(code);
            var lexer = new LimitCLexer(inputStream);
            var tkStream = new CommonTokenStream(lexer);
            var parser = LimitCParser.Instance(tkStream);

            parser.AddErrorListener(new DiagnosticErrorListener());

            var limitCContext = parser.prog();

            if (parser.Errors.Count != 0)
                return null; // This is dirty. Errors should be handled and displayed to the user.
            else
                return limitCContext;
        }

        private void VisitorOnLabelCheckPointReachedCreateSolution(object? sender, LabelCheckPointEventArgs e)
        {
            if (solution == null) solution = [];

            var vars = e.VisibleVars;

            VariableSolution[] newRow = [];

            foreach (var (name, addr) in vars)
            {
                if (!e.MemoryStorage.Memory.ContainsKey(addr))
                {
                    throw new($"Das hätte nicht passieren dürfen! Es wurde eine Adresse übergeben, welche keinen entsprechenden Speichereintrag besitzt! varName: {name} varAddr: {addr}");
                }

                TypedValue typedValue = e.MemoryStorage.Memory[addr];

                var valueString = typedValue.Value;

                var indirectionsCount = typedValue.Type.Count(c => c == '*');

                if (typedValue.Type.Contains('*'))
                {
                    if (valueString != null)
                    {
                        TypedValue? nextValue = null;
                        if (e.MemoryStorage.Memory.ContainsKey((int)valueString))
                        {
                            nextValue = e.MemoryStorage.Memory[(int)valueString];
                            for (int i = 1; i < indirectionsCount; i++) // Vorgang für Zeigertiefe {p.Length} wiederholen
                            {
                                if (nextValue.Value == null || !e.MemoryStorage.Memory.ContainsKey((int)nextValue.Value))
                                {
                                    nextValue = null;
                                    break;
                                }
                                nextValue = e.MemoryStorage.Memory[(int)nextValue.Value];
                            }

                        }

                        valueString = nextValue?.Value ?? null;
                    }

                }

                string protocolValue = "";
                string alternativeRepresentation = "";
                if (valueString is int intValue)
                {
                    protocolValue = intValue.ToString();
                    if (typedValue.Type.Contains("char"))
                    {
                        alternativeRepresentation = ((char)intValue).ToString();
                    }
                }
                else if (valueString is double doubleValue)
                {
                    protocolValue = doubleValue.ToString("F2", CultureInfo.InvariantCulture);
                }
                else if (valueString is null)
                {
                    protocolValue = "NULL";
                }

                var pointerChain = new string('*', indirectionsCount);

                newRow = newRow.Append(new($"{pointerChain}{name}", typedValue.Type, protocolValue, alternativeRepresentation)).ToArray();
            }

            solution = solution.Append(newRow).ToArray();
        }
    }
}
