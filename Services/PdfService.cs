using System.Text;

namespace PMS.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateListPdf<T>(
            string title,
            string subtitle,
            List<PdfColumn<T>> columns,
            IEnumerable<T> data,
            string generatedBy,
            Dictionary<string, string>? summaryItems = null,
            bool landscape = false)
        {
            var doc = new PdfDocumentBuilder(landscape);
            doc.Title = title;
            doc.Subtitle = subtitle;
            doc.GeneratedBy = generatedBy;

            // Add summary box if provided
            if (summaryItems != null && summaryItems.Count > 0)
            {
                doc.AddSummaryBox(summaryItems);
            }

            // Convert columns and data to generic table
            var headers = columns.Select(c => c.Header).ToList();
            var widths = columns.Select(c => c.Width).ToList();
            var alignments = columns.Select(c => c.Alignment).ToList();

            var rows = new List<List<string>>();
            foreach (var item in data)
            {
                var row = columns.Select(c => c.ValueSelector(item) ?? "").ToList();
                rows.Add(row);
            }

            doc.AddTable(headers, widths, alignments, rows);
            return doc.Build();
        }

        public byte[] GenerateRecordDetailsPdf(
            string title,
            string recordCode,
            string recordName,
            List<PdfFieldGroup> fieldGroups,
            List<PdfDetailTable>? tables,
            string generatedBy)
        {
            var doc = new PdfDocumentBuilder(false);
            doc.Title = title;
            doc.Subtitle = $"{recordCode} - {recordName}";
            doc.GeneratedBy = generatedBy;

            foreach (var group in fieldGroups)
            {
                doc.AddFieldGroup(group.Title, group.Fields);
            }

            if (tables != null)
            {
                foreach (var table in tables)
                {
                    doc.AddSectionTitle(table.Title);
                    doc.AddTable(table.Headers, table.ColumnWidths, table.Alignments, table.Rows);
                }
            }

            return doc.Build();
        }
    }

    internal class PdfDocumentBuilder
    {
        public string Title { get; set; } = "Report";
        public string Subtitle { get; set; } = "";
        public string GeneratedBy { get; set; } = "System";

        private readonly float _pageWidth;
        private readonly float _pageHeight;
        private readonly float _marginLeft = 36f;
        private readonly float _marginRight = 36f;
        private readonly float _marginTop = 40f;
        private readonly float _marginBottom = 40f;
        private readonly float _contentWidth;

        private class PageContent
        {
            public StringBuilder Stream { get; set; } = new();
        }

        private readonly List<PageContent> _pages = new();
        private PageContent _currentPage;
        private float _currentY;

        public PdfDocumentBuilder(bool landscape = false)
        {
            if (landscape)
            {
                _pageWidth = 841.89f;
                _pageHeight = 595.28f;
            }
            else
            {
                _pageWidth = 595.28f;
                _pageHeight = 841.89f;
            }

            _contentWidth = _pageWidth - _marginLeft - _marginRight;
            _currentPage = NewPage();
        }

        private PageContent NewPage()
        {
            var page = new PageContent();
            _pages.Add(page);
            _currentY = _pageHeight - _marginTop;
            return page;
        }

        public void AddSummaryBox(Dictionary<string, string> items)
        {
            EnsurePageHeaderDrawn();

            var boxHeight = 44f;
            if (_currentY - boxHeight < _marginBottom + 50)
            {
                _currentPage = NewPage();
                EnsurePageHeaderDrawn();
            }

            var itemWidth = _contentWidth / items.Count;
            var sb = _currentPage.Stream;

            // Draw container box
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "0.96 0.97 0.98 rg {0:F2} {1:F2} {2:F2} {3:F2} re f\n",
                _marginLeft, _currentY - boxHeight, _contentWidth, boxHeight);
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "0.85 0.88 0.92 RG 1 w {0:F2} {1:F2} {2:F2} {3:F2} re S\n",
                _marginLeft, _currentY - boxHeight, _contentWidth, boxHeight);

            var idx = 0;
            foreach (var kvp in items)
            {
                var x = _marginLeft + (idx * itemWidth) + 10f;
                // Label
                sb.AppendLine("BT /F1 8 Tf 0.45 0.50 0.58 rg");
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:F2} {1:F2} Td ({2}) Tj ET\n",
                    x, _currentY - 16f, EscapePdfText(kvp.Key.ToUpper()));

                // Value
                sb.AppendLine("BT /F2 11 Tf 0.12 0.16 0.24 rg");
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:F2} {1:F2} Td ({2}) Tj ET\n",
                    x, _currentY - 32f, EscapePdfText(kvp.Value));

                idx++;
            }

            _currentY -= (boxHeight + 14f);
        }

        public void AddFieldGroup(string title, List<KeyValuePair<string, string>> fields)
        {
            EnsurePageHeaderDrawn();

            AddSectionTitle(title);

            var rowHeight = 20f;
            var colWidth = _contentWidth / 2f;

            for (int i = 0; i < fields.Count; i += 2)
            {
                if (_currentY - rowHeight < _marginBottom + 40)
                {
                    _currentPage = NewPage();
                    EnsurePageHeaderDrawn();
                }

                DrawFieldItem(_marginLeft, _currentY, colWidth - 10, fields[i]);

                if (i + 1 < fields.Count)
                {
                    DrawFieldItem(_marginLeft + colWidth, _currentY, colWidth - 10, fields[i + 1]);
                }

                _currentY -= rowHeight;
            }

            _currentY -= 10f;
        }

        private void DrawFieldItem(float x, float y, float width, KeyValuePair<string, string> field)
        {
            var sb = _currentPage.Stream;
            var labelWidth = 110f;

            // Label
            sb.AppendLine("BT /F2 9 Tf 0.35 0.40 0.48 rg");
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:F2} {1:F2} Td ({2}:) Tj ET\n",
                x, y - 12f, EscapePdfText(field.Key));

            // Value
            sb.AppendLine("BT /F1 9 Tf 0.10 0.12 0.18 rg");
            var valText = TruncateText(field.Value, width - labelWidth, 9f);
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:F2} {1:F2} Td ({2}) Tj ET\n",
                x + labelWidth, y - 12f, EscapePdfText(valText));
        }

        public void AddSectionTitle(string title)
        {
            EnsurePageHeaderDrawn();

            if (_currentY - 30f < _marginBottom + 40)
            {
                _currentPage = NewPage();
                EnsurePageHeaderDrawn();
            }

            var sb = _currentPage.Stream;

            // Accent bar
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "0.12 0.44 0.88 rg {0:F2} {1:F2} 3 14 re f\n",
                _marginLeft, _currentY - 14f);

            // Title text
            sb.AppendLine("BT /F2 11 Tf 0.10 0.14 0.22 rg");
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:F2} {1:F2} Td ({2}) Tj ET\n",
                _marginLeft + 8f, _currentY - 12f, EscapePdfText(title));

            _currentY -= 22f;
        }

        public void AddTable(List<string> headers, List<float> columnWidths, List<TextAlignment>? alignments, List<List<string>> rows)
        {
            EnsurePageHeaderDrawn();

            // Normalize column widths to fit content width
            var totalAssigned = columnWidths.Sum();
            var normalizedWidths = new List<float>();
            foreach (var w in columnWidths)
            {
                normalizedWidths.Add((w / totalAssigned) * _contentWidth);
            }

            var headerHeight = 22f;
            var rowHeight = 20f;

            // Draw initial header
            if (_currentY - headerHeight < _marginBottom + 40)
            {
                _currentPage = NewPage();
                EnsurePageHeaderDrawn();
            }

            DrawTableHeader(headers, normalizedWidths, headerHeight);

            var rowIndex = 0;
            foreach (var row in rows)
            {
                if (_currentY - rowHeight < _marginBottom + 30)
                {
                    _currentPage = NewPage();
                    EnsurePageHeaderDrawn();
                    DrawTableHeader(headers, normalizedWidths, headerHeight);
                }

                DrawTableRow(row, normalizedWidths, alignments, rowHeight, rowIndex % 2 == 1);
                rowIndex++;
            }

            _currentY -= 12f;
        }

        private void DrawTableHeader(List<string> headers, List<float> widths, float height)
        {
            var sb = _currentPage.Stream;

            // Background
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "0.11 0.18 0.29 rg {0:F2} {1:F2} {2:F2} {3:F2} re f\n",
                _marginLeft, _currentY - height, _contentWidth, height);

            var curX = _marginLeft;
            for (int i = 0; i < headers.Count; i++)
            {
                var text = headers[i];
                var width = widths[i];
                var safeText = TruncateText(text, width - 8f, 9f);

                sb.AppendLine("BT /F2 8.5 Tf 1 1 1 rg");
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:F2} {1:F2} Td ({2}) Tj ET\n",
                    curX + 6f, _currentY - (height / 2f) - 3f, EscapePdfText(safeText));

                curX += width;
            }

            _currentY -= height;
        }

        private void DrawTableRow(List<string> row, List<float> widths, List<TextAlignment>? alignments, float height, bool isAlt)
        {
            var sb = _currentPage.Stream;

            // Row background
            if (isAlt)
            {
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "0.97 0.98 0.99 rg {0:F2} {1:F2} {2:F2} {3:F2} re f\n",
                    _marginLeft, _currentY - height, _contentWidth, height);
            }

            // Bottom border
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "0.90 0.92 0.95 RG 0.5 w {0:F2} {1:F2} m {2:F2} {1:F2} l S\n",
                _marginLeft, _currentY - height, _marginLeft + _contentWidth);

            var curX = _marginLeft;
            for (int i = 0; i < row.Count && i < widths.Count; i++)
            {
                var text = row[i] ?? "";
                var width = widths[i];
                var align = alignments != null && i < alignments.Count ? alignments[i] : TextAlignment.Left;
                var safeText = TruncateText(text, width - 10f, 8.5f);

                var approxTextLen = safeText.Length * 4.6f;
                var textX = curX + 6f;
                if (align == TextAlignment.Right)
                {
                    textX = curX + width - approxTextLen - 6f;
                }
                else if (align == TextAlignment.Center)
                {
                    textX = curX + ((width - approxTextLen) / 2f);
                }

                sb.AppendLine("BT /F1 8.5 Tf 0.15 0.18 0.24 rg");
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:F2} {1:F2} Td ({2}) Tj ET\n",
                    textX, _currentY - (height / 2f) - 3f, EscapePdfText(safeText));

                curX += width;
            }

            _currentY -= height;
        }

        private bool _headerDrawnOnCurrentPage = false;

        private void EnsurePageHeaderDrawn()
        {
            if (_headerDrawnOnCurrentPage) return;

            var sb = _currentPage.Stream;

            // Primary Top Banner
            var bannerHeight = 44f;
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "0.10 0.15 0.24 rg 0 {0:F2} {1:F2} {2:F2} re f\n",
                _pageHeight - bannerHeight, _pageWidth, bannerHeight);

            // App Brand
            sb.AppendLine("BT /F2 12 Tf 1 1 1 rg");
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:F2} {1:F2} Td (PROJECT MANAGEMENT SYSTEM) Tj ET\n",
                _marginLeft, _pageHeight - 24f);

            // App Sub-Brand
            sb.AppendLine("BT /F1 8 Tf 0.75 0.82 0.95 rg");
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:F2} {1:F2} Td (Enterprise PMS Platform) Tj ET\n",
                _marginLeft, _pageHeight - 36f);

            // Generation Date
            var dateStr = DateTime.Now.ToString("dd MMM yyyy, HH:mm");
            sb.AppendLine("BT /F1 8 Tf 0.85 0.90 0.98 rg");
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:F2} {1:F2} Td (Generated: {2}) Tj ET\n",
                _pageWidth - _marginRight - 160f, _pageHeight - 24f, EscapePdfText(dateStr));

            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:F2} {1:F2} Td (User: {2}) Tj ET\n",
                _pageWidth - _marginRight - 160f, _pageHeight - 36f, EscapePdfText(GeneratedBy));

            _currentY = _pageHeight - bannerHeight - 20f;

            // Document Title
            sb.AppendLine("BT /F2 16 Tf 0.08 0.12 0.20 rg");
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:F2} {1:F2} Td ({2}) Tj ET\n",
                _marginLeft, _currentY, EscapePdfText(Title));

            _currentY -= 16f;

            if (!string.IsNullOrEmpty(Subtitle))
            {
                sb.AppendLine("BT /F1 9.5 Tf 0.40 0.45 0.55 rg");
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:F2} {1:F2} Td ({2}) Tj ET\n",
                    _marginLeft, _currentY, EscapePdfText(Subtitle));
                _currentY -= 14f;
            }

            _currentY -= 10f;
            _headerDrawnOnCurrentPage = true;
        }

        public byte[] Build()
        {
            var totalPages = _pages.Count;

            // Draw footers on each page with page numbers
            for (int i = 0; i < totalPages; i++)
            {
                var sb = _pages[i].Stream;

                // Footer line
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "0.85 0.88 0.92 RG 0.5 w {0:F2} {1:F2} m {2:F2} {1:F2} l S\n",
                    _marginLeft, _marginBottom, _marginLeft + _contentWidth);

                // Footer Left
                sb.AppendLine("BT /F1 7.5 Tf 0.50 0.55 0.65 rg");
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "{0:F2} {1:F2} Td (PMS Enterprise | Confidential & Proprietary Document) Tj ET\n",
                    _marginLeft, _marginBottom - 14f);

                // Footer Right: Page X of Y
                var pageStr = $"Page {i + 1} of {totalPages}";
                sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "BT /F1 7.5 Tf 0.50 0.55 0.65 rg {0:F2} {1:F2} Td ({2}) Tj ET\n",
                    _pageWidth - _marginRight - 60f, _marginBottom - 14f, EscapePdfText(pageStr));
            }

            // Generate complete PDF structure
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.ASCII);

            writer.Write("%%PDF-1.4\n%\xE2\xE3\xCF\xD3\n");
            writer.Flush();

            var offsets = new List<long>();

            // Object 1: Catalog
            offsets.Add(ms.Position);
            writer.Write("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
            writer.Flush();

            // Object 2: Pages
            offsets.Add(ms.Position);
            writer.Write("2 0 obj\n<< /Type /Pages /Count " + totalPages + " /Kids [");
            for (int i = 0; i < totalPages; i++)
            {
                var pageObjNum = 5 + (i * 2);
                writer.Write(pageObjNum + " 0 R ");
            }
            writer.Write("] >>\nendobj\n");
            writer.Flush();

            // Object 3: Font F1 (Helvetica)
            offsets.Add(ms.Position);
            writer.Write("3 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n");
            writer.Flush();

            // Object 4: Font F2 (Helvetica-Bold)
            offsets.Add(ms.Position);
            writer.Write("4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n");
            writer.Flush();

            // Objects for Pages and Contents
            for (int i = 0; i < totalPages; i++)
            {
                var pageObjNum = 5 + (i * 2);
                var contentObjNum = pageObjNum + 1;

                var contentBytes = Encoding.Latin1.GetBytes(_pages[i].Stream.ToString());

                // Page Object
                offsets.Add(ms.Position);
                writer.Write(pageObjNum + " 0 obj\n");
                writer.Write("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 " +
                    _pageWidth.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                    _pageHeight.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "] /Contents " + contentObjNum + " 0 R /Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> >>\nendobj\n");
                writer.Flush();

                // Content Object
                offsets.Add(ms.Position);
                writer.Write(contentObjNum + " 0 obj\n");
                writer.Write("<< /Length " + contentBytes.Length + " >>\nstream\n");
                writer.Flush();

                ms.Write(contentBytes, 0, contentBytes.Length);

                writer.Write("\nendstream\nendobj\n");
                writer.Flush();
            }

            var startXref = ms.Position;
            var totalObjs = offsets.Count + 1;

            writer.Write("xref\n0 " + totalObjs + "\n0000000000 65535 f \n");
            foreach (var off in offsets)
            {
                writer.Write(off.ToString("D10") + " 00000 n \n");
            }

            writer.Write("trailer\n<< /Size " + totalObjs + " /Root 1 0 R >>\nstartxref\n" + startXref + "\n%%EOF\n");
            writer.Flush();

            return ms.ToArray();
        }

        private static string EscapePdfText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            var sb = new StringBuilder(text.Length);
            foreach (var ch in text)
            {
                if (ch == '(') sb.Append("\\(");
                else if (ch == ')') sb.Append("\\)");
                else if (ch == '\\') sb.Append("\\\\");
                else if (ch >= 32 && ch <= 126) sb.Append(ch);
                else sb.Append(' ');
            }
            return sb.ToString();
        }

        private static string TruncateText(string text, float maxWidth, float fontSize)
        {
            if (string.IsNullOrEmpty(text)) return "";
            var maxChars = (int)(maxWidth / (fontSize * 0.52f));
            if (maxChars <= 0) return "";
            if (text.Length <= maxChars) return text;
            if (maxChars <= 3) return text.Substring(0, maxChars);
            return text.Substring(0, maxChars - 3) + "...";
        }
    }
}
