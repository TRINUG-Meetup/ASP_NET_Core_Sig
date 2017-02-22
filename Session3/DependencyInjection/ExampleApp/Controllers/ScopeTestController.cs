using MvcApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers
{
    public class ScopeTestController : Controller
    {
        private IInstanceService _instanceService;
        public ScopeTestController(IInstanceService instanceService)
        {
            _instanceService = instanceService;
        }
        public IActionResult Index()
        {
            ViewData["InstanceId"] = _instanceService.GetInstanceId().ToString();
            return View();
        }
    }
}
