using CfgCompLib;
using CfgCompLib.classes;
using LittleLearner.CFG.StateLogic;
using LittleLearner.SyntaxHighlighting;
using System.Text.RegularExpressions;
using System.Web;

namespace LittleLearner.CFG;

public partial class CfgComparer : ContentPage
{
    TextHighlighter highlighter = new TextHighlighter();
    public Shape SelectedShape = Shape.Start;
    public State state;
    public FlowchartDrawer flowchartDrawer;
    public DashboardViewModel dashboardViewModel;
    public string code;

    public CfgComparer(DashboardViewModel viewModel)
	{
        //TODO Anwender sollte die möglichkeit bekommen, den Graphen in DOT-Format exportieren zu können
        InitializeComponent();
        BindingContext = viewModel;
        dashboardViewModel = viewModel;

        code = "int main(){ \n char a[] = \"<Hallo><Welt>\"\nreturn 0; }\n\nint";
        CodeSection.Html = highlighter.initializeCodeHighligher(code);

        flowchartDrawer = new FlowchartDrawer();
        Resources["flowchart"] = flowchartDrawer;
        state = new SelectState(flowchartDrawer, FlowchartView);

        // Declares Functionality for the Flowgraph to be moved by dragging the Pointer across the drawable area
        FlowchartView.Invalidate();
    }

    // Saves the Point, where the user touched the drawable area to calculate the direction and distance of movement

    // Calculates the direction and distance the user moved the Pointer after touching the drawable area
    // Moves the drawable objects accordingly

    private void ImportCProgram(object sender, EventArgs args)
    {
        return;
    }
    private void SaveCProgram(object sender, EventArgs args)
    {
        return;
    }
    private async void ImportCFG(object sender, EventArgs args)
    {
        FileResult? file = await FilePicker.Default.PickAsync();
        if (file == null) { return; }

        flowchartDrawer.graph = CfgFromFlowChart.GenerateGraphFromXML(file.FullPath);

        FlowchartView.Invalidate();
    }
    private void SaveCFG(object sender, EventArgs args)
    {
        return;
    }

    private void Move(object sender, EventArgs args){ state.ClearEventHandler(); state = new MoveFlowchartState(flowchartDrawer, FlowchartView); }
    private void Scale(object sender, EventArgs args) { state.ClearEventHandler(); state = new ScaleFlowchartState(flowchartDrawer, FlowchartView); }
    private void Select(object sender, EventArgs args) { state.ClearEventHandler(); state = new SelectState(flowchartDrawer, FlowchartView); }
    private void Add(object sender, EventArgs args) { state.ClearEventHandler(); state = new AddElementState(flowchartDrawer, FlowchartView); }
    private void Connect(object sender, EventArgs args) { state.ClearEventHandler(); state = new ConnectElementState(flowchartDrawer, FlowchartView); }
    private void Edit(object sender, EventArgs args){ state.ClearEventHandler(); state = new EditElementState(flowchartDrawer, FlowchartView); }
    private void Delete(object sender, EventArgs args) { state.ClearEventHandler(); state = new DeleteShapeState(flowchartDrawer, FlowchartView); }


    private async void CodeWritten(object? sender, WebNavigatingEventArgs navigationArgs)
    {
        string[] queryParams = navigationArgs.Url.Split("?");
        if (queryParams.Length != 2) return;

        code = HttpUtility.UrlDecode(new Regex("^newCode=").Replace(queryParams[1], ""));
        string coloredCode = highlighter.updateCode(code);
        navigationArgs.Cancel = true;

        await CodeWebView.EvaluateJavaScriptAsync($"setInnerText('{coloredCode}')");
    }

    public void CompareGraphs(object sender, EventArgs args)
    {
        if (!DashboardView.IsVisible)
        {
            //DashboardView.IsVisible = true;
            CfgMainGrid.SetRowSpan(CodeWebView, 2);
            CfgMainGrid.SetRowSpan(GraphLayout, 2);
        }
        else
        {
            //DashboardView.IsVisible = false;
            CfgMainGrid.SetRowSpan(CodeWebView, 1);
            CfgMainGrid.SetRowSpan(GraphLayout, 1);
        }

        dashboardViewModel.UpdateViewModel(null, null, 0);
        return;

        List<Graph> graphs = new List<Graph>();

        Graph maximumCodeGraph = GraphUtils.ExpandToMaxGraph(flowchartDrawer.graph);
        maximumCodeGraph.Description = "Control Flow Graph";

        Graph maximumFlowChartGraph = GraphUtils.ExpandToMaxGraph(flowchartDrawer.graph);
        maximumFlowChartGraph.Description = "Flow Graph";

        graphs.Add(maximumCodeGraph);
        graphs.Add(maximumFlowChartGraph);
    }
}