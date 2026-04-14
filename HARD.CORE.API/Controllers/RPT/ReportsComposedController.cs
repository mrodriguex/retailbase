//namespace HARD.CORE.API.Controllers.RPT
//{
//    using Asp.Versioning;

//    using HARD.CORE.API.Configuration;
//    using HARD.CORE.API.Helpers;
//    using HARD.CORE.API.Models.RPT;

//    using Microsoft.AspNetCore.Mvc;

//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    using Telerik.Reporting;
//    using Telerik.Reporting.Services;
//    using Telerik.Reporting.Services.AspNetCore;

//    [ApiVersion("1.0")]
//    [Route("rpt/v{version:apiVersion}/[controller]")] // Version in the URL path
//    public class ReportsComposedController : ReportsControllerBase
//    {

//        static readonly ReportServiceConfiguration configurationInstance =
//            new ReportServiceConfiguration
//            {
//                HostAppId = "ReportingNet5App",
//                ReportSourceResolver = new CustomReportSourceResolver(),
//                Storage = new Telerik.Reporting.Cache.File.FileStorage()
//            };

//        public ReportsComposedController(IReportServiceConfiguration reportServiceConfiguration) : base(configurationInstance)
//        {
//            //  this.ReportServiceConfiguration = configurationInstance;
//        }

//        public override IActionResult CreateDocument(string clientID, string instanceID, [FromBody] CreateDocumentArgs args)
//        {
//            object immediatePrintValue = null;
//            if (args.DeviceInfo.TryGetValue("ImmediatePrint", out immediatePrintValue))
//            {
//                bool immediatePrint = Convert.ToBoolean(immediatePrintValue);
//            }
//            return base.CreateDocument(clientID, instanceID, args);
//        }

//    }

//    public class CustomReportSourceResolver : IReportSourceResolver
//    {
//        private List<ReportParameterModel> _reportParameters;
//        private ReportBook _reportBook;

//        List<ReportParameterModel> ReportParameters
//        {
//            get
//            {
//                _reportParameters ??= new List<ReportParameterModel>();
//                return (_reportParameters);
//            }
//            set { _reportParameters = value; }
//        }
//        ReportBook ReportBook
//        {
//            get
//            {
//                _reportBook ??= new ReportBook();
//                return (_reportBook);
//            }
//            set { _reportBook = value; }
//        }

//        public ReportSource Resolve(string reportName, OperationOrigin operationOrigin, IDictionary<string, object> parameters)
//        {
//            if (reportName == "ReportBook")
//            {
//                switch (operationOrigin)
//                {
//                    case OperationOrigin.ResolveReportParameters:
//                        ReportParameters = ReportSourceHelper.GetReportParametersBook(parameters: parameters);
//                        // Crear instancia del ReportBook
//                        ReportBook = ReportSourceHelper.GetReportsBook(reportParameters: ReportParameters, Config.ReportPublish);

//                        //El formato predeterminado es PDF (exportedExtension = "pdf")
//                        var reportDataModels = ReportSourceHelper.GetReportDataModels(reportParameters: ReportParameters, designPath: Config.ReportPublish);

//                        // Retornar como InstanceReportSource
//                        return new InstanceReportSource { ReportDocument = ReportBook };
//                    case OperationOrigin.GenerateReportDocument:
//                    case OperationOrigin.CreateReportInstance:
//                    case OperationOrigin.GetPageLayout:
//                    default:
//                        return new InstanceReportSource { ReportDocument = ReportBook };
//                }
//            }

//            // Manejar otros reportes
//            return new UriReportSource { Uri = reportName };
//        }

//    }

//}
