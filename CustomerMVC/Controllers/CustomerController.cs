using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CustomerMvc.Domain.Models;
using CustomerMvc.Domain.Interfaces;
using CustomerMVC.Models;
using System.Net;

namespace CustomerMvc.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerHttpClientService _customerService;

        public CustomerController(ICustomerHttpClientService customerService)
        {
            this._customerService = customerService;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var result = await _customerService.GetCustomersAsync();

            return result.Key switch
            {
                HttpStatusCode.OK => View(result.Value),
                HttpStatusCode.NoContent => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NoContent.ToString(), Message = "There are no customers to display." }),
                HttpStatusCode.BadRequest => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.BadRequest.ToString(), Message = "The request is invalid. Please check your data." }),
                _ => View("Error",
                        new ErrorViewModel { StatusCode = HttpStatusCode.InternalServerError.ToString(), Message = "Internal Server Error!" })
            };
        }

        private async Task<IActionResult> GetCustomer(int? id)
        {
            if (id == null)
            {
                return View("Notification", new NotificationModel { StatusCode = HttpStatusCode.NotFound.ToString(), Message = "No customers found." });
            }

            var result = await _customerService.GetCustomer(id);

            return result.Key switch
            {
                HttpStatusCode.OK => View(result.Value),
                HttpStatusCode.NotFound => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NotFound.ToString(), Message = "No customers found." }),
                HttpStatusCode.BadRequest => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.BadRequest.ToString(), Message = "The request is invalid. Please check your data." }),
                _ => View("Error",
                        new ErrorViewModel { StatusCode = HttpStatusCode.InternalServerError.ToString(), Message = "Internal Server Error!" })
            };
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await GetCustomer(id);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Address")] CustomerModel customerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(customerModel);
            }

            var result = await _customerService.SaveCustomer(customerModel);

            return result.Key switch
            {
                HttpStatusCode.Created => RedirectToAction(nameof(Index)),
                HttpStatusCode.NoContent => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NoContent.ToString(), Message = "No customers found." }),
                HttpStatusCode.BadRequest => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.BadRequest.ToString(), Message = "The request is invalid. Please check your data." }),
                _ => View("Error",
                        new ErrorViewModel { StatusCode = HttpStatusCode.InternalServerError.ToString(), Message = "Internal Server Error!" })
            };
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await GetCustomer(id);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Address")] CustomerModel customerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(customerModel);
            }

            var result = await _customerService.UpdateCustomer(id, customerModel);

            return result.Key switch
            {
                HttpStatusCode.OK => RedirectToAction(nameof(Index)),
                HttpStatusCode.NoContent => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NoContent.ToString(), Message = "No customers found." }),
                HttpStatusCode.BadRequest => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.BadRequest.ToString(), Message = "The request is invalid. Please check your data." }),
                _ => View("Error",
                        new ErrorViewModel { StatusCode = HttpStatusCode.InternalServerError.ToString(), Message = "Internal Server Error!" })
            };
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetCustomer(id);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _customerService.DeleteCustomer(id);

            return result.Key switch
            {
                HttpStatusCode.NoContent => RedirectToAction(nameof(Index)),
                HttpStatusCode.NotFound => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NotFound.ToString(), Message = "No customers found." }),
                HttpStatusCode.BadRequest => View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.BadRequest.ToString(), Message = "The request is invalid. Please check your data." }),
                _ => View("Error",
                        new ErrorViewModel { StatusCode = HttpStatusCode.InternalServerError.ToString(), Message = "Internal Server Error!" })
            };
        }

    }
}
