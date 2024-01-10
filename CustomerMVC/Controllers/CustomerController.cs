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

            if(result != null && result.Count > 0)
            {
                return View(result);
            }
            else
            {
                return View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NoContent.ToString(), Message = "There are no customers to display." });
            }
        }

        private async Task<IActionResult> GetCustomer(int? id)
        {
            if (id == null)
            {
                return View("Notification", new NotificationModel { StatusCode = HttpStatusCode.NotFound.ToString(), Message = "No matching customer found." });
            }

            var result = await _customerService.GetCustomer(id);

            if (result != null)
            {
                return View(result);
            }
            else
            {
                return View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NotFound.ToString(), Message = "No matching customer found." });
            }
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

            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NoContent.ToString(), Message = "Customer was not added." });
            }        
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

            var IsUpdated = await _customerService.UpdateCustomer(id, customerModel);

            if (IsUpdated)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NoContent.ToString(), Message = "No matching customer found to edit." });
            }
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
            var isDeleted = await _customerService.DeleteCustomer(id);

            if (isDeleted)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("Notification",
                        new NotificationModel { StatusCode = HttpStatusCode.NotFound.ToString(), Message = "No matching customer found to delete." });
            }       
        }

    }
}
