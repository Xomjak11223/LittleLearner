using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;

namespace LittleLearner.LCS
{
    public partial class Protocol : ObservableObject
    {
        private string[] variables = new string[0];
        private Dictionary<int, String[]> rows = new Dictionary<int, String[]>();
        private static readonly Color primaryRowColor = Color.FromRgba("#CBCBCB");
        private static readonly Color secondaryRowColor = Color.FromRgba("#E1E1E1");
        private static readonly Color notExistingRowColor = Color.FromRgba("#222222");

        public Protocol() { }

        public void AddEmptyLabel(int labelNumber, string[] variables)
        {
            if (rows.ContainsKey(labelNumber)) return;

            // If a new variable gets introduced, it has to be appended to every row
            foreach (string variable in variables)
            {
                if (this.variables.Contains(variable)) continue;

                this.variables = this.variables.Append(variable).ToArray<string>();

                foreach (int key in rows.Keys)
                {
                    rows[key] = rows[key].Append(null).ToArray<String>();
                }
            }

            // Creates the new row of Labels
            String[] newLabel = new string[this.variables.Length];
            for (int i = 0; i < newLabel.Length; i++) { newLabel[i] = ""; }
            rows.Add(labelNumber, newLabel);
        }

        public Grid? CreateGridTable()
        {
            Grid grid = new Grid();
            //frame

            // Template kann nicht erstellt werden, error Template anzeigen
            if (variables.Length == 0 || rows.Count == 0)
            {
                Label ErrorColumn = new Label();
                ErrorColumn.Background = Color.FromRgba("#003C64");
                ErrorColumn.Text = "Aus dem bestehdem Code konnte keine Tabelle erstellt werden";
                ErrorColumn.HorizontalTextAlignment = TextAlignment.Center;
                ErrorColumn.VerticalTextAlignment = TextAlignment.Center;

                grid.Add(ErrorColumn);
                return grid;
            }

            Border IdColumn = new Border
            {
                Stroke = Colors.Black,
                Background = Color.FromRgba("#003C64"),
                StrokeThickness = 1,
                Content = new Label
                {
                    Text = "Label",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black
                }
            };

            grid.AddRowDefinition(new RowDefinition(GridLength.Auto));
            grid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
            grid.Add(IdColumn);

            for(int i = 0; i < variables.Count(); i++)
            {
                Border ColumnHeader = new Border
                {
                    Stroke = Colors.Black,
                    Background = Color.FromRgba("#003C64"),
                    StrokeThickness = 1,
                    Content = new Label
                    {
                        Text = variables[i],
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.Black
                    }
                };

                grid.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
                grid.Add(ColumnHeader, i+1, 0);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                int labelNumber = rows.ElementAt(i).Key;
                string[] row = rows.ElementAt(i).Value;
                Color backgroundColor = i % 2 == 0 ? primaryRowColor : secondaryRowColor;


                Border labelNumberView = new Border
                {
                    Stroke = Colors.Black,
                    Background = backgroundColor,
                    StrokeThickness = 1,
                    Content = new Label
                    {
                        Text = ("" + labelNumber),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.Black
                    }
                };

                grid.AddRowDefinition(new RowDefinition(GridLength.Star));
                grid.Add(labelNumberView, 0, i + 1);

                for(int j = 0; j < row.Length; j++)
                {
                    Border labelValue;

                    if (row[j] == null)
                    {
                        labelValue = new Border
                        {
                            Stroke = Colors.Black,
                            Background = notExistingRowColor,
                            StrokeThickness = 1,
                            Content = new Label
                            {
                                Text = "-",
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Colors.Black
                            }
                        };
                    }
                    else
                    {
                        labelValue = new Border
                        {
                            Stroke = Colors.Black,
                            Background = backgroundColor,
                            StrokeThickness = 1,
                            Content = new Editor
                            {
                                HorizontalTextAlignment = TextAlignment.Center,
                                VerticalTextAlignment = TextAlignment.Center,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Colors.Black
                            }
                        };
                    }

                    grid.Add(labelValue, j + 1, i + 1);
                }
            }

            return grid;
        }

        public void protocolAdapter(string[] input)
        {

        }
    }
}
