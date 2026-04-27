namespace LCS_FillProtocol.Table
{
    public class TableTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LabelTemplate { get; set; }
        public DataTemplate EditorTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((TableCell)item).IsWritable ? (EditorTemplate) : (LabelTemplate);
        }
    }
}
