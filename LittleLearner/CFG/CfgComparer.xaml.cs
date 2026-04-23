using CfgCompLib;
using CfgCompLib.classes;
using LittleLearner.CFG.StateLogic;
using LittleLearner.CFG.ViewModel;
using System.Text.Json;

namespace LittleLearner.CFG;

public partial class CfgComparer : ContentPage
{
    public Shape SelectedShape = Shape.Start;
    public State state;
    public FlowchartDrawer flowchartDrawer;
    public Dashboard dashboard;
    public string code = "";

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
        int a = 0;
    }

    private async void SaveCProgram(object sender, EventArgs args) 
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

    private void Move(object sender, EventArgs args){ state.ClearEventHandler(); state = new MoveFlowchartState(flowchartDrawer, FlowchartView); }
    private void Scale(object sender, EventArgs args) { state.ClearEventHandler(); state = new ScaleFlowchartState(flowchartDrawer, FlowchartView); }
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

        if (DashboardLayout.IsVisible)
        {
            DashboardLayout.IsVisible = false;
            CfgMainGrid.SetRowSpan(EditorLayout, 2);
            return;
        }

        DashboardLayout.IsVisible = true;
        CfgMainGrid.SetRowSpan(EditorLayout, 1);

        string path = Path.Combine(FileSystem.AppDataDirectory, "code.c");
        File.WriteAllText(path, code);

        string codeGraphString;
        try { codeGraphString = CfgFromCompiler.ImportCompilerCfgRaw(path); } catch { return; }
        dashboard.UpdateViewModel(CfgFromCompiler.GenerateGraphFromRaw(codeGraphString), flowchartDrawer.graph, 0.5);
    }

    public Task<string> SetCodeEditorCode(string newCode)
    {
        newCode = JsonSerializer.Serialize(newCode);
        newCode = newCode.Replace("\\n", "\n").Replace("\\r", "\r");
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
}