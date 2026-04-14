//namespace HARD.CORE.API.Controllers.RPT
//{
//    using Asp.Versioning;

//    using HARD.CORE.API.Configuration;

//    using Microsoft.AspNetCore.Mvc;
//    using System.Net;
//    using System.Net.Mail;
//    using Telerik.Reporting.Services;
//    using Telerik.Reporting.Services.AspNetCore;

//    [ApiVersion("1.0")]
//    [Route("rpt/v{version:apiVersion}/[controller]")] // Version in the URL path
//    public class ReportsHistoryController : ReportsControllerBase
//    {

//        static readonly ReportServiceConfiguration configurationInstance =
//     new ReportServiceConfiguration
//     {
//         HostAppId = "ReportingNet5App",
//         ReportSourceResolver = new UriReportSourceResolver(Config.ReportHistory)
//             .AddFallbackResolver(new TypeReportSourceResolver()),
//         Storage = new Telerik.Reporting.Cache.File.FileStorage(),
//     };


//        public ReportsHistoryController(IReportServiceConfiguration reportServiceConfiguration) : base(configurationInstance)
//        {
//            //  this.ReportServiceConfiguration = configurationInstance;
//        }

//        //public ReportsHistoryController(IReportServiceConfiguration reportServiceConfiguration)
//        //    : base(reportServiceConfiguration)
//        //{
//        //}

//        protected override HttpStatusCode SendMailMessage(MailMessage mailMessage)
//        {
//            using (var smtpClient = new SmtpClient("10.100.0.31", 465))
//            {
//                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
//                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
//                smtpClient.EnableSsl = false;

//                smtpClient.Send(mailMessage);
//            }
//            return HttpStatusCode.OK;
//        }

//    }
//}
