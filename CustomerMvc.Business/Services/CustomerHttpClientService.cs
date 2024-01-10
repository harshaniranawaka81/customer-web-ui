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
using CustomerMvc.Domain.Entities;
using AutoMapper;

namespace CustomerMvc.Business.Services
{
    public class CustomerHttpClientService : ICustomerHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IMapper _autoMapper;
        private static readonly string customerApiEndpoint = "CustomerApi:BaseUrl";

        public CustomerHttpClientService(IHttpClientFactory httpClientFactory, IMapper autoMapper, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _autoMapper = autoMapper;
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

        public async Task<List<CustomerModel>?> GetCustomersAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();

            var httpResponseMessage = await httpClient.GetAsync(GetCustomerApiEndpoint());

            List<CustomerModel>? customerModels = null;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                List<Customer>? customers = JsonConvert.DeserializeObject<List<Customer>>(content);

                if (customers != null && customers.Count > 0)
                {
                    customerModels = _autoMapper.Map<List<Customer>?, List<CustomerModel>?>(customers);
                }
            }

            return customerModels;
        }

        public async Task<CustomerModel?> GetCustomer(int? id)
        {
            if (id == 0)
            {
                return null;
            }

            var httpClient = _httpClientFactory.CreateClient();

            var url = GetCustomerApiEndpoint();
            url += $"/{id}";
            var httpResponseMessage = await httpClient.GetAsync(url);

            CustomerModel? customerModel = null;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                Customer? customer = JsonConvert.DeserializeObject<Customer>(content);

                if (customer != null)
                    customerModel = _autoMapper.Map<Customer?, CustomerModel?>(customer);
            }           

            return customerModel;
        }

        public async Task<CustomerModel?> SaveCustomer(CustomerModel customerModel)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(customerModel);
            var postContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(GetCustomerApiEndpoint(), postContent);

            CustomerModel? createdCustomerModel = null;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                Customer? customer = JsonConvert.DeserializeObject<Customer?>(content);

                if(customer != null) 
                    createdCustomerModel = _autoMapper.Map<Customer?, CustomerModel?>(customer);
            }

            return createdCustomerModel;
        }

        public async Task<bool> UpdateCustomer(int id, CustomerModel customer)
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

            return isUpdated;
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            if (id == 0)
            {
                return false;
            }

            var httpClient = _httpClientFactory.CreateClient();

            var url = GetCustomerApiEndpoint();
            url += $"/{id}";
            var httpResponseMessage = await httpClient.DeleteAsync(url);

            if (httpResponseMessage.IsSuccessStatusCode)
                return true;
            else
                return false;
        }
    }
}
