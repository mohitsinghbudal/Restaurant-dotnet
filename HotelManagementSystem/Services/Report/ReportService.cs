using HotelManagementSystem.Interfaces.Report;
using ClosedXML.Excel;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HotelManagementSystem.Services.Report
{
    public class ReportService : IReportService
    {
        private readonly IReportDLL _service;

        public ReportService(IReportDLL service)
        {
            _service = service;
        }

        public async Task<byte[]> ExportDashboardReportAsync()
        {
            // 1. Retrieve the analytical datasets from the data layer via Dapper
            var data = await _service.GetDashboardReportAsync();

            // 2. Initialize the ClosedXML Workbook
            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Dashboard Metrics");
                //ws.SheetView.ShowGridLines = true;
                ws.SetShowGridLines(true);

                // Palette definitions (Professional Slate Muted Teal Scheme)
                var darkTeal = XLColor.FromHtml("#005B60");
                var lightTeal = XLColor.FromHtml("#E0F2F1");
                var zebraBg = XLColor.FromHtml("#F4FBFB");
                var white = XLColor.White;
                var grayBorder = XLColor.FromHtml("#CCCCCC");

                // ==========================================
                // HEADER BANNER BLOCKS
                // ==========================================
                ws.Cell("A1").Value = "Executive Management Performance Report";
                ws.Cell("A1").Style.Font.FontName = "Segoe UI";
                ws.Cell("A1").Style.Font.FontSize = 16;
                ws.Cell("A1").Style.Font.Bold = true;
                ws.Cell("A1").Style.Font.FontColor = darkTeal;
                ws.Row(1).Height = 30;

                // ==========================================
                // PILLAR SECTION 1: FINANCIAL OVERVIEW CARD
                // ==========================================
                ws.Cell("A3").Value = "Financial Performance Summary";
                ws.Cell("A3").Style.Font.FontName = "Segoe UI";
                ws.Cell("A3").Style.Font.FontSize = 13;
                ws.Cell("A3").Style.Font.Bold = true;
                ws.Cell("A3").Style.Font.FontColor = darkTeal;

                ws.Cell("A4").Value = "Metric Pillar";
                ws.Cell("B4").Value = "Value Representation";

                var financialHeaders = ws.Range("A4:B4");
                financialHeaders.Style.Font.FontName = "Segoe UI";
                financialHeaders.Style.Font.FontSize = 11;
                financialHeaders.Style.Font.Bold = true;
                financialHeaders.Style.Font.FontColor = white;
                financialHeaders.Style.Fill.BackgroundColor = darkTeal;
                financialHeaders.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                financialHeaders.Style.Border.OutsideBorderColor = grayBorder;
                ws.Row(4).Height = 24;

                // Row 5: Total Revenue Key KPI Data Card
                ws.Cell("A5").Value = "Total Revenue Aggregation";
                ws.Cell("B5").Value = data.Summary.TotalRevenue;
                ws.Cell("B5").Style.NumberFormat.Format = "$#,##0.00";

                // Row 6: Volume Card
                ws.Cell("A6").Value = "Total Order Volume";
                ws.Cell("B6").Value = data.Summary.TotalOrders;
                ws.Cell("B6").Style.NumberFormat.Format = "#,##0";

                var financialData = ws.Range("A5:B6");
                financialData.Style.Font.FontName = "Segoe UI";
                financialData.Style.Font.FontSize = 11;
                financialData.Style.Fill.BackgroundColor = lightTeal;
                financialData.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                financialData.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                financialData.Style.Border.InsideBorderColor = grayBorder;
                financialData.Style.Border.OutsideBorderColor = grayBorder;
                ws.Range("B5:B6").Style.Font.Bold = true;
                ws.Row(5).Height = 20;
                ws.Row(6).Height = 20;

                // ==========================================
                // PILLAR SECTION 2: MOST ORDERED ITEMS (LEFT BLOCK)
                // ==========================================
                ws.Cell("A8").Value = "Product Affinity Matrix (Top Ordered Items)";
                ws.Cell("A8").Style.Font.FontName = "Segoe UI";
                ws.Cell("A8").Style.Font.FontSize = 13;
                ws.Cell("A8").Style.Font.Bold = true;
                ws.Cell("A8").Style.Font.FontColor = darkTeal;

                ws.Cell("A9").Value = "Item Descriptor / Name";
                ws.Cell("B9").Value = "Total Quantity Dispatched";

                var itemHeaders = ws.Range("A9:B9");
                itemHeaders.Style.Font.FontName = "Segoe UI";
                itemHeaders.Style.Font.FontSize = 11;
                itemHeaders.Style.Font.Bold = true;
                itemHeaders.Style.Font.FontColor = white;
                itemHeaders.Style.Fill.BackgroundColor = darkTeal;
                ws.Cell("B9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Row(9).Height = 24;

                int itemRow = 10;
                foreach (var item in data.MostOrderedItems)
                {
                    ws.Cell($"A{itemRow}").Value = item.ItemName;
                    ws.Cell($"B{itemRow}").Value = item.TotalQuantity;
                    ws.Cell($"B{itemRow}").Style.NumberFormat.Format = "#,##0";
                    ws.Cell($"B{itemRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    var currentRowRange = ws.Range($"A{itemRow}:B{itemRow}");
                    if (itemRow % 2 != 0)
                    {
                        currentRowRange.Style.Fill.BackgroundColor = zebraBg;
                    }

                    currentRowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    currentRowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    currentRowRange.Style.Border.OutsideBorderColor = grayBorder;
                    currentRowRange.Style.Border.InsideBorderColor = grayBorder;
                    currentRowRange.Style.Font.FontName = "Segoe UI";
                    currentRowRange.Style.Font.FontSize = 11;
                    ws.Row(itemRow).Height = 20;
                    itemRow++;
                }

                // ==========================================
                // PILLAR SECTION 3: REGULAR CUSTOMERS (RIGHT BLOCK)
                // ==========================================
                ws.Cell("D8").Value = "Customer Lifetime Value Matrix (Top Spenders)";
                ws.Cell("D8").Style.Font.FontName = "Segoe UI";
                ws.Cell("D8").Style.Font.FontSize = 13;
                ws.Cell("D8").Style.Font.Bold = true;
                ws.Cell("D8").Style.Font.FontColor = darkTeal;

                ws.Cell("D9").Value = "Session Identity";
                ws.Cell("E9").Value = "Frequency Index";
                ws.Cell("F9").Value = "Aggregate Capital Contributed";

                var customerHeaders = ws.Range("D9:F9");
                customerHeaders.Style.Font.FontName = "Segoe UI";
                customerHeaders.Style.Font.FontSize = 11;
                customerHeaders.Style.Font.Bold = true;
                customerHeaders.Style.Font.FontColor = white;
                customerHeaders.Style.Fill.BackgroundColor = darkTeal;
                ws.Cell("D9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("E9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("F9").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Row(9).Height = 24;

                int custRow = 10;
                foreach (var cust in data.RegularCustomers)
                {
                    ws.Cell($"D{custRow}").Value = cust.SessionId;
                    ws.Cell($"E{custRow}").Value = cust.VisitCount;
                    ws.Cell($"F{custRow}").Value = cust.TotalSpent;

                    ws.Cell($"D{custRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell($"E{custRow}").Style.NumberFormat.Format = "#,##0";
                    ws.Cell($"E{custRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell($"F{custRow}").Style.NumberFormat.Format = "$#,##0.00";
                    ws.Cell($"F{custRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    var currentCustRange = ws.Range($"D{custRow}:F{custRow}");
                    if (custRow % 2 != 0)
                    {
                        currentCustRange.Style.Fill.BackgroundColor = zebraBg;
                    }

                    currentCustRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    currentCustRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    currentCustRange.Style.Border.OutsideBorderColor = grayBorder;
                    currentCustRange.Style.Border.InsideBorderColor = grayBorder;
                    currentCustRange.Style.Font.FontName = "Segoe UI";
                    currentCustRange.Style.Font.FontSize = 11;
                    ws.Row(custRow).Height = 20;
                    custRow++;
                }

                // Set Column Width Formatting Explicitly
                ws.Column(1).Width = 32;
                ws.Column(2).Width = 24;
                ws.Column(3).Width = 6;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 18;
                ws.Column(6).Width = 28;

                // 3. Render out to a memory stream byte array
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}