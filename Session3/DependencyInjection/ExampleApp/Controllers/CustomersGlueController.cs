using MvcApp.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers
{
    public class CustomersGlueController : Controller
    {
        private CustomerRepository _customers;

        public CustomersGlueController()
        {
            _customers = new CustomerRepository();
        }
        
        public IActionResult Index()
        {
            return View(_customers);
        }

        public IActionResult Add(Customer customer)
        {
            _customers.Add(customer);

            return View(_customers);
        }
    }
}
