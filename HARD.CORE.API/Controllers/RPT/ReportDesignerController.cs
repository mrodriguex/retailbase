//namespace HARD.CORE.API.Controllers.RPT
//{
//    using Asp.Versioning;

//    using HARD.CORE.API.Configuration;

//    using Microsoft.AspNetCore.Hosting;
//    using Microsoft.AspNetCore.Mvc;

//    using System;
//    using System.Collections.Generic;
//    using System.Drawing;
//    using System.Drawing.Imaging;
//    using System.IO;
//    using System.Threading.Tasks;

//    using Telerik.Reporting.Services;
//    using Telerik.WebReportDesigner.Services;
//    using Telerik.WebReportDesigner.Services.Controllers;

//    [ApiVersion("1.0")]
//    [Route("rpt/v{version:apiVersion}/[controller]")] // Version in the URL path
//    public class ReportDesignerController : ReportDesignerControllerBase
//    {

//        //static string ReportDesign
//        //{
//        //    get
//        //    {
//        //        return (Path.Combine(Config.ReportDesign));
//        //    }
//        //}

//        public IWebHostEnvironment WebHostEnvironment { get; private set; }

//        public ReportDesignerController(IReportDesignerServiceConfiguration reportDesignerServiceConfiguration, IReportServiceConfiguration reportServiceConfiguration, IWebHostEnvironment webHostEnvironment)
//            : base(reportDesignerServiceConfiguration, reportServiceConfiguration)
//        {
//            WebHostEnvironment = webHostEnvironment;
//        }

//        //public override Task<IActionResult> Save(string resourceName)
//        //{
//        //    return base.Save(resourceName);
//        //}

//        public override Task<IActionResult> SaveReportByUriAsync([FromQuery] string uri)
//        {
//            string reportId = uri;
//            Task<IActionResult> actionResult = null;
//            try
//            {
//                string reportName = reportId.Replace(".trdp", "").Replace(".TRDP", "").Replace(".trdx", "").Replace(".TRDX", "");
//                string reportType = reportId.ToLower().EndsWith(".trdp") ? "trdp" : "trdx";
//                string fileOriginal = Path.Combine(Config.ReportDesign, reportId);
//                if (!Directory.Exists(Config.ReportHistory))
//                {
//                    Directory.CreateDirectory(Config.ReportHistory);
//                }

//                if (System.IO.File.Exists(fileOriginal))
//                {
//                    string fileHistoric = Path.Combine(Config.ReportHistory, $"{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss.fff")}_{reportId}");
//                    System.IO.File.Copy(fileOriginal, fileHistoric, true);
//                }

//                actionResult = base.SaveReportByUriAsync(uri);

//                string filePreviewPath = Path.Combine(Config.ReportDesign, "Preview");
//                string exportedName = $"{reportId}.tiff";
//                string exported2Name = $"{reportId}.jpg";

//                var result = new ViewerController(WebHostEnvironment).Export(
//                    name: new List<string>() { reportName },
//                    type: reportType,
//                    designPath: Config.ReportDesign,
//                    exportedPath: filePreviewPath,
//                    exportedName: exportedName,
//                    exportedExtension: new List<string>() { "image" },
//                    parameters: "{}"
//                    );

//                Bitmap bm = (Bitmap)Bitmap.FromFile(@$"{filePreviewPath}\{exportedName}");
//                bm.Save(@$"{filePreviewPath}\{exported2Name}", ImageFormat.Jpeg);

//            }
//            catch (IOException iox)
//            {
//                Console.WriteLine(iox.Message);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//            return actionResult;
//        }


//        [HttpGet("SaveReportThumb/{reportId}")]
//        public Task<IActionResult> SaveReportThumb(string reportId)
//        {
//            Task<IActionResult> actionResult = null;
//            try
//            {
//                string reportName = reportId.Replace(".trdp", "").Replace(".TRDP", "").Replace(".trdx", "").Replace(".TRDX", "");
//                string reportType = reportId.ToLower().EndsWith(".trdp") ? "trdp" : "trdx";
//                string fileOriginal = Path.Combine(Config.ReportDesign, reportId);

//                string filePreviewPath = Path.Combine(Config.ReportDesign, "Preview");
//                string exportedName = $"{reportId}.tiff";
//                string exported2Name = $"{reportId}.jpg";

//                var result = new ViewerController(WebHostEnvironment).Export(
//                    name: new List<string>() { reportName },
//                    type: reportType,
//                    designPath: Config.ReportDesign,
//                    exportedPath: filePreviewPath,
//                    exportedName: exportedName,
//                    exportedExtension: new List<string>() { "image" },
//                    parameters: "{}"
//                    );

//                Bitmap bm = (Bitmap)Bitmap.FromFile(@$"{filePreviewPath}\{exportedName}");
//                bm.Save(@$"{filePreviewPath}\{exported2Name}", ImageFormat.Jpeg);

//            }
//            catch (IOException iox)
//            {
//                Console.WriteLine(iox.Message);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//            return actionResult;
//        }

//    }
//}
