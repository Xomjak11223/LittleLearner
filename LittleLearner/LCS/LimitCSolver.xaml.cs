using CommunityToolkit.Maui.Storage;
using LimitCSolver.LimitCGenerator;
using LittleLearner.LCS.Modals;
using LittleLearner.LCS.ViewModel;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace LittleLearner.LCS;

public partial class LimitCSolver : ContentPage
{
    DifficultySettings difficulty = (new Settings()).Easy;
    string code = "";
    TableViewModel table;

	public LimitCSolver(TableViewModel vm) { 
        InitializeComponent();
        
        table = vm;
        BindingContext = vm;

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CodeWebViewLCS.SetInvokeJavaScriptTarget(this);
    }

    private async void OpenModalCodeSettings(object sender, EventArgs args) { await Navigation.PushModalAsync(new CodeCreationConfiguratoin(difficulty)); }
    private async void OpenModalColorMapping(object sender, EventArgs args) { await Navigation.PushModalAsync(new ColorMapping()); }
    private async void ImportCProgram(object sender, EventArgs args) 
    {
        FileResult? file = await GetFileWithExtensionFromAllPlatforms(".c", "C Programmcode");
        if (file == null) { return; }

        Stream codeStream = await file.OpenReadAsync();
        StreamReader reader = new StreamReader(codeStream);
        string content = reader.ReadToEnd();

        await SetCodeEditorCode(content);
        table.InitializeTableFromCode(content);
    }

	private async void SaveCProgram(object sender, EventArgs args)
    {
        string outString = await GetCodeEditorCode();

        using var stream = new MemoryStream(Encoding.Default.GetBytes(outString));
        await FileSaver.Default.SaveAsync($"NewPogram.c", stream, CancellationToken.None);
    }

	private void ImportLables(object sender, EventArgs args){return;}
	private async void Sync(object sender, EventArgs args) 
    {
        string code = await GetCodeEditorCode();
        code = code.Replace("\\r", "\r").Replace("\\n", "\n");

        table.InitializeTableFromCode(code); 
    }
    private void GenerateCodeC(object? sender, EventArgs args) 
    {
        if (difficulty == null) { return; }
        GetCodeEditorCode();

        code = (new CodeGenerator(difficulty)).GenerateCode();
        if(code == null) { return; }

        Task? config = JsonConvert.DeserializeObject<Task>(code);
        if(config == null || config?.Code == null) { return; }

        code = config.Code.Trim([ ' ', '\r', '\n' ]);
        SetCodeEditorCode(code);
        table.InitializeTableFromCode(code);
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

    public Task<string> SetCodeEditorCode(string newCode)
    {
        newCode = System.Text.Json.JsonSerializer.Serialize(newCode);
        return CodeWebViewLCS.EvaluateJavaScriptAsync($"setCode({newCode})");
    }

    public Task<string> GetCodeEditorCode() { return CodeWebViewLCS.EvaluateJavaScriptAsync($"getCode()"); }

    // This Methode gets called from the JavaScript of the page
    // It gets called every time the user changes the code in the source code generator by typing or pasting content into it
    // This Method currently does not work due to bugs from hybridWebView
    public void OnUserWriteUpdate(string newCode)
    {
        SetCodeEditorCode(code);
        table.InitializeTableFromCode(code);
    }

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