using CommunityToolkit.Maui.Storage;
using LCS_FillProtocol.Table;
using LCS_FillProtocol.TaskDeclaration;
using Newtonsoft.Json;
using System.Text;

namespace LCS_FillProtocol
{
    public partial class MainPage : ContentPage
    {
        TableViewModel table;
        public MainPage(TableViewModel vm){
            InitializeComponent();
            table = vm;
            BindingContext = vm;
            CodeWebView.SetInvokeJavaScriptTarget(this);
        }

        public async void ImportProtocol(object? sender, EventArgs args) 
        {
            string fileExtension = "json";
            FilePickerFileType fileTypes = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[]{ fileExtension } },
                    { DevicePlatform.Android, new[]{ $"application/{fileExtension}" } },
                    { DevicePlatform.MacCatalyst, new[]{ fileExtension } },
                    { DevicePlatform.macOS, new[]{ fileExtension } },
                    { DevicePlatform.WinUI, new[]{ $".{fileExtension}" } }
                }
            );

            FileResult? result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "LimitCSolver Task",
                FileTypes = fileTypes
            });

            if(result == null) return;

            Stream codeStream = await result.OpenReadAsync();
            StreamReader reader = new StreamReader(codeStream);
            string content = reader.ReadToEnd();

            TaskInput? newTask = JsonConvert.DeserializeObject<TaskInput>(content);
            if(newTask == null) return;

            table.InitializeFromTask(newTask);

            if(table.taskCode != null) 
            {
                string newCode = System.Text.Json.JsonSerializer.Serialize(table.taskCode);
                await CodeWebView.EvaluateJavaScriptAsync($"setCode({newCode})");
            }
        }
        public async void ExportProtocol(object? sender, EventArgs args)
        {
            // Misleading name, because InputProtokol is one subClass used in TaskInput
            InputProtokol output = table.ExportCurrentTast();
            string outString = JsonConvert.SerializeObject(output);

            using var stream = new MemoryStream(Encoding.Default.GetBytes(outString));
            var fileSaverResult = await FileSaver.Default.SaveAsync($"{table.TaskTitle}.lcp.json", stream, CancellationToken.None);
        }
    }
}
