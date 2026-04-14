// //using HARD.CORE.API.Configuration;
// using HARD.CORE.API.Models;
// using Newtonsoft.Json;
// using System.Collections.Generic;
// using Telerik.Reporting.Processing;
// using Telerik.Reporting;
// using System;
// using Microsoft.AspNetCore.Mvc;
// using System.IO;
// using System.Linq;
// using HARD.CORE.API.Models.RPT;
// using HARD.CORE.OBJ.Models;

// namespace HARD.CORE.API.Helpers
// {
//     public static class ReportSourceHelper
//     {

//         public static List<ReportParameterModel> GetReportParametersBook(IDictionary<string, object> parameters)
//         {
//             List<ReportParameterModel> reportParameters = new List<ReportParameterModel>();

//             object? parametersObj = null;

//             int? claveBitacoraParam = null;
//             int? claveDetalleSolicitudLlenadoParam = null;

//             if (parameters.TryGetValue("claveBitacora", out parametersObj))
//             {
//                 int claveBitacora = 0;
//                 if (int.TryParse(parametersObj.ToString(), out claveBitacora))
//                 {
//                     claveBitacoraParam = claveBitacora;
//                 }
//             }

//             if (parameters.TryGetValue("claveDetalleSolicitudLlenado", out parametersObj))
//             {
//                 int claveDetalleSolicitudLlenado = 0;
//                 if (int.TryParse(parametersObj.ToString(), out claveDetalleSolicitudLlenado))
//                 {
//                     claveDetalleSolicitudLlenadoParam = claveDetalleSolicitudLlenado;
//                 }

//             }

//             //reportParameters = BitacoraCertificadosDA.GetInstance().Obtener(claveBitacora: claveBitacoraParam, claveDetalleSolicitudLlenado: claveDetalleSolicitudLlenadoParam);

//             return reportParameters;
//         }

//         public static Telerik.Reporting.ReportBook GetReportsBook(List<ReportParameterModel> reportParameters, string designPath = "")
//         {
//             var reportBook = new ReportBook();
//             reportBook.ReportSources.Clear();

//             foreach (var reportParameter in reportParameters)
//             {
//                 string reportName = reportParameter.ReportName;
//                 string parametersJson = JsonConvert.SerializeObject(new { reportParameter.ClaveCertificadoPlantilla, reportParameter.ClaveRemision });

//                 var reportDocument = GetReport(reportName: reportName, parameters: parametersJson, designPath: designPath);
//                 if (reportDocument != null)
//                 {
//                     var report = new InstanceReportSource
//                     {
//                         ReportDocument = reportDocument
//                     };
//                     reportBook.ReportSources.Add(report);
//                 }
//                 else
//                 {
//                     // Crear un reporte de error cuando el reporte no existe
//                     var errorReport = CreateErrorReport($"El reporte '{reportName}' no pudo ser generado. Es posible que el diseño en '{designPath}' no exista");
//                     var errorReportSource = new InstanceReportSource
//                     {
//                         ReportDocument = errorReport
//                     };
//                     reportBook.ReportSources.Add(errorReportSource);
//                 }
//             }
//             return reportBook;
//         }

//         public static List<ReportDataModel> GetReportDataModels(List<ReportParameterModel> reportParameters, string designPath = "")
//         {
//             var reportDataModels = new List<ReportDataModel>();

//             foreach (var reportParameter in reportParameters)
//             {
//                 string reportName = reportParameter.ReportName;
//                 string parametersJson = JsonConvert.SerializeObject(new { reportParameter.ClaveCertificadoPlantilla, reportParameter.ClaveRemision });

//                 ReportDataModel reportData = GetReportDataModel(name: reportName, designPath: designPath, parameters: parametersJson);

//                 if (reportData.RenderingResult != null)
//                 {
//                     reportData.ReportName = $"{reportParameter.ClaveBitacora}_{reportParameter.ClaveRemision}_{reportParameter.ClaveCertificadoPlantilla}_{reportParameter.ReportName}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
//                 }
//                 else
//                 {
//                     reportData.Errors.Add($"El archivo de diseño del reporte no existe en: {reportData.ReportDesignPathFile}");
//                 }

//                 reportDataModels.Add(reportData);
//             }
//             return reportDataModels;
//         }


//         // Método para convertir el resultado de ObtenerReportDataModel a un Telerik Report
//         public static Telerik.Reporting.Report GetReport(string reportName, string parameters = "{}", string designPath = "")
//         {
//             Telerik.Reporting.Report reporte = null;
//             // Invocar tu método personalizado
//             ReportDataModel reportData = GetReportDataModel(name: reportName, designPath: designPath, parameters: parameters);

//             if (reportData.RenderingResult != null)
//             {
//                 // Convertir la ruta del diseño (.trdx) en un Telerik Report
//                 reporte = new Telerik.Reporting.XmlSerialization.ReportXmlSerializer()
//                 .Deserialize(reportData.ReportDesignPathFile) as Telerik.Reporting.Report;

//                 Dictionary<string, string> reportParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters);

//                 foreach (KeyValuePair<string, string> reportParameter in reportParameters)
//                 {
//                     string key = reportParameter.Key.Substring(0, 1).ToLower() + reportParameter.Key.Substring(1);
//                     reporte.ReportParameters.Where(x => x.Name.ToLower().Equals(key.ToLower())).ToList().ForEach(y => y.Value = reportParameter.Value);
//                 }

//                 foreach (var item in reporte.ReportParameters)
//                 {
//                     item.Mergeable = false;
//                     item.Visible = false;
//                 }
//             }

//             return reporte;
//         }

