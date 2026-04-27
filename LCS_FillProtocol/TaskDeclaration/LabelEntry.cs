namespace LCS_FillProtocol.TaskDeclaration
{
    public class LabelEntry
    {
        public int Index { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
        public string? ValueRepresentation { get; set; }
        public object? TypeCheck { get; set; }
        public object? ValueCheck { get; set; }
        public bool Corrected { get; set; }
        public bool AbsCorrectedType { get; set; }
        public bool AbsCorrectedValue { get; set; }
        public bool FailedToInclude { get; set; }
        public bool GotPoint { get; set; }
        public string? FailedToIncludeMessage { get; set; }
        public bool HasErrors { get; set; }
    }
}
