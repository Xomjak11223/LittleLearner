using CfgCompLib;
using CfgCompLib.classes;
using LittleLearner.CFG;

namespace LittleLearner.Tabs;

public partial class CfgComparer : ContentPage
{
    private Shape selectedShape = Shape.Action;
    private float GraphicsViewOffsetY = 0;
	private float GraphicsViewOffsetX = 0;

    private float UserTouchX = 0;
    private float UserTouchY = 0;

    public CfgComparer()
	{
        InitializeComponent();
        // Declares Functionality for the Flowgraph to be moved by dragging the Pointer across the drawable area
        FlowchartView.StartInteraction += OnFlowchartPressed;
        FlowchartView.DragInteraction += OnFlowchartDragged;
    }

    // Saves the Point, where the user touched the drawable area to calculate the direction and distance of movement
	private void OnFlowchartPressed(object sender, TouchEventArgs eventArgs)
	{
		if(FlowchartDrawer.graph == null) { return; }

		UserTouchX = eventArgs.Touches.FirstOrDefault().X;
        UserTouchY = eventArgs.Touches.FirstOrDefault().Y;

        GraphicsViewOffsetX = FlowchartDrawer.offsetX;
        GraphicsViewOffsetY = FlowchartDrawer.offsetY;
    }

    // Calculates the direction and distance the user moved the Pointer after touching the drawable area
    // Moves the drawable objects accordingly
    private void OnFlowchartDragged(object sender, TouchEventArgs eventArgs)
	{
        if (FlowchartDrawer.graph == null) { return; }

        float dx = eventArgs.Touches.FirstOrDefault().X - UserTouchX;
        float dy = eventArgs.Touches.FirstOrDefault().Y - UserTouchY;

        FlowchartDrawer.offsetX = GraphicsViewOffsetX + dx;
        FlowchartDrawer.offsetY = GraphicsViewOffsetY + dy;
        FlowchartView.Invalidate();
    }

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

        FlowchartDrawer.graph = CfgFromFlowChart.GenerateGraphFromXML(file.FullPath);

        FlowchartView.Invalidate();
    }
    private void SaveCFG(object sender, EventArgs args)
    {
        return;
    }

}