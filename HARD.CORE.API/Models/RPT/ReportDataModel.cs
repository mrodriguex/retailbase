// //using HARD.CORE.API.Configuration;

// using System.Collections.Generic;
// using System.IO;

// using Telerik.Reporting.Processing;

// namespace HARD.CORE.API.Models.RPT
// {
//     public class ReportDataModel
//     {
//         private string _reportType;
//         private string _reportName;
//         private string _reportDesignPath;
//         private string _reportExportedPath;
//         private string _reportExportedExtension;
//         private string _reportExportedName;

//         public string ReportName
//         {
//             get
//             {
//                 _reportName ??= "Plantilla";
//                 return (_reportName);
//             }
//             set
//             {
//                 _reportName = value;
//             }
//         }

//         public string ReportType
//         {
//             get
//             {
//                 if (string.IsNullOrEmpty(_reportType)) { _reportType = "trdx"; }
//                 return (_reportType);
//             }
//             set
//             {
//                 _reportType = value;
//             }
//         }

//         public string ReportDesignName
//         {
//             get
//             {
//                 return ($"{ReportName}.{ReportType}");
//             }
//         }

//         public string ReportExportedName
//         {
//             get
//             {
//                 if (string.IsNullOrEmpty(_reportExportedName))
//                 {
//                     _reportExportedName = $"{ReportName}.{ReportExportedExtension}";
//                 }
//                 return (_reportExportedName);
//             }
//             set
//             {
//                 _reportExportedName = value;
//             }
//         }

//         public string ReportDesignPath
//         {
//             get
//             {
//                 if (string.IsNullOrEmpty(_reportDesignPath))
//                 {
//                     _reportDesignPath = "";
//                 }
//                 return (_reportDesignPath);
//             }
//             set
//             {
//                 _reportDesignPath = value;
//             }
//         }

//         public string ReportDesignPathFile
//         {
//             get
//             {
//                 return Path.Combine(ReportDesignPath, ReportDesignName);
//             }
//         }

//         public string ReportExportedPath
//         {
//             get
//             {
//                 if (string.IsNullOrEmpty(_reportExportedPath))
//                 {
//                     _reportExportedPath = "";
//                 }
//                 return (_reportExportedPath);
//             }
//             set
//             {
//                 _reportExportedPath = value;
//             }
//         }

//         public string ReportExportedExtension
//         {
//             get
//             {
//                 if (string.IsNullOrEmpty(_reportExportedExtension)) { _reportExportedExtension = "pdf"; }
//                 return (_reportExportedExtension.Trim().ToLower());
//             }
//             set
//             {
//                 _reportExportedExtension = value;
//             }
//         }

//         public string ReportExportedPathFile
//         {
//             get
//             {
//                 return Path.Combine(ReportExportedPath, ReportExportedName);
//             }
//         }

//         public string ContentType
//         {
//             get
//             {
//                 return ReportExportedExtension switch
//                 {
//                     "pdf" => ("application/pdf"),
//                     "xlsx" => ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
//                     _ => ("application/plaintext"),
//                 };
//             }
//         }
//         public RenderingResult RenderingResult { get; set; }

//         public List<string> Errors { get; set; } = new List<string>();
//     }
// }
