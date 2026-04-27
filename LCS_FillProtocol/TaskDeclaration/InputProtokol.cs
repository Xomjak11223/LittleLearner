using System;
namespace LCS_FillProtocol.TaskDeclaration
{
    public class InputProtokol
    {
        public Label[]? Entrys { get; set; }
        public string? ProtocolOrLabelMismatchMessage { get; set; }
        public bool ProtocolLabelOrVarMismatch { get; set; }
        public object? Points { get; set; }
        public int MaxVarCount { get; set; }
    }
}