//         public static ReportDataModel GetReportDataModel(string name = "Plantilla", string type = "trdx", string designPath = "", string exportedPath = "", string exportedName = "", string exportedExtension = "pdf", string parameters = "{}")
//         {
//             ReportDataModel reportData = new ReportDataModel();
//             reportData.ReportName = name;
//             reportData.ReportType = type;
//             reportData.ReportDesignPath = designPath;
//             reportData.ReportExportedPath = exportedPath;
//             reportData.ReportExportedName = exportedName;
//             reportData.ReportExportedExtension = exportedExtension;

//             System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();
//             var uriReportSource = new UriReportSource();
//             uriReportSource.Uri = reportData.ReportDesignPathFile;

//             if (System.IO.File.Exists(uriReportSource.Uri))
//             {
//                 Dictionary<string, string> reportParameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(parameters);

//                 foreach (KeyValuePair<string, string> reportParameter in reportParameters)
//                 {
//                     string key = reportParameter.Key.Substring(0, 1).ToLower() + reportParameter.Key.Substring(1);
//                     uriReportSource.Parameters.Add(new Telerik.Reporting.Parameter(key, reportParameter.Value));
//                 }

//                 ReportProcessor reportProcessor = new ReportProcessor();
//                 reportData.RenderingResult = reportProcessor.RenderReport(reportData.ReportExportedExtension.ToUpper(), uriReportSource, deviceInfo);
//             }
//             return (reportData);
//         }

//         public static ReportDataModel GetReportBookDataModel(ReportBook reportBook, string name = "Plantilla", string type = "trdx", string designPath = "", string exportedPath = "", string exportedName = "", string exportedExtension = "pdf")
//         {
//             ReportDataModel reportData = new ReportDataModel();
//             reportData.ReportName = name;
//             reportData.ReportType = type;
//             reportData.ReportDesignPath = designPath;
//             reportData.ReportExportedPath = exportedPath;
//             reportData.ReportExportedName = exportedName;
//             reportData.ReportExportedExtension = exportedExtension;

//             ReportProcessor reportProcessor = new ReportProcessor();
//             System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();
//             reportData.RenderingResult = reportProcessor.RenderReport(reportData.ReportExportedExtension.ToUpper(), reportBook, deviceInfo);

//             return (reportData);
//         }


//         public static ResultModel<string> TransmitFile(ReportDataModel reportData)
//         {
//             ResultModel<string> resultado = new ResultModel<string>();
//             try
//             {
//                 Directory.CreateDirectory(reportData.ReportExportedPath);

//                 if (Directory.Exists(reportData.ReportExportedPath) && reportData.RenderingResult != null)
//                 {
//                     System.IO.File.WriteAllBytes(reportData.ReportExportedPathFile, reportData.RenderingResult.DocumentBytes);
//                     resultado.Success = true;
//                     resultado.Data = $"El reporte ({reportData.ReportName}) ha sido exportado con éxito en ({reportData.ReportExportedPathFile}).";
//                     return (resultado);
//                 }
//                 resultado.Errors.Add($"El directorio ({reportData.ReportExportedPath}) no existe o es inaccesible.");
//             }
//             catch (Exception ex)
//             {
//                 resultado.Errors.Add($"Mensaje de la excepción: {ex.Message}");
//             }

//             resultado.Success = false;
//             resultado.Data = $"Ocurrió un error al exportar el reporte ({reportData.ReportName})";

//             return (resultado);
//         }

//         public static ActionResult ObtenerActionResultError(string error, object data)
//         {
//             ActionResult actionResult;
//             ResultModel<object> resultado = new ResultModel<object>();
//             resultado.Success = false;
//             resultado.Data = data;
//             resultado.Errors.Add($"{error}.");
//             actionResult = new BadRequestObjectResult(resultado);
//             return (actionResult);
//         }

//         private static Telerik.Reporting.Report CreateErrorReport(string errorMessage)
//         {
//             // Crear un nuevo reporte
//             var report = new Telerik.Reporting.Report();

//             // Configurar la página directamente en el reporte (sin ReportPage)
//             report.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
//             report.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(
//                 Telerik.Reporting.Drawing.Unit.Inch(0.5),
//                 Telerik.Reporting.Drawing.Unit.Inch(0.5),
//                 Telerik.Reporting.Drawing.Unit.Inch(0.5),
//                 Telerik.Reporting.Drawing.Unit.Inch(0.5));

//             // Crear una sección Detail (ahora se agrega directamente al reporte)
//             var detailSection = new Telerik.Reporting.DetailSection();
//             detailSection.Height = Telerik.Reporting.Drawing.Unit.Inch(2);

//             // Crear el TextBox con el mensaje de error
//             var textBox = new Telerik.Reporting.TextBox();
//             textBox.Name = "txtErrorMessage";
//             textBox.Value = errorMessage;
//             textBox.Size = new Telerik.Reporting.Drawing.SizeU(
//                 Telerik.Reporting.Drawing.Unit.Inch(7),
//                 Telerik.Reporting.Drawing.Unit.Inch(1));
//             textBox.Location = new Telerik.Reporting.Drawing.PointU(
//                 Telerik.Reporting.Drawing.Unit.Inch(0.5),
//                 Telerik.Reporting.Drawing.Unit.Inch(0.5));
//             textBox.Style.Color = System.Drawing.Color.Red;
//             textBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12);
//             textBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
//             textBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;

//             // Agregar el TextBox a la sección Detail
//             detailSection.Items.Add(textBox);

//             // Agregar la sección Detail al reporte
//             report.Items.Add(detailSection);

//             return report;
//         }

//     }
// }
