namespace LittleLearner.Tabs;

public partial class CfgComparer : ContentPage
{
	public CfgComparer()
	{
		InitializeComponent();

		var canvas = this.FlowchartView;
		canvas.Invalidate();
	}
}