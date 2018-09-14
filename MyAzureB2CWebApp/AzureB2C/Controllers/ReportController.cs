using System.Web.Mvc;
using AzureB2C.Models;
using AzureB2C.Utils;

namespace AzureB2C.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly string _loggedInUser = LoggedInUserInfo.GetLoginUserEmailId();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TestWithRls()
        {
            var reportProperties = EmbedPowerBiReport.GetPowerBiReportProperties("TestReport");
            var embedConfig = EmbedPowerBiReport.EmbedPbiReport(reportProperties, "Test", _loggedInUser, true);
            var embedConfiguration = new PowerBiEmbedConfig()
            {
                EmbedToken = embedConfig.EmbedToken,
                EmbedUrl = embedConfig.EmbedUrl,
                Id = embedConfig.Id
            };
            ViewBag.Title = "Test Report with RLS";

            return View(embedConfiguration);
        }

        public ActionResult TestWithoutRls()
        {
            var reportProperties = EmbedPowerBiReport.GetPowerBiReportProperties("TestReport");
            var embedConfig = EmbedPowerBiReport.EmbedPbiReport(reportProperties, "Test", _loggedInUser);
            var embedConfiguration = new PowerBiEmbedConfig()
            {
                EmbedToken = embedConfig.EmbedToken,
                EmbedUrl = embedConfig.EmbedUrl,
                Id = embedConfig.Id
            };
            ViewBag.Title = "Test Report without RLS";

            return View(embedConfiguration);
        }
    }
}