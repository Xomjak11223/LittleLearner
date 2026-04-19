using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG.ViewModel
{
    public class SimilarLabels
    {
        public string CodeLabel { get; set; }
        public string GraphLabel { get; set; }
        public double Similarity { get; set; }

        public SimilarLabels(string CodeLabel, string GraphLabel, double Similarity){
            this.CodeLabel = CodeLabel;
            this.GraphLabel = GraphLabel;
            this.Similarity = Similarity;
        }
    }
}
