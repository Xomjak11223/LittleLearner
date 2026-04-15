using CfgCompLib;
using CfgCompLib.classes;
using ColorfulCode;
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

    public CfgComparer()
	{
        InitializeComponent();

        CodeSection.Html = highlighter.initializeCodeHighligher("int main(){ \n char a[] = \"<Hallo><Welt>\"\nreturn 0; }\n\nint");

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

        string newCode = HttpUtility.UrlDecode(new Regex("^newCode=").Replace(queryParams[1], ""));
        string coloredCode = highlighter.updateCode(newCode);
        navigationArgs.Cancel = true;

        await CodeWebView.EvaluateJavaScriptAsync($"setInnerText('{coloredCode}')");
    }
}