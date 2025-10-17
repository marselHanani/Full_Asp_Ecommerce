using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Application.Dtos.Response;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Elements;

namespace Ecommerce.Application.Helper.Pdf
{
    public class SalesReportPdfDocument(List<SalesReportResponse> data) : IDocument
    {
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(14).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated on: ").SemiBold();
                    x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                });
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Sales Report")
                        .FontSize(24)
                        .Bold()
                        .FontColor(Colors.Blue.Medium);
                    col.Item().Text($"Period: {GetPeriod()}")
                        .FontSize(12)
                        .FontColor(Colors.Grey.Darken2);
                });
                row.ConstantItem(60).Height(60).AlignRight().AlignMiddle().Image(GetLogo(), ImageScaling.FitArea);
            });
            container.PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(90);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Date").Bold();
                    header.Cell().Element(CellStyle).Text("Total Orders").Bold();
                    header.Cell().Element(CellStyle).Text("Total Revenue").Bold();
                    header.Cell().Element(CellStyle).Text("Pending").Bold();
                    header.Cell().Element(CellStyle).Text("Shipped").Bold();
                    header.Cell().Element(CellStyle).Text("Delivered").Bold();
                    header.Cell().Element(CellStyle).Text("Cancelled").Bold();
                });

                // Rows
                foreach (var item in data)
                {
                    table.Cell().Element(CellStyle).Text(item.Date.ToShortDateString());
                    table.Cell().Element(CellStyle).Text(item.TotalOrders.ToString());
                    table.Cell().Element(CellStyle).Text(item.TotalRevenue.ToString("C"));
                    table.Cell().Element(CellStyle).Text(item.PendingOrders.ToString());
                    table.Cell().Element(CellStyle).Text(item.ShippedOrders.ToString());
                    table.Cell().Element(CellStyle).Text(item.DeliveredOrders.ToString());
                    table.Cell().Element(CellStyle).Text(item.CancelledOrders.ToString());
                }

                // Summary Row
                table.Cell().ColumnSpan(1).Element(CellStyle).Text("Total").Bold();
                table.Cell().Element(CellStyle).Text(data.Sum(x => x.TotalOrders).ToString()).Bold();
                table.Cell().Element(CellStyle).Text(data.Sum(x => x.TotalRevenue).ToString("C")).Bold();
                table.Cell().Element(CellStyle).Text(data.Sum(x => x.PendingOrders).ToString()).Bold();
                table.Cell().Element(CellStyle).Text(data.Sum(x => x.ShippedOrders).ToString()).Bold();
                table.Cell().Element(CellStyle).Text(data.Sum(x => x.DeliveredOrders).ToString()).Bold();
                table.Cell().Element(CellStyle).Text(data.Sum(x => x.CancelledOrders).ToString()).Bold();
            });
        }

        static IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten3)
                .PaddingVertical(6)
                .AlignMiddle()
                .AlignLeft();
        }

        string GetPeriod()
        {
            if (data.Count == 0)
                return "-";
            var min = data.Min(x => x.Date).ToShortDateString();
            var max = data.Max(x => x.Date).ToShortDateString();
            return min == max ? min : $"{min} - {max}";
        }

        byte[] GetLogo()
        {
            // Placeholder: Replace with your logo as byte array
            return Placeholders.Image(60, 60);
        }
    }
}
