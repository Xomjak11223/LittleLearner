using CommunityToolkit.Mvvm.ComponentModel;

namespace LittleLearner.LCS
{
    public partial class Protocol : ObservableObject
    {
        private string[] variables = new string[0];
        private Dictionary<int, string[]> rows = new Dictionary<int, string[]>();

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
                    rows[key] = rows[key].Append("").ToArray<string>();
                }
            }

            // Creates the new row of Labels
            rows.Add(labelNumber, new string[this.variables.Length]);
        }

        public Grid CreateGridTable()
        {
            Grid grid = new Grid();

            Label IdColumn = new Label();
            IdColumn.Background = Color.FromRgba("#003C64");
            IdColumn.Text = "ID";
            IdColumn.HorizontalTextAlignment = TextAlignment.Center;

            grid.AddRowDefinition(new RowDefinition(GridLength.Auto));
            grid.AddColumnDefinition(new ColumnDefinition(GridLength.Auto));
            grid.Add(IdColumn);

            for(int i = 0; i < variables.Count(); i++)
            {
                Label ColumnHeader = new Label();
                ColumnHeader.Text = variables[i];
                ColumnHeader.Background = Color.FromRgba("#003C64");
                ColumnHeader.HorizontalTextAlignment = TextAlignment.Center;

                grid.AddColumnDefinition(new ColumnDefinition(GridLength.Star));
                grid.Add(ColumnHeader, i+1, 0);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                int labelNumber = rows.ElementAt(i).Key;
                string[] row = rows.ElementAt(i).Value;
                Color backgroundColor = i % 2 == 0 ? (Color.FromRgba("#CBCBCB")) : (Color.FromRgba("#E1E1E1"));

                Label labelNumberView = new Label();
                labelNumberView.Text = ("" + labelNumber);
                labelNumberView.Background = backgroundColor;
                labelNumberView.VerticalTextAlignment = TextAlignment.Center;

                grid.AddRowDefinition(new RowDefinition(GridLength.Star));
                grid.Add(labelNumberView, 0, i + 1);

                for(int j = 0; j < row.Length; j++)
                {
                    Editor labelValue = new Editor();
                    labelValue.Background = backgroundColor;

                    grid.Add(labelValue, j + 1, i + 1);
                }
            }

            return grid;
        }
    }
}
