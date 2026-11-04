namespace PMS.Services
{
    public interface IPdfService
    {
        byte[] GenerateListPdf<T>(
            string title,
            string subtitle,
            List<PdfColumn<T>> columns,
            IEnumerable<T> data,
            string generatedBy,
            Dictionary<string, string>? summaryItems = null,
            bool landscape = false);

        byte[] GenerateRecordDetailsPdf(
            string title,
            string recordCode,
            string recordName,
            List<PdfFieldGroup> fieldGroups,
            List<PdfDetailTable>? tables,
            string generatedBy);
    }

    public class PdfColumn<T>
    {
        public string Header { get; set; } = string.Empty;
        public Func<T, string> ValueSelector { get; set; } = _ => string.Empty;
        public float Width { get; set; } = 100f;
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;
    }

    public class PdfFieldGroup
    {
        public string Title { get; set; } = string.Empty;
        public List<KeyValuePair<string, string>> Fields { get; set; } = new();
    }

    public class PdfDetailTable
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Headers { get; set; } = new();
        public List<float> ColumnWidths { get; set; } = new();
        public List<TextAlignment>? Alignments { get; set; }
        public List<List<string>> Rows { get; set; } = new();
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }
}
