using CfgCompLib;
using CfgCompLib.classes;
using CommunityToolkit.Maui.Storage;
using LittleLearner.CFG.Modal;
using LittleLearner.CFG.StateLogic;
using LittleLearner.CFG.ViewModel;
using System.Text;
using System.Text.RegularExpressions;

namespace LittleLearner.CFG;

public partial class CfgComparer : ContentPage
{
    public Shape SelectedShape = Shape.Start;
    public State state;
    public FlowchartDrawer flowchartDrawer;
    public Dashboard dashboard;
    public string code = "";

    Graph swapGraph;
    float swapOffsetX;
    float swapOffsetY;
    float swapZoom;
    bool onOptimalFlowchart = false;

    public CfgComparer(Dashboard viewModel)
	{
        //TODO Anwender sollte die möglichkeit bekommen, den Graphen in DOT-Format exportieren zu können
        InitializeComponent();
        BindingContext = viewModel;
        dashboard = viewModel;

        flowchartDrawer = new FlowchartDrawer();
        Resources["flowchart"] = flowchartDrawer;
        state = new SelectState(flowchartDrawer, FlowchartView);

        // Declares Functionality for the Flowgraph to be moved by dragging the Pointer across the drawable area
        FlowchartView.Invalidate();
    }

    // Saves the Point, where the user touched the drawable area to calculate the direction and distance of movement

    // Calculates the direction and distance the user moved the Pointer after touching the drawable area
    // Moves the drawable objects accordingly

    private async void ImportCProgram(object sender, EventArgs args)
    {
        FileResult? file = await GetFileWithExtensionFromAllPlatforms(".c", "C Programmcode");
        if (file == null) { return; }

        Stream codeStream = await file.OpenReadAsync();
        StreamReader reader = new StreamReader(codeStream);
        string content = reader.ReadToEnd();

        var result = await SetCodeEditorCode(content);
    }

    private async void ChangeConfigurations(object sender, EventArgs args)
    {
        await Navigation.PushModalAsync(new Konfiguration());
    }

    private async void SaveCProgram(object sender, EventArgs args)
    {
        string outString = await GetCodeEditorCode();

        using var stream = new MemoryStream(Encoding.Default.GetBytes(outString));
        await FileSaver.Default.SaveAsync($"NewPogram.c", stream, CancellationToken.None);
    }

    private async void SaveCodeToDot(object sender, EventArgs args)
    {
        // Misleading name, because InputProtokol is one subClass used in TaskInput
        string outString = await GetCodeEditorCode();

        outString = outString.Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\\\", @"\");
        string path = Path.Combine(FileSystem.AppDataDirectory, "code.c");
        File.WriteAllText(path, outString);

        string codeGraphString;
        try { codeGraphString = CfgFromCompiler.ImportCompilerCfgRaw(path); } catch {
            Console.Error.WriteLine("C Code could not be compiled. Cant convert code to Graph");
            return; 
        }

        using var stream = new MemoryStream(Encoding.Default.GetBytes(""));
        FileSaverResult savedFile = await FileSaver.Default.SaveAsync($"NewGraph.dot", stream, CancellationToken.None);
        if (!savedFile.IsSuccessful) return;

        string dotPath = Regex.Replace(savedFile.FilePath, "[^\\.]+\\.dot$", "");
        string fileName = Regex.Replace(savedFile.FilePath, dotPath, "");
        fileName = Regex.Replace(fileName, "\\.dot$", "");

        GraphUtils.ExportGraphToDot(CfgFromCompiler.GenerateGraphFromRaw(codeGraphString), dotPath, fileName);
    }

    private async void SaveGraphToDot(object sender, EventArgs args)
    {
        if(flowchartDrawer.graph == null) { return; }

        using var stream = new MemoryStream(Encoding.Default.GetBytes(""));
        FileSaverResult savedFile = await FileSaver.Default.SaveAsync($"NewGraph.dot", stream, CancellationToken.None);
        if (!savedFile.IsSuccessful) return;

        string dotPath = Regex.Replace(savedFile.FilePath, "[^\\.]+\\.dot$", "");
        string fileName = Regex.Replace(savedFile.FilePath, dotPath, "");
        fileName = Regex.Replace(fileName, "\\.dot", "");

        GraphUtils.ExportGraphToDot(flowchartDrawer.graph, dotPath, fileName);
    }

    private async void ImportCFG(object sender, EventArgs args)
    {
        FileResult? file = await FilePicker.Default.PickAsync();
        if (file == null) { return; }

        Graph newGraph;
        try { newGraph = CfgFromFlowChart.GenerateGraphFromXML(file.FullPath); } catch
        {
            Console.Error.WriteLine("Imported File could not be convertet to Graph");
            return;
        }

        flowchartDrawer.graph = newGraph;
        FlowchartView.Invalidate();
    }

