using Antlr4.Runtime;
using LimitCSolver.LimitCGenerator;
using LimitCSolver.LimitCInterpreter;
using LimitCSolver.LimitCInterpreter.Parser;
using System.Text.RegularExpressions;

namespace LittleLearner.LCS;

public partial class LimitCSolver : ContentPage
{
	DifficultySettings? difficulty;
    Protocol protocol;

	public LimitCSolver(Protocol viewModel)
	{
		InitializeComponent();
        protocol = viewModel;
        BindingContext = protocol;
	}

	public void ToggleCodeCreator(object sender, EventArgs arguments){ CodeCreator.IsVisible = !CodeCreator.IsVisible; }

	public void AutomaticCreateCode(object sender, EventArgs arguments)
	{
		if(difficulty == null) { return; }
		string createdCode = (new CodeGenerator(difficulty)).GenerateCode();

        createdCode = Regex.Replace(createdCode, "^{\"Code\":\"", "");
        createdCode = Regex.Replace(createdCode, "\"}$", "");
        createdCode = createdCode.Replace("\\r\\n", "\r\n");

		string coloredCode = createdCode;
        Protocol newProtocol = GetProtocolFromCode(createdCode);

        // Creates the new Table on the UI-Thread
        Grid grid = newProtocol.CreateGridTable();
        MainGrid.RemoveAt(2);
        MainGrid.Children.Add(grid);

        code.Text = coloredCode;
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

    private void ImportCProgram(object sender, EventArgs args){
    }
	private void SaveCProgram(object sender, EventArgs args){return;}
	private void ImportLables(object sender, EventArgs args){return;}
	private void Sync(object sender, EventArgs args) { return; }

	public Protocol GetProtocolFromCode(string inputCode)
	{
        // newProtocol is the variable that holds all the lables
        Protocol newProtocol = new();
        var interpreter = new LimitCInterpreter();
        var program = parse(inputCode);

        interpreter.LabelCheckPointReached += (sender, args) =>
        {
            string[] variables = new string[0];
            //var npe = new ProtocolEntryViewModel() { Num =  };

            foreach (var (name, addr) in args.VisibleVars)
            {
                /* TODO need to look into what this does
                TypedValue memVal = args.MemoryStorage.Memory[addr];
                var p = new string('*', memVal.Type.Count(c => c == '*'));

                npe.VarEntrys.Add(new VarViewModel($"{p}{name}", "", "", ""));
                */

                variables = variables.Append(name).ToArray<string>();
            }
            newProtocol.AddEmptyLabel(args.LabelNum, variables);
        };



        if (program == null)
            return newProtocol; // an error occured during tree build, we cannot continue safely

        interpreter.evaluate(program);

        return newProtocol;
    }

    private static LimitCParser.ProgContext? parse(string code)
    {
        var inputStream = new AntlrInputStream(code);
        var lexer = new LimitCLexer(inputStream);
        var tkStream = new CommonTokenStream(lexer);
        var parser = LimitCParser.Instance(tkStream);

        parser.AddErrorListener(new DiagnosticErrorListener());

        var limitCContext = parser.prog();

        if (parser.Errors.Count != 0)
            return null; // This is dirty. Errors should be handled and displayed to the user.
        else
            return limitCContext;
    }
}