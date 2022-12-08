using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Perform_OCR_NET_Core.Models;
using Syncfusion.OCRProcessor;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Perform_OCR_NET_Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PerformOCR()
        {
            string binaries = Path.Combine(_hostingEnvironment.ContentRootPath, "Tesseractbinaries", "Windows");
            
            //Initialize OCR processor with tesseract binaries.
            OCRProcessor processor = new OCRProcessor(binaries);
            //Set language to the OCR processor.
            processor.Settings.Language = Languages.English;

            string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Data", "times.ttf");
            FileStream fontStream = new FileStream(path, FileMode.Open);

            //Create a true type font to support unicode characters in PDF.
            processor.UnicodeFont = new PdfTrueTypeFont(fontStream, 8);

            //Set temporary folder to save intermediate files.
            processor.Settings.TempFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Data");

            //Load a PDF document.
            FileStream inputDocument = new FileStream(Path.Combine(_hostingEnvironment.ContentRootPath, "Data", "pdf_succinctly.pdf"), FileMode.Open);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputDocument);

            //Perform OCR with language data.
            string tessdataPath = Path.Combine(_hostingEnvironment.ContentRootPath, "tessdata");
            processor.PerformOCR(loadedDocument, tessdataPath);

            //Save the PDF document.
            MemoryStream outputDocument = new MemoryStream();
            loadedDocument.Save(outputDocument);
            outputDocument.Position = 0;

            //Dispose OCR processor and PDF document.
            processor.Dispose();
            loadedDocument.Close(true);

            //Download the PDF document in the browser.
            FileStreamResult fileStreamResult = new FileStreamResult(outputDocument, "application/pdf");
            fileStreamResult.FileDownloadName = "OCRed_PDF_document.pdf";

            return fileStreamResult;
           
        }
        public IActionResult Privacy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
