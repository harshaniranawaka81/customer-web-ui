using CustomerMvc.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using CustomerMvc.Domain.Interfaces;
using System.Net.Http.Json;

namespace CustomerMvc.Business.Services
{
    public class CustomerHttpClientService : ICustomerHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private static readonly string customerApiEndpoint = "CustomerApi:BaseUrl";

        public CustomerHttpClientService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        private string GetCustomerApiEndpoint()
        {
            var endpoint = _config[$"{customerApiEndpoint}"];

            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException("CustomerApi endpoint not set in config!");
            }

            return endpoint;
        }

        public async Task<KeyValuePair<HttpStatusCode, List<CustomerModel>?>> GetCustomersAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();

            var httpResponseMessage = await httpClient.GetAsync(GetCustomerApiEndpoint());

            List<CustomerModel>? customers = null;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                customers = JsonConvert.DeserializeObject<List<CustomerModel>>(content);
            }

            return new KeyValuePair<HttpStatusCode, List<CustomerModel>?>(httpResponseMessage.StatusCode, customers);
        }

        public async Task<KeyValuePair<HttpStatusCode, CustomerModel?>> GetCustomer(int? id)
        {
            if (id == 0)
            {
                return new KeyValuePair<HttpStatusCode, CustomerModel?>(HttpStatusCode.BadRequest, null);
            }

            var httpClient = _httpClientFactory.CreateClient();

            var url = GetCustomerApiEndpoint();
            url += $"/{id}";
            var httpResponseMessage = await httpClient.GetAsync(url);

            CustomerModel? customer = null;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                customer = JsonConvert.DeserializeObject<CustomerModel>(content);
            }           

            return new KeyValuePair<HttpStatusCode, CustomerModel?>(httpResponseMessage.StatusCode, customer);
        }

        public async Task<KeyValuePair<HttpStatusCode, CustomerModel?>> SaveCustomer(CustomerModel customer)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
            var postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(GetCustomerApiEndpoint(), postContent);

            CustomerModel? createdCustomer = null;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                createdCustomer = JsonConvert.DeserializeObject<CustomerModel?>(content);               
            }

            return new KeyValuePair<HttpStatusCode, CustomerModel?>(httpResponseMessage.StatusCode, createdCustomer);
        }

        public async Task<KeyValuePair<HttpStatusCode, bool>> UpdateCustomer(int id, CustomerModel customer)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var url = GetCustomerApiEndpoint();
            url += $"?id={id}";

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
            var postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = await httpClient.PutAsync(url, postContent);

            bool isUpdated = false;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                isUpdated = JsonConvert.DeserializeObject<bool>(content);
            }

            return new KeyValuePair<HttpStatusCode, bool>(httpResponseMessage.StatusCode, isUpdated);
        }

        public async Task<KeyValuePair<HttpStatusCode, bool>> DeleteCustomer(int id)
        {
            if (id == 0)
            {
                return new KeyValuePair<HttpStatusCode, bool>(HttpStatusCode.BadRequest, false);
            }

            var httpClient = _httpClientFactory.CreateClient();

            var url = GetCustomerApiEndpoint();
            url += $"/{id}";
            var httpResponseMessage = await httpClient.DeleteAsync(url);

            if (httpResponseMessage.IsSuccessStatusCode)
                return new KeyValuePair<HttpStatusCode, bool>(httpResponseMessage.StatusCode, true);
            else
                return new KeyValuePair<HttpStatusCode, bool>(httpResponseMessage.StatusCode, false);
        }
    }
}
