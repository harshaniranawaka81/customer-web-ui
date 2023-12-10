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
        Task<KeyValuePair<HttpStatusCode, List<CustomerModel>>> GetCustomersAsync();

        Task<KeyValuePair<HttpStatusCode, CustomerModel?>> GetCustomer(int? id);

        Task<KeyValuePair<HttpStatusCode, CustomerModel?>> SaveCustomer(CustomerModel customer);

        Task<KeyValuePair<HttpStatusCode, bool>> UpdateCustomer(int id, CustomerModel customer);

        Task<KeyValuePair<HttpStatusCode, bool>> DeleteCustomer(int id);
    }
}
