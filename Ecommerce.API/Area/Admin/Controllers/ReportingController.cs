using Ecommerce.Application.Helper;
using Ecommerce.Application.Helper.Pdf;
using Ecommerce.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using Stripe;
using ReportingService = Ecommerce.Application.Service.ReportingService;

namespace Ecommerce.API.Area.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ReportingController(ReportingService service) : ControllerBase
    {
        private readonly ReportingService _service = service;

        [HttpGet("sales-report")]
        public async Task<IActionResult> GetSalesReport(DateTime from, DateTime to)
        {
            var report = await _service.GetSalesReportAsync(from, to);
            return Ok(report);
        }
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int top)
        {
            var products = await _service.GetTopProductsAsync(top);
            return Ok(products);
        }
        [HttpGet("top-customers")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int top)
        {
            var customers = await _service.GetTopCustomersAsync(top);
            return Ok(customers);
        }

        [HttpGet("sales/pdf")]
        public async Task<IActionResult> GetSalesReportPdf(DateTime from, DateTime to)
        {
            var data = await _service.GetSalesReportAsync(from, to);
            var pdfDoc = new SalesReportPdfDocument(data);
            var pdfBytes = pdfDoc.GeneratePdf();
            return File(pdfBytes, "application/pdf", "SalesReport.pdf");
        }

        [HttpGet("sales/excel")]
        public async Task<IActionResult> GetSalesReportExcel(DateTime from, DateTime to)
        {
            var data = await _service.GetSalesReportAsync(from, to);
            var excelBytes = ExcelExportHelper.ExportSalesReport(data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesReport.xlsx");
        }
        [HttpGet("top-products/pdf")]
        public async Task<IActionResult> GetTopProductsPdf([FromQuery] int top)
        {
            var data = await _service.GetTopProductsAsync(top);
            var pdfDoc = new TopProductsPdfDocument(data);
            var pdfBytes = pdfDoc.GeneratePdf();
            return File(pdfBytes, "application/pdf", "TopProductsReport.pdf");
        }
        [HttpGet("top-products/excel")]
        public async Task<IActionResult> GetTopProductsExcel([FromQuery] int top)
        {
            var data = await _service.GetTopProductsAsync(top);
            var excelBytes = ExcelExportHelper.ExportTopProducts(data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TopProductsReport.xlsx");
        }

        [HttpGet("top-customers/pdf")]
        public async Task<IActionResult> GetTopCustomersPdf([FromQuery] int top)
        {
            var data = await _service.GetTopCustomersAsync(top);
            var pdfDoc = new TopCustomersPdfDocument(data);
            var pdfBytes = pdfDoc.GeneratePdf();
            return File(pdfBytes, "application/pdf", "TopCustomersReport.pdf");
        }
        [HttpGet("top-customers/excel")]
        public async Task<IActionResult> GetTopCustomersExcel([FromQuery] int top)
        {
            var data = await _service.GetTopCustomersAsync(top);
            var excelBytes = ExcelExportHelper.ExportTopCustomers(data);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TopCustomersReport.xlsx");
        }

    }
}
