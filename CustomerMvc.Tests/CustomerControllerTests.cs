using CustomerMvc.Controllers;
using CustomerMvc.Domain.Interfaces;
using CustomerMvc.Domain.Models;
using CustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace CustomerMvc.Tests
{
    public class CustomerControllerTests
    {
        private List<CustomerModel> GetCustomerModelList()
        {
            return new List<CustomerModel>()
            {
                new CustomerModel()
                {
                    Id = 1,
                    Name = "Harshani",
                    Email = "harshani@email.com",
                    Address = "aaa"
                },
                new CustomerModel()
                {
                    Id = 2,
                    Name = "Viraj",
                    Email = "viraj@email.com",
                    Address = "bbb"
                },
                new CustomerModel()
                {
                    Id = 3,
                    Name = "saman",
                    Email = "saman@email.com",
                    Address = "ddd"
                }
            };

        }

        #region Index

        [Fact]
        public async Task Index_AllCustomers_ReturnsViewWithCustomers()
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();

            var customers = GetCustomerModelList();

            _ = customerApiServiceMock.Setup(service => service.GetCustomersAsync())
                                  .ReturnsAsync(customers);

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<CustomerModel>>(viewResult.Model);
            Assert.Equal(customers, model);
        }

        [Fact]
        public async Task Index_NoCustomers_ReturnsNotificationView()
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            _ = customerApiServiceMock.Setup(service => service.GetCustomersAsync())
                                  .ReturnsAsync(new List<CustomerModel>());

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Notification", viewResult.ViewName);

            var model = Assert.IsType<NotificationModel>(viewResult.Model);
            Assert.Equal(HttpStatusCode.NoContent.ToString(), model.StatusCode);
            Assert.Equal("There are no customers to display.", model.Message);
        }

        [Fact]
        public async Task Index_ThrowsException_ReturnsException()
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.GetCustomersAsync())
                                  .ThrowsAsync(new Exception("Test exception"));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await controller.Index());
        }

        #endregion

        #region Details

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithCustomer()
        {
            // Arrange
            int? customerId = 1;

            var customer = new CustomerModel { Id = 1, Name = "Harshani", Email = "harshani@email.com", Address = "aaa" };

            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            _ = customerApiServiceMock.Setup(service => service.GetCustomer(customerId))
                                  .ReturnsAsync(customer);

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Details(customerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CustomerModel>(viewResult.Model);
            Assert.Equal(customer, model);
        }
        #endregion

        #region Create

        [Fact]
        public async Task Create_ValidModelState_RedirectsToIndex()
        {
            // Arrange
            var customerModel = new CustomerModel { Id = 1, Name = "Harshani", Email = "harshani@email.com", Address = "aaa" };

            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.SaveCustomer(customerModel))
                                  .ReturnsAsync(customerModel);

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Create(customerModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var customerModel = new CustomerModel { Id = 1, Name = "Harshani", Email = "harshani@email.com", Address = "aaa" };

            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();

            var controller = new CustomerController(customerApiServiceMock.Object);
            controller.ModelState.AddModelError("Phone", "Phone number must be 11 digits.");

            // Act
            var result = await controller.Create(customerModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(result);
            Assert.Equal(customerModel, viewResult.Model);
        }

        #endregion
    }
}