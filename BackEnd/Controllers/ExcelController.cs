using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MauiApp1.BackEnd.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp1.BackEnd.Controllers
{
    public class ExcelController
    {
        private Lazy<BookService> BookService;
        private BookService _BookService;

        public ExcelController() {
           
            _BookService = BookService.Value;
        }

        public static async Task<bool> ImportExcelData(XLWorkbook workbook)
        {
            // Implement logic to read Excel file and import data into the database
            // You can use libraries like ClosedXML or EPPlus to handle Excel files
            BookService bookService = new BookService();
            var result = false; //await bookService.ImportBookDataFromExcel(workbook);

            return result;
        }

        public static async Task<bool> ExportExcelData(string path)
        {
            BookService bookService = new BookService();
            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    await bookService.ExportBookData(wb);
                    path= path + "\\MediaExcel.xlsx";
                    wb.SaveAs(path);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
