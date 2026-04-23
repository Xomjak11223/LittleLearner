using LimitCSolver.LimitCGenerator;
using LittleLearner.LCS.ViewModel;

namespace LittleLearner.LCS.Modals;

public partial class CodeCreationConfiguratoin : ContentPage
{
    public DifficultySettings DifficultySettings;
    public DifficultySettingsViewModel vm;

    public CodeCreationConfiguratoin(DifficultySettings oldDifficultySettings) 
    { 
        InitializeComponent();
        if(oldDifficultySettings == null) { DifficultySettings = (new Settings()).Easy; }
        else { DifficultySettings = oldDifficultySettings; }

        vm = new DifficultySettingsViewModel(DifficultySettings);
        BindingContext = vm;
    }

    private void CloseModal(object? sender, EventArgs arguments) { Navigation.PopModalAsync(); }

    private void RemovedNumericOperation(object? sender, EventArgs arguments) { vm.RemoveNumericOperation(((Button)sender).Text); }
    private void RemovedVariableAssignment(object? sender, EventArgs arguments) { vm.RemoveVariableAssignment(((Button)sender).Text); }
    private void PickedNumericOperation(object? sender, EventArgs arguments) {
        string operation = (string)((Picker)sender).SelectedItem;
        if (string.IsNullOrEmpty(operation)) { return; }

        vm.AddNumericOperation(operation);
        NumericOperationsPicker.SelectedIndex = -1;
    }
    private void PickedVariableAssignment(object? sender, EventArgs arguments) { 
        string assignment = (string)((Picker)sender).SelectedItem;
        if (string.IsNullOrEmpty(assignment)) { return; }

        vm.AddVariableAssignment(assignment);
        NumericOperationsPicker.SelectedIndex = -1;
    }
}