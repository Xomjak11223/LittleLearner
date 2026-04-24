using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LimitCSolver.LimitCGenerator;
using System.Collections.ObjectModel;

namespace LittleLearner.LCS.ViewModel
{
    public partial class DifficultySettingsViewModel : ObservableObject
    {
        public static string[] numericOperations = ["Addition", "Subtraction", "Multiplication", "Division"];
        public static string[] variableAssignments = ["Global Variables", "Variable Assignment", "Shadow Variables"];

        [ObservableProperty]
        public ObservableCollection<string> numericOperationsSelected;

        [ObservableProperty]
        public ObservableCollection<string> numericOperationsNotSelected;

        [ObservableProperty]
        public ObservableCollection<string> variableAssignmentsSelected;

        [ObservableProperty]
        public ObservableCollection<string> variableAssignmentsNotSelected;

        [ObservableProperty]
        public DifficultySettings settings;

        public DifficultySettingsViewModel(DifficultySettings oldSettings)
        {
            numericOperationsSelected = new ObservableCollection<string>();
            numericOperationsNotSelected = new ObservableCollection<string>();
            variableAssignmentsSelected = new ObservableCollection<string>();
            variableAssignmentsNotSelected = new ObservableCollection<string>();

            if (oldSettings == null) oldSettings = new Settings().Easy;
            settings = new DifficultySettings();
            UpdateDifficulty(oldSettings);
        }

        public void AddNumericOperation(string operation)
        {
            if (!numericOperations.Contains(operation)) { return; }
            NumericOperationsSelected.Add(operation);
            NumericOperationsNotSelected.Remove(operation);
        }

        public void RemoveNumericOperation(string operation)
        {
            if (!numericOperations.Contains(operation)) { return; }
            NumericOperationsNotSelected.Add(operation);
            NumericOperationsSelected.Remove(operation);
        }

        public void AddVariableAssignment(string assignment)
        {
            if (!variableAssignments.Contains(assignment)) { return; }
            VariableAssignmentsSelected.Add(assignment);
            VariableAssignmentsNotSelected.Remove(assignment);
        }

        public void RemoveVariableAssignment(string assignment)
        {
            if (!variableAssignments.Contains(assignment)) { return; }
            VariableAssignmentsNotSelected.Add(assignment);
            VariableAssignmentsSelected.Remove(assignment);
        }

        public void UpdateDifficulty(DifficultySettings difficulty)
        {
            Settings = difficulty;
            NumericOperationsSelected.Clear();
            NumericOperationsNotSelected.Clear();
            VariableAssignmentsSelected.Clear();
            VariableAssignmentsNotSelected.Clear();

            // Assignes numeric Operations
            if (Settings.AllowAddition) { NumericOperationsSelected.Add(numericOperations[0]); }
            else { NumericOperationsNotSelected.Add(numericOperations[0]); }

            if (Settings.AllowSubtraction) { NumericOperationsSelected.Add(numericOperations[1]); }
            else { NumericOperationsNotSelected.Add(numericOperations[1]); }

            if (Settings.AllowMultiplication) { NumericOperationsSelected.Add(numericOperations[2]); }
            else { NumericOperationsNotSelected.Add(numericOperations[2]); }

            if (Settings.AllowDivision) { NumericOperationsSelected.Add(numericOperations[3]); }
            else { NumericOperationsNotSelected.Add(numericOperations[3]); }

            // Assigns Varible Assignments
            if (Settings.AllowGlobalVariables) { VariableAssignmentsSelected.Add(variableAssignments[0]); }
            else { VariableAssignmentsNotSelected.Add(variableAssignments[0]); }

            if (Settings.AllowVariableAssignment) { VariableAssignmentsSelected.Add(variableAssignments[1]); }
            else { VariableAssignmentsNotSelected.Add(variableAssignments[1]); }

            if (Settings.AllowShadowVariables) { VariableAssignmentsSelected.Add(variableAssignments[2]); }
            else { VariableAssignmentsNotSelected.Add(variableAssignments[2]); }
        }
    }
}
