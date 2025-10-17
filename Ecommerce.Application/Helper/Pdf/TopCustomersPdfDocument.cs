using System;
using System.Collections.Generic;
using Ecommerce.Application.Dtos.Response;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;

namespace Ecommerce.Application.Helper.Pdf
{
    public class TopCustomersPdfDocument(List<TopCustomersResponse> data) : IDocument
    {
        private readonly List<TopCustomersResponse> _data = data;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("Top Customers Report")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // #
                            columns.RelativeColumn(2); // UserId
                            columns.RelativeColumn(3); // FullName
                            columns.RelativeColumn(2); // OrdersCount
                            columns.RelativeColumn(2); // TotalSpent
                        });

                        // Table Header
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("User ID");
                            header.Cell().Element(CellStyle).Text("Full Name");
                            header.Cell().Element(CellStyle).Text("Orders");
                            header.Cell().Element(CellStyle).Text("Total Spent");
                        });

                        // Table Rows
                        for (int i = 0; i < _data.Count; i++)
                        {
                            var customer = _data[i];
                            table.Cell().Element(CellStyle).Text((i + 1).ToString());
                            table.Cell().Element(CellStyle).Text(customer.UserId);
                            table.Cell().Element(CellStyle).Text(customer.FullName);
                            table.Cell().Element(CellStyle).Text(customer.OrdersCount.ToString());
                            table.Cell().Element(CellStyle).Text(customer.TotalSpent.ToString("C"));
                        }

                        static IContainer CellStyle(IContainer container) =>
                            container.PaddingVertical(5).PaddingHorizontal(2);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on: ");
                        x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm")).SemiBold();
                    });
            });
        }
    }
}
