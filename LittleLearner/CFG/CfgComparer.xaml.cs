using CfgCompLib;
using CfgCompLib.classes;
using LittleLearner.CFG.StateLogic;

namespace LittleLearner.CFG;

public partial class CfgComparer : ContentPage
{
    private Shape selectedShape = Shape.Action;
    private float GraphicsViewOffsetY = 0;
	private float GraphicsViewOffsetX = 0;

    private float UserTouchX = 0;
    private float UserTouchY = 0;

    private readonly float standartWidth = 100;
    private readonly float standartHeight = 100;

    private Boolean shapeSelected = false;
    public Shape SelectedShape = Shape.Start;
    public State state;
    public FlowchartDrawer flowchartDrawer;

    public CfgComparer()
	{
        InitializeComponent();

        flowchartDrawer = new FlowchartDrawer();
        Resources["flowchart"] = flowchartDrawer;
        state = new MoveFlowchartState(fl);

        // Declares Functionality for the Flowgraph to be moved by dragging the Pointer across the drawable area
        FlowchartView.StartInteraction += OnFlowchartPressed;
        FlowchartView.DragInteraction += OnFlowchartDragged;
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


}