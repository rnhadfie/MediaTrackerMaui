using ClosedXML.Excel;
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
            return await bookService.ImportBookDataFromExcel(workbook);
        }
    }
}
