using LimitCSolver.LimitCGenerator;
using System.Text.RegularExpressions;

namespace LittleLearner.Tabs;

public partial class LimitCSolver : ContentPage
{
	DifficultySettings? difficulty;

	public LimitCSolver()
	{
		InitializeComponent();
	}

	public void ToggleCodeCreator(object sender, EventArgs arguments){ CodeCreator.IsVisible = !CodeCreator.IsVisible; }

	public void AutomaticCreateCode(object sender, EventArgs arguments)
	{
		if(difficulty == null) { return; }
		string createdCode = (new CodeGenerator(difficulty)).GenerateCode();
		createdCode = createdCode.Replace("\\r\\n", "\n");
		createdCode = Regex.Replace(createdCode, "^{\"Code\":\"", "");
        createdCode = Regex.Replace(createdCode, "\\n\"}", "");

        code.Text = createdCode;
    }

	public void SelectDifficulty(object sender, EventArgs arguments)
	{
		DifficultyEasy.BackgroundColor = new Color(255, 0, 255);
        DifficultyMedium.BackgroundColor = new Color(255, 0, 255);
        DifficultyHard.BackgroundColor = new Color(255, 0, 255);
		
		if(sender == DifficultyEasy){ difficulty = new Settings().Easy; }
		else if(sender == DifficultyMedium){ difficulty = new Settings().Medium; }
		else if (sender == DifficultyHard) { difficulty = new Settings().Hard; }

        ((Border)sender).BackgroundColor = new Color(255, 0, 0);
	}
}