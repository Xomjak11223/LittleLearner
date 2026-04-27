using CommunityToolkit.Mvvm.ComponentModel;

namespace LittleLearner.CFG.ViewModel
{
    public partial class ConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        public int nodeInsertCost;

        [ObservableProperty]
        public int nodeDeleteCost;

        [ObservableProperty]
        public int nodeRelabelCost;

        [ObservableProperty]
        public int edgeInsertCost;

        [ObservableProperty]
        public int edgeDeleteCost;

        [ObservableProperty]
        public string gccPath;

        [ObservableProperty]
        public string compilerOptimizations;

        [ObservableProperty]
        public float labelSimilarity;

        [ObservableProperty]
        public float comparisonCriteria1;

        [ObservableProperty]
        public float comparisonCriteria2;

        public ConfigurationViewModel(int nodeInsertCost, int nodeDeleteCost, int nodeRelabelCost, int edgeInsertCost, int edgeDeleteCost, string gccPath, string compilerOptimizations, float labelSimilarity, float comparisonCriteria1, float comparisonCriteria2)
        {
            this.nodeInsertCost = nodeInsertCost;
            this.nodeDeleteCost = nodeDeleteCost;
            this.nodeRelabelCost = nodeRelabelCost;
            this.edgeInsertCost = edgeInsertCost;
            this.edgeDeleteCost = edgeDeleteCost;
            this.gccPath = gccPath;
            this.compilerOptimizations = compilerOptimizations;
            this.labelSimilarity = labelSimilarity;
            this.comparisonCriteria1 = comparisonCriteria1;
            this.comparisonCriteria2 = comparisonCriteria2;
        }
    }
}
