using System;
using System.Collections.Generic;
using Ecommerce.Application.Dtos.Response;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

namespace Ecommerce.Application.Helper.Pdf
{
    public class TopProductsPdfDocument(List<TopProductsResponse> data) : IDocument
    {
        private readonly List<TopProductsResponse> _data = data;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text("Top Products Report").FontSize(20).Bold().AlignCenter();
                page.Content().Element(ComposeTable);
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated on: ");
                    x.Span($"{DateTime.Now:yyyy-MM-dd HH:mm}").SemiBold();
                });
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40); // #
                    columns.RelativeColumn(3);  // Product Name
                    columns.RelativeColumn(2);  // Quantity Sold
                    columns.RelativeColumn(2);  // Total Revenue
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#").Bold();
                    header.Cell().Element(CellStyle).Text("Product Name").Bold();
                    header.Cell().Element(CellStyle).Text("Quantity Sold").Bold();
                    header.Cell().Element(CellStyle).Text("Total Revenue").Bold();
                });

                // Rows
                for (int i = 0; i < _data.Count; i++)
                {
                    var item = _data[i];
                    table.Cell().Element(CellStyle).Text((i + 1).ToString());
                    table.Cell().Element(CellStyle).Text(item.ProductName);
                    table.Cell().Element(CellStyle).Text(item.QuantitySold.ToString());
                    table.Cell().Element(CellStyle).Text($"{item.TotalRevenue:C}");
                }
            });
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .PaddingVertical(5)
                .PaddingHorizontal(3)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2);
        }
    }
}
