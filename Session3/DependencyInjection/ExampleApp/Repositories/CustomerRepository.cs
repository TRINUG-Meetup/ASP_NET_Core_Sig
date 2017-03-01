using System.Collections.Generic;

namespace MvcApp.Repositories
{
    public class Customer
    {
        public string Name;
    }
    
    public interface ICustomerRepository 
    {
        IEnumerable<Customer> GetCustomers();
        void Add(Customer customer);    
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly IList<Customer> _customers;

        public CustomerRepository()
        {
            _customers = new List<Customer>();

            _customers.Add(new Customer { Name = "Queen Consolidated"});
            _customers.Add(new Customer { Name = "Central City"});
            _customers.Add(new Customer { Name = "S.T.A.R. Labs"});
        }

        public void Add(Customer customer)
        {
            _customers.Add(customer);
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _customers;
        }
    }
}