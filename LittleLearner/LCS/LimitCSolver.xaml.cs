using Antlr4.Runtime;
using LimitCSolver.LimitCGenerator;
using LimitCSolver.LimitCInterpreter;
using LimitCSolver.LimitCInterpreter.Memory;
using LimitCSolver.LimitCInterpreter.Parser;
using Newtonsoft.Json;
using System.Globalization;
using LittleLearner.LCS.Modals;

namespace LittleLearner.LCS;

public partial class LimitCSolver : ContentPage
{
    DifficultySettings difficulty = (new Settings()).Easy;
    Protocol currentProtocol;
    String code = "";

	public LimitCSolver() { 
        InitializeComponent();
        currentProtocol = new Protocol();
        LabelGrid.RemoveAt(2);
        LabelGrid.Children.Add(currentProtocol.CreateGridTable());
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    }

    private async void OpenModalCodeSettings(object sender, EventArgs args) { await Navigation.PushModalAsync(new CodeCreationConfiguratoin(difficulty)); }
    private async void OpenModalColorMapping(object sender, EventArgs args) { await Navigation.PushModalAsync(new ColorMapping()); }
    private void ImportCProgram(object sender, EventArgs args){ return; }
	private void SaveCProgram(object sender, EventArgs args){return;}
	private void ImportLables(object sender, EventArgs args){return;}
	private void Sync(object sender, EventArgs args) { return; }
    private void GenerateCodeC(object? sender, EventArgs args) 
    {
        if (difficulty == null) { return; }

        code = (new CodeGenerator(difficulty)).GenerateCode();
        if(code == null) { return; }

        Task? config = JsonConvert.DeserializeObject<Task>(code);
        if(config == null || config?.Code == null) { return; }

        code = config.Code.Trim([ ' ', '\r', '\n' ]);
        SetCodeEditorCode(code);
        currentProtocol = GetProtocolFromCode(code);

        // Creates the new Table on the UI-Thread
        Grid? grid = currentProtocol.CreateGridTable();

        LabelGrid.RemoveAt(2);
        LabelGrid.Children.Add(grid);
    }

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
                TypedValue memVal = args.MemoryStorage.Memory[addr];
                var p = new string('*', memVal.Type.Count(c => c == '*'));

                //npe.VarEntrys.Add(new VarViewModel($"{p}{name}", "", "", ""));

                variables = variables.Append(p + name).ToArray<string>();
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

    private void CompareSolutions(object sender, EventArgs args)
    {
        string[] answers = new string[0];
        var rows = ((Grid)LabelGrid.ElementAt(2)).Children;

        foreach (Border row in rows)
        {
            if(row.Content == null || !typeof(Editor).Equals(row.Content.GetType())) continue;

            Editor content = (Editor) row.Content;
            answers.Append(content.Text);
        }
    }

    public Task<string> SetCodeEditorCode(string newCode) {
        newCode = JsonConvert.SerializeObject(new { code = newCode })
            .Replace("\\r", "\r")
            .Replace("\\n", "\n");

        return CodeWebViewLCS.EvaluateJavaScriptAsync($"setCode({newCode})"); 
    }
    public Task<string> GetCodeEditorCode() { return CodeWebViewLCS.EvaluateJavaScriptAsync("getCode()"); }
}