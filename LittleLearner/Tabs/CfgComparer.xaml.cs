using CfgCompLib;
using CfgCompLib.classes;
using LittleLearner.CFG;

namespace LittleLearner.Tabs;

public partial class CfgComparer : ContentPage
{
	public CfgComparer()
	{
		InitializeComponent();
		initProtocol();
    }

	public async void initProtocol()
	{
        FileResult? file = await FilePicker.Default.PickAsync();
		if (file == null) { return; }

        FlowchartDrawer.graph = CfgFromFlowChart.GenerateGraphFromXML(file.FullPath); ;
        FlowchartView.Invalidate();
    }
}