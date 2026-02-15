using LimitCSolver.LimitCGenerator;
using System.Diagnostics;

namespace LittleLearner.Tabs;

public partial class LimitCSolver : ContentPage
{
	public LimitCSolver()
	{
		InitializeComponent();
	}

	public void toggleCodeCreator(){
		Code.Text = "Hello World";
		CodeCreator.IsVisible = !CodeCreator.IsVisible;
	}

	public void automaticCreateCode()
	{
		Settings difficultySettings = new Settings();
		CodeGenerator codeGenerator = new CodeGenerator(difficultySettings.Medium);
		String code = codeGenerator.GenerateCode();
		Code.Text = "Hello";
	}
}