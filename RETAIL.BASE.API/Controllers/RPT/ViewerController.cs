//using Asp.Viewsioning;

//using RETAIL.BASE.API.Configuration;
//using RETAIL.BASE.API.Helpers;
//using RETAIL.BASE.API.Models;
//using RETAIL.BASE.API.Models.RPT;
//using RETAIL.BASE.NEG;

//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;

//using Newtonsoft.Json;

//using System;
//using System.Collections.Generic;
//using System.IO;

//using Telerik.Reporting;
//using Telerik.Reporting.Processing;


//namespace RETAIL.BASE.API.Controllers.RPT
//{
//    [ApiViewsion("1.0")]
//    [Route("rpt/v{version:apiViewsion}/[controller]")] // Viewsion in the URL path
//    public class ViewerController : Controller
//    {
//        public static bool IsLinux
//        {
//            get
//            {
//                int p = (int)Environment.OSViewsion.Platform;
//                return (p == 4) || (p == 6) || (p == 128);
//            }
//        }
//        private IWebHostEnvironment webHostEnvironment;

//        public ViewerController(IWebHostEnvironment webHostEnvironment)
//        {
//            this.webHostEnvironment = webHostEnvironment;
//        }

//        [HttpGet("Index")]
//        public IActionResult Index(string reportName = "Plantilla.trdx", string parameters = "{}")
//        {
//            ViewData["ReportName"] = reportName;
//            ViewData["Parameters"] = parameters;

//            return View();
//        }


//        [HttpGet("Download")]
//        public IActionResult Download(string name = "Plantilla", string type = "trdx", string designPath = "", string exportedName = "", string exportedExtension = "pdf", string parameters = "{}")
//        {
//            ActionResult actionResult;
//            List<ResultModel<string>> resultados = new List<ResultModel<string>>();
//            try
//            {
//                ReportDataModel reportData = ObtenerReportDataModel(name: name, type: type, designPath: designPath, exportedName: exportedName, exportedExtension: exportedExtension, parameters: parameters);

//                actionResult = File(reportData.RenderingResult.DocumentBytes, reportData.ContentType, reportData.ReportExportedName);
//            }
//            catch (Exception ex)
//            {
//                var error = ex.Message;
//                ResultModel<string> resultado = new ResultModel<string>();
//                resultado.Success = false;
//                resultado.Data = $"Ocurrió un error al exportar el reporte ({name})";
//                resultado.Errors.Add($"{error}.");
//                resultados.Add(resultado);
//                actionResult = new BadRequestObjectResult(resultados);
//            }
//            return (actionResult);
//        }

//        [HttpGet("Export")]
//        public IActionResult Export(List<string> name = null, string type = "trdx", string designPath = "", string exportedPath = "", string exportedName = "", int claveEmail = 0, List<string> exportedExtension = null, string parameters = "{}")
//        {
//            ActionResult actionResult;
//            List<ResultModel<string>> resultados = new List<ResultModel<string>>();
//            try
//            {
//                name.ForEach(reportName =>
//                {
//                    exportedExtension.ForEach(reportExportedExtension =>
//                    {
//                        ReportDataModel reportData = ObtenerReportDataModel(name: reportName, type: type, designPath: designPath, exportedPath: exportedPath, exportedName: exportedName, exportedExtension: reportExportedExtension, parameters: parameters);
//                        reportData.ReportExportedPath = Config.ReportTargetSingle;
//                        resultados.Add(ReportSourceHelper.TransmitFile(reportData));
//                    });
//                });
//                actionResult = Ok(resultados);
//            }
//            catch (Exception ex)
//            {
//                actionResult = ReportSourceHelper.ObtenerActionResultError(ex.Message, $"Ocurrió un error al exportar el (los) reporte (s): {string.Join(", ", name)}");
//            }
//            return (actionResult);
//        }

//        [HttpGet("Send")]
//        public IActionResult Send(List<string> name = null, string type = "trdx", string designPath = "", string exportedPath = "", string exportedName = "", int claveEmail = 0, List<string> exportedExtension = null, string parameters = "{}")
//        {
//            ActionResult actionResult;
//            List<ResultModel<string>> resultados = new List<ResultModel<string>>();
//            try
//            {
//                List<string> attachPathFileNames = new List<string>();

//                name.ForEach(reportName =>
//                {
//                    exportedExtension.ForEach(reportExportedExtension =>
//                    {
//                        ReportDataModel reportData = ObtenerReportDataModel(name: reportName, type: type, designPath: designPath, exportedPath: exportedPath, exportedName: exportedName, exportedExtension: reportExportedExtension, parameters: parameters);
//                        attachPathFileNames.Add($"{Config.ReportTarget}\\{reportData.ReportExportedName}");
//                        resultados.Add(ReportSourceHelper.TransmitFile(reportData));
//                    });
//                });

//                string joinedAttachPathFileNames = string.Join(";", attachPathFileNames);

//                resultados.Add(NotificacionB.EnviarEmailReporte(claveEmail: claveEmail, pathNameAdjunto: joinedAttachPathFileNames) as ResultModel<string>);

//                actionResult = Ok(resultados);
//            }
//            catch (Exception ex)
//            {
//                actionResult = ReportSourceHelper.ObtenerActionResultError(ex.Message, $"Ocurrió un error al exportar el (los) reporte (s): {string.Join(", ", name)}");
//            }
//            return (actionResult);
//        }

//        [HttpGet("ObtenerReportDataModel")]
//        private ReportDataModel ObtenerReportDataModel(string name = "Plantilla", string type = "trdx", string designPath = "", string exportedPath = "", string exportedName = "", string exportedExtension = "pdf", string parameters = "{}")
//        {
//            ReportDataModel reportData = new ReportDataModel();
//            reportData.ReportName = name;
//            reportData.ReportType = type;
//            reportData.ReportDesignPath = designPath;
//            reportData.ReportExportedPath = exportedPath;
//            reportData.ReportExportedName = exportedName;
//            reportData.ReportExportedExtension = exportedExtension;

//            System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();
//            var uriReportSource = new UriReportSource();
//            uriReportSource.Uri = reportData.ReportDesignPathFile;

//            Dictionary<string, string> reportParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters);

//            foreach (KeyValuePair<string, string> reportParameter in reportParameters)
//            {
//                uriReportSource.Parameters.Add(new Telerik.Reporting.Parameter(reportParameter.Key, reportParameter.Value));
//            }

//            ReportProcessor reportProcessor = new ReportProcessor();
//            reportData.RenderingResult = reportProcessor.RenderReport(reportData.ReportExportedExtension.ToUpper(), uriReportSource, deviceInfo);
//            return (reportData);
//        }

//    }
//}