    private async void SaveOutpt(object sender, EventArgs args)
    {
        if (dashboard.optimalGraph == null) { return; }

        using var stream = new MemoryStream(Encoding.Default.GetBytes(""));
        FileSaverResult savedFile = await FileSaver.Default.SaveAsync($"NewGraph.dot", stream, CancellationToken.None);
        if (!savedFile.IsSuccessful) return;

        string dotPath = Regex.Replace(savedFile.FilePath, "[^\\.]+\\.dot$", "");
        string fileName = Regex.Replace(savedFile.FilePath, dotPath, "");
        fileName = Regex.Replace(fileName, "\\.dot", "");

        GraphUtils.ExportGraphToDot(dashboard.optimalGraph, dotPath, fileName);
    }

    private void Move(object sender, EventArgs args){ state.ClearEventHandler(); state = new MoveFlowchartState(flowchartDrawer, FlowchartView); }
    private void ScaleGraphObject(object sender, EventArgs args) { state.ClearEventHandler(); state = new ScaleFlowchartState(flowchartDrawer, FlowchartView); }
    private void Select(object sender, EventArgs args) { state.ClearEventHandler(); state = new SelectState(flowchartDrawer, FlowchartView); }
    private void Add(object sender, EventArgs args) { state.ClearEventHandler(); state = new AddElementState(flowchartDrawer, FlowchartView); }
    private void Connect(object sender, EventArgs args) { state.ClearEventHandler(); state = new ConnectElementState(flowchartDrawer, FlowchartView); }
    private void Edit(object sender, EventArgs args){ state.ClearEventHandler(); state = new EditElementState(flowchartDrawer, FlowchartView, this); }
    private void Delete(object sender, EventArgs args) { state.ClearEventHandler(); state = new DeleteShapeState(flowchartDrawer, FlowchartView); }

    public async void CompareGraphs(object sender, EventArgs args)
    {
        code = await GetCodeEditorCode();
        code = code.Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\\\", @"\");
        if (flowchartDrawer.graph == null || string.IsNullOrEmpty(code)) return;

        if (dashboard.CanSwapFlowchart)
        {
            if (onOptimalFlowchart) ToggleGraphPresent(null, null);
            
            dashboard.CanSwapFlowchart = false;
            dashboard.optimalGraph = null;
            CfgMainGrid.SetRowSpan(EditorLayout, 2);
            return;
        }

        dashboard.CanSwapFlowchart = true;
        CfgMainGrid.SetRowSpan(EditorLayout, 1);

        string path = Path.Combine(FileSystem.AppDataDirectory, "code.c");
        File.WriteAllText(path, code);

        string codeGraphString;
        try { codeGraphString = CfgFromCompiler.ImportCompilerCfgRaw(path); } catch { return; }
        dashboard.UpdateViewModel(CfgFromCompiler.GenerateGraphFromRaw(codeGraphString), flowchartDrawer.graph, 0.5);

        swapGraph = dashboard.optimalGraph;
        swapOffsetX = 0;
        swapOffsetY = 0;
        swapZoom = 1;
    }

    public Task<string> SetCodeEditorCode(string newCode)
    {
        newCode = System.Text.Json.JsonSerializer.Serialize(newCode);
        return CodeWebViewCfg.EvaluateJavaScriptAsync($"setCode({newCode})");
    }

    public Task<string> GetCodeEditorCode() { return CodeWebViewCfg.EvaluateJavaScriptAsync("getCode()"); }
    public Task<FileResult?> GetFileWithExtensionFromAllPlatforms(string fileExtension, string filePickerTitle)
    {
        FilePickerFileType fileTypes = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[]{ fileExtension } },
                { DevicePlatform.Android, new[]{ fileExtension } },
                { DevicePlatform.MacCatalyst, new[]{ fileExtension } },
                { DevicePlatform.macOS, new[]{ fileExtension } },
                { DevicePlatform.WinUI, new[]{ fileExtension } }
            }
        );

        return FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = filePickerTitle,
            FileTypes = fileTypes
        });
    }

    public void ToggleGraphPresent(object sender, EventArgs args)
    {
        // swaps visible flowChart
        Graph tempGraph = flowchartDrawer.graph;
        float tempOffsetX = flowchartDrawer.offsetX;
        float tempOffsetY = flowchartDrawer.offsetY;
        float tempZoom = flowchartDrawer.zoom;

        flowchartDrawer.graph = swapGraph;
        flowchartDrawer.offsetX = swapOffsetX;
        flowchartDrawer.offsetY = swapOffsetY;
        flowchartDrawer.zoom = swapZoom;

        swapGraph = tempGraph;
        swapOffsetX = tempOffsetX;
        swapOffsetY = tempOffsetY;
        swapZoom = tempZoom;

        onOptimalFlowchart = !onOptimalFlowchart;
    }

    public async void displayErrorToast(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    }
}