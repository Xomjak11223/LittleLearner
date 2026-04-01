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
        state = new MoveFlowchartState(flowchartDrawer, FlowchartView);

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

    private void addNode(float x, float y) {
        
    }

    private void temporaryStart(object sender, EventArgs args)
    {

    }
    private void temporaryEnd(object sender, EventArgs args)
    {

    }
    private void temporaryAction(object sender, EventArgs args)
    {

    }
    private void temporaryDecision(object sender, EventArgs args)
    {

    }

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