using CustomerMvc.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMvc.Domain.Interfaces
{
    public interface ICustomerHttpClientService
    {
        Task<List<CustomerModel>?> GetCustomersAsync();

        Task<CustomerModel?> GetCustomer(int? id);

        Task<CustomerModel?> SaveCustomer(CustomerModel customer);

        Task<bool> UpdateCustomer(int id, CustomerModel customer);

        Task<bool> DeleteCustomer(int id);
    }
}
