using ExampleApp.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers
{
    public class CustomersController : Controller
    {
        private ICustomerRepository _customers;

        public CustomersController(ICustomerRepository customers)
        {
            _customers = customers;
        }
        
        public IActionResult Index()
        {
            return View(_customers.GetCustomers());
        }
    }
}
