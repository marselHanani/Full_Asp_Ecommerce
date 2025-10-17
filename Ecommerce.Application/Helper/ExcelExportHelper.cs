using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using Ecommerce.Application.Dtos.Response;

namespace Ecommerce.Application.Helper
{
    public static class ExcelExportHelper
    {
        private static void ApplyHeaderStyle(IXLRange range)
        {
            range.Style.Font.Bold = true;
            range.Style.Font.FontColor = XLColor.White;
            range.Style.Fill.BackgroundColor = XLColor.FromArgb(54, 96, 146);
            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.OutsideBorderColor = XLColor.FromArgb(31, 56, 100);
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorderColor = XLColor.FromArgb(31, 56, 100);
        }

        private static void ApplyDataStyle(IXLRange range)
        {
            range.Style.Font.FontColor = XLColor.FromArgb(31, 56, 100);
            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.OutsideBorderColor = XLColor.FromArgb(31, 56, 100);
            range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            range.Style.Border.InsideBorderColor = XLColor.FromArgb(31, 56, 100);
        }

        public static byte[] ExportSalesReport(List<SalesReportResponse> data)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Sales Report");

            // Header
            ws.Cell(1, 1).Value = "Date";
            ws.Cell(1, 2).Value = "Total Orders";
            ws.Cell(1, 3).Value = "Total Revenue";
            ws.Cell(1, 4).Value = "Pending";
            ws.Cell(1, 5).Value = "Shipped";
            ws.Cell(1, 6).Value = "Delivered";
            ws.Cell(1, 7).Value = "Cancelled";
            ws.Cell(1, 8).Value = "Paid";
            var headerRange = ws.Range(1, 1, 1, 8);
            ApplyHeaderStyle(headerRange);

            // Data
            int row = 2;
            foreach (var item in data)
            {
                ws.Cell(row, 1).Value = item.Date.ToShortDateString();
                ws.Cell(row, 2).Value = item.TotalOrders;
                ws.Cell(row, 3).Value = item.TotalRevenue;
                ws.Cell(row, 4).Value = item.PendingOrders;
                ws.Cell(row, 5).Value = item.ShippedOrders;
                ws.Cell(row, 6).Value = item.DeliveredOrders;
                ws.Cell(row, 7).Value = item.CancelledOrders;
                ws.Cell(row, 8).Value = item.PaidOrders;
                row++;
            }
            if (data.Count > 0)
                ApplyDataStyle(ws.Range(2, 1, row - 1, 8));

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public static byte[] ExportTopProducts(List<TopProductsResponse> data)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Top Products");

            // Header
            ws.Cell(1, 1).Value = "#";
            ws.Cell(1, 2).Value = "Product Name";
            ws.Cell(1, 3).Value = "Quantity Sold";
            ws.Cell(1, 4).Value = "Total Revenue";
            var headerRange = ws.Range(1, 1, 1, 4);
            ApplyHeaderStyle(headerRange);

            // Data
            int row = 2;
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = item.ProductName;
                ws.Cell(row, 3).Value = item.QuantitySold;
                ws.Cell(row, 4).Value = item.TotalRevenue;
                row++;
            }
            if (data.Count > 0)
                ApplyDataStyle(ws.Range(2, 1, row - 1, 4));

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public static byte[] ExportTopCustomers(List<TopCustomersResponse> data)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Top Customers");

            // Header
            ws.Cell(1, 1).Value = "#";
            ws.Cell(1, 2).Value = "User ID";
            ws.Cell(1, 3).Value = "Full Name";
            ws.Cell(1, 4).Value = "Orders Count";
            ws.Cell(1, 5).Value = "Total Spent";
            var headerRange = ws.Range(1, 1, 1, 5);
            ApplyHeaderStyle(headerRange);

            // Data
            int row = 2;
            for (int i = 0; i < data.Count; i++)
            {
                var customer = data[i];
                ws.Cell(row, 1).Value = i + 1;
                ws.Cell(row, 2).Value = customer.UserId;
                ws.Cell(row, 3).Value = customer.FullName;
                ws.Cell(row, 4).Value = customer.OrdersCount;
                ws.Cell(row, 5).Value = customer.TotalSpent;
                row++;
            }
            if (data.Count > 0)
                ApplyDataStyle(ws.Range(2, 1, row - 1, 5));

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
