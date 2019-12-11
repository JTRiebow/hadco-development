using System.Web.Mvc;

namespace Hadco.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class WebAppController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }
    }
}