using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace LittleLearner.LCS.ViewModel
{
    public class VariableSolution
    {
        public string Name;
        public string Type;
        public string Value;
        public string ValueRepresentation;

        public VariableSolution(string name, string type, string value, string valueRepresentation)
        {
            Name = name;
            Type = type;
            Value = value;
            ValueRepresentation = valueRepresentation;
        }
    }
}
