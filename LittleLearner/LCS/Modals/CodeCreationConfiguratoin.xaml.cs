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

    private void SelectPredefinedDifficulty(object? sender, EventArgs arguments)
    {
        string difficultyString = (string)((Picker)sender).SelectedItem;
        DifficultySettings difficulty;
        switch (difficultyString) 
        {
            case "Easy": difficulty = new Settings().Easy; break;
            case "Medium": difficulty = new Settings().Easy; break;
            case "Hard": difficulty = new Settings().Easy; break;
            default: difficulty = new Settings().Easy; break;
        }

        vm.UpdateDifficulty(difficulty);
        ResetNumericPickerSelection();
        ResetVariablePickerSelection();
    }

    private void CloseModal(object? sender, EventArgs arguments) { Navigation.PopModalAsync(); }

    private void RemovedNumericOperation(object? sender, EventArgs arguments) 
    { 
        vm.RemoveNumericOperation(((Button)sender).Text);
        ResetNumericPickerSelection();
    }
    private void RemovedVariableAssignment(object? sender, EventArgs arguments) 
    { 
        vm.RemoveVariableAssignment(((Button)sender).Text);
        ResetVariablePickerSelection();
    }
    private void PickedNumericOperation(object? sender, EventArgs arguments) {
        string operation = (string)((Picker)sender).SelectedItem;
        if (string.IsNullOrEmpty(operation)) { return; }

        vm.AddNumericOperation(operation);
        ResetNumericPickerSelection();
    }
    private void PickedVariableAssignment(object? sender, EventArgs arguments) { 
        string assignment = (string)((Picker)sender).SelectedItem;
        if (string.IsNullOrEmpty(assignment)) { return; }

        vm.AddVariableAssignment(assignment);
        ResetVariablePickerSelection();
    }

    private void StepToNextValue(object? sender, EventArgs arguments)
    {
        double currentValue = ((Slider)sender).Value;
        int integerValue = (int) currentValue;

        if(currentValue - integerValue <= 0.1) { ((Slider)sender).Value = integerValue; }
    }

    private void ResetNumericPickerSelection()
    {
        NumericOperationsPicker.SelectedIndexChanged -= PickedNumericOperation;
        NumericOperationsPicker.SelectedItem = null;
        NumericOperationsPicker.SelectedIndexChanged += PickedNumericOperation;
    }

    private void ResetVariablePickerSelection()
    {
        VariableAssignmentPicker.SelectedIndexChanged -= PickedVariableAssignment;
        VariableAssignmentPicker.SelectedItem = null;
        VariableAssignmentPicker.SelectedIndexChanged += PickedVariableAssignment;
    }
}