using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CustomerMvc.Domain.Entities;
using CustomerMvc.Domain.Models;

namespace CustomerMvc.Business.Mappers
{
    public class CustomerMapping : Profile
    {
        public CustomerMapping()
        {
            CreateMap<Customer, CustomerModel>();

        }
    }
}