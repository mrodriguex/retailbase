//using Asp.Versioning;

//using HARD.CORE.API.Configuration;
//using HARD.CORE.API.Helpers;
//using HARD.CORE.API.Models;
//using HARD.CORE.API.Models.RPT;
//using HARD.CORE.NEG;

//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;

//using Newtonsoft.Json;

//using System;
//using System.Collections.Generic;

//using Telerik.Reporting;


//namespace HARD.CORE.API.Controllers.RPT
//{
//    [ApiVersion("1.0")]
//    [Route("rpt/v{version:apiVersion}/[controller]")] // Version in the URL path
//    public class ViewerComposedController : Controller
//    {
//        public static bool IsLinux
//        {
//            get
//            {
//                int p = (int)Environment.OSVersion.Platform;
//                return (p == 4) || (p == 6) || (p == 128);
//            }
//        }
//        private IWebHostEnvironment webHostEnvironment;

//        public ViewerComposedController(IWebHostEnvironment webHostEnvironment)
//        {
//            this.webHostEnvironment = webHostEnvironment;
//        }

//        [HttpGet("Index")]
//        public IActionResult Index(string reportName = "ReportBook", string parameters = "{}")
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
//                ReportDataModel reportData = ReportSourceHelper.GetReportDataModel(name: name, type: type, designPath: designPath, exportedName: exportedName, exportedExtension: exportedExtension, parameters: parameters);

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
//        public IActionResult Export(List<string> name = null, string type = "trdx", string designPath = "", string exportedPath = "", string exportedName = "", int claveCorreo = 0, List<string> exportedExtension = null, string parameters = "{}")
//        {
//            ActionResult actionResult;
//            List<ResultModel<string>> resultados = new List<ResultModel<string>>();
//            name ??= new List<string>() { "reportBook" };
//            exportedExtension ??= new List<string>() { "pdf" };
//            try
//            {
//                name.ForEach(reportName =>
//                {
//                    exportedExtension.ForEach(reportExportedExtension =>
//                    {

//                        var parametersBook = JsonConvert.DeserializeObject<Dictionary<string, object>>(parameters);
//                        var reportParameters = ReportSourceHelper.GetReportParametersBook(parameters: parametersBook);
//                        // Crear instancia del ReportBook
//                        ReportBook reportBook = ReportSourceHelper.GetReportsBook(reportParameters: reportParameters, Config.ReportPublish);

//                        var reportBookDataModels = ReportSourceHelper.GetReportBookDataModel(reportBook: reportBook, name: reportName, type: type, designPath: designPath, exportedPath: exportedPath, exportedName: exportedName, exportedExtension: reportExportedExtension);
//                        reportBookDataModels.ReportExportedPath = Config.ReportTargetBooked;

//                        resultados.Add(ReportSourceHelper.TransmitFile(reportBookDataModels));
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
//        public IActionResult Send(List<string> name = null, string type = "trdx", string designPath = "", string exportedPath = "", string exportedName = "", int claveCorreo = 0, List<string> exportedExtension = null, string parameters = "{}")
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
//                        ReportDataModel reportData = ReportSourceHelper.GetReportDataModel(name: reportName, type: type, designPath: designPath, exportedPath: exportedPath, exportedName: exportedName, exportedExtension: reportExportedExtension, parameters: parameters);
//                        attachPathFileNames.Add($"{Config.ReportTarget}\\{reportData.ReportExportedName}");
//                        resultados.Add(ReportSourceHelper.TransmitFile(reportData));
//                    });
//                });

//                string joinedAttachPathFileNames = string.Join(";", attachPathFileNames);

//                resultados.Add(NotificacionB.EnviarCorreoReporte(claveCorreo: claveCorreo, rutaNombreAdjunto: joinedAttachPathFileNames) as ResultModel<string>);

//                actionResult = Ok(resultados);
//            }
//            catch (Exception ex)
//            {
//                actionResult = ReportSourceHelper.ObtenerActionResultError(ex.Message, $"Ocurrió un error al exportar el (los) reporte (s): {string.Join(", ", name)}");
//            }
//            return (actionResult);
//        }

//    }
//}
