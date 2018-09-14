using System.Web.Mvc;

namespace AzureB2C.Controllers
{
    public class HomeController : Controller
    {
        // GET: SignUp
        public ActionResult Error()
        {
            return View();
        }

        public ActionResult LogIn()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}